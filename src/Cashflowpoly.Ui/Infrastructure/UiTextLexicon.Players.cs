// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui UiTextLexicon.Players.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `UiTextLexicon`.
internal static partial class UiTextLexicon
// Membuka scope tipe UiTextLexicon; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `AddPlayers` dengan hasil bertipe `void`; operasi ini menangani add pemain. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddPlayers(Dictionary<string, (string Id, string En)> terms)
    // Membuka scope metode AddPlayers; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddPlayers.
    {
        // Memperbarui `terms[”players.management”]` menggunakan tuple yang membawa bagian 1: ”Manajemen Pemain”; bagian 2: ”Player Management” dalam
        // AddPlayers.
        terms["players.management"] = ("Manajemen Pemain", "Player Management");
        // Memperbarui `terms[”players.title”]` menggunakan tuple yang membawa bagian 1: ”Direktori Pemain”; bagian 2: ”Player Directory” dalam AddPlayers.
        terms["players.title"] = ("Direktori Pemain", "Player Directory");
        // Memperbarui `terms[”players.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Lihat peserta setiap sesi dan buka analitika performa mereka.”;
        // bagian 2: ”View each session's participants and open their performance analytics.” dalam AddPlayers.
        terms["players.subtitle"] = ("Lihat pemain yang terdaftar dalam sesi Anda dan analitika mereka setelah sesi selesai.", "View players registered in your sessions and their analytics after each session ends.");
        // Memperbarui `terms[”players.player_directory_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Lihat seluruh sesi yang Anda ikuti beserta
        // pemain yang berada dalam sesi yang s...; bagian 2: ”View every session you joined and the players who shared those sessions.” dalam AddPlayers.
        terms["players.player_directory_subtitle"] = ("Lihat seluruh sesi yang Anda ikuti beserta pemain yang berada dalam sesi yang sama.", "View every session you joined and the players who shared those sessions.");
        // Memperbarui `terms[”players.index.player_privacy_hint”]` menggunakan tuple yang membawa bagian 1: ”Analitika permainan hanya tersedia pada baris
        // akun Anda sendiri.”; bagian 2: ”Gameplay analytics are available only on your own account row.” dalam AddPlayers.
        terms["players.index.player_privacy_hint"] = ("Analitika permainan hanya tersedia pada baris akun Anda sendiri.", "Gameplay analytics are available only on your own account row.");
        // Memperbarui `terms[”players.index.you”]` menggunakan tuple yang membawa bagian 1: ”Anda”; bagian 2: ”You” dalam AddPlayers.
        terms["players.index.you"] = ("Anda", "You");
        // Memperbarui `terms[”players.index.view_detail”]` menggunakan tuple yang membawa bagian 1: ”Lihat Analitika”; bagian 2: ”View Analytics” dalam
        // AddPlayers.
        terms["players.index.view_detail"] = ("Lihat Analitika", "View Analytics");
        // Memperbarui `terms[”players.index.detail_unavailable”]` menggunakan tuple yang membawa bagian 1: ”Analitika pemain lain tidak tersedia”; bagian
        // 2: ”Other player analytics are unavailable” dalam AddPlayers.
        terms["players.index.detail_unavailable"] = ("Analitika pemain lain tidak tersedia", "Other player analytics are unavailable");
        // Memperbarui `terms[”players.index.analytics_column”]` menggunakan tuple yang membawa bagian 1: ”Analitika”; bagian 2: ”Analytics” dalam
        // AddPlayers.
        terms["players.index.analytics_column"] = ("Analitika", "Analytics");
        // Memperbarui `terms[”players.total_players”]` menggunakan tuple yang membawa bagian 1: ”Total Pemain”; bagian 2: ”Total Players” dalam AddPlayers.
        terms["players.total_players"] = ("Total Pemain", "Total Players");
        // Memperbarui `terms[”players.total_sessions”]` menggunakan tuple yang membawa bagian 1: ”Total Sesi Diikuti”; bagian 2: ”Total Sessions Joined”
        // dalam AddPlayers.
        terms["players.total_sessions"] = ("Total Sesi Diikuti", "Total Sessions Joined");
        // Memperbarui `terms[”players.index.source_hint”]` menggunakan tuple yang membawa bagian 1: ”Pemain dikelompokkan sesuai keikutsertaan pada setiap
        // sesi.”; bagian 2: ”Players are grouped by participation in each session.” dalam AddPlayers.
        terms["players.index.source_hint"] = ("Pemain dikelompokkan sesuai keikutsertaan pada setiap sesi.", "Players are grouped by participation in each session.");
        // Memperbarui `terms[”players.index.summary_title”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Direktori Pemain”; bagian 2: ”Player
        // Directory Summary” dalam AddPlayers.
        terms["players.index.summary_title"] = ("Ringkasan Direktori Pemain", "Player Directory Summary");
        // Memperbarui `terms[”players.index.summary_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Gunakan ringkasan ini untuk cepat melihat jumlah
        // pemain yang tersedia.”; bagian 2: ”Use this summary to quickly see the number of available players.” dalam AddPlayers.
        terms["players.index.summary_subtitle"] = ("Gunakan ringkasan ini untuk cepat melihat jumlah pemain yang tersedia.", "Use this summary to quickly see the number of available players.");
        // Memperbarui `terms[”players.index.players_with_data”]` menggunakan tuple yang membawa bagian 1: ”Pemain dengan Data Sesi”; bagian 2: ”Players
        // with Session Data” dalam AddPlayers.
        terms["players.index.players_with_data"] = ("Pemain dengan Data Sesi", "Players with Session Data");
        // Memperbarui `terms[”players.index.sessions_with_data”]` menggunakan tuple yang membawa bagian 1: ”Sesi dengan Data Pemain”; bagian 2: ”Sessions
        // with Player Data” dalam AddPlayers.
        terms["players.index.sessions_with_data"] = ("Sesi dengan Data Pemain", "Sessions with Player Data");
        // Memperbarui `terms[”players.index.happiness_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan”; bagian 2: ”Happiness Points”
        // dalam AddPlayers.
        terms["players.index.happiness_points"] = ("Poin Kebahagiaan", "Happiness Points");
        // Memperbarui `terms[”players.index.ungrouped_title”]` menggunakan tuple yang membawa bagian 1: ”Daftar Pemain Umum”; bagian 2: ”General Player
        // List” dalam AddPlayers.
        terms["players.index.ungrouped_title"] = ("Daftar Pemain Umum", "General Player List");
        terms["players.monitored_players"] = ("Pemain Dipantau", "Monitored Players");
        terms["players.index.monitored_subtitle"] = ("Peserta sesi yang Anda kelola. Setiap pemain dihitung satu kali, termasuk peserta sesi yang belum dimulai.", "Participants in the sessions you manage. Each player is counted once, including participants in sessions that have not started.");
        terms["players.index.no_session_participants"] = ("Belum ada pemain yang terdaftar dalam sesi Anda. Tambahkan pemain ke sesi agar muncul di direktori ini.", "No players are registered in your sessions yet. Add players to a session to see them in this directory.");
        terms["players.index.without_session_title"] = ("Pemain Belum Masuk Sesi", "Players Without a Session");
        terms["players.index.without_session_subtitle"] = ("Pemain berikut belum bergabung ke sesi yang tersedia di direktori ini.", "These players have not joined any session available in this directory.");
        // Memperbarui `terms[”players.index.ungrouped_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Belum ada pengelompokan per sesi, namun daftar
        // identitas pemain tetap tersedia....; bagian 2: ”Per-session grouping is not available yet, but the player identity list is stil... dalam
        // AddPlayers.
        terms["players.index.ungrouped_subtitle"] = ("Pengelompokan sesi belum lengkap. Pemain yang belum tercantum dalam kelompok sesi tetap ditampilkan di sini.", "Session grouping is incomplete. Players not listed in a session group are still shown here.");
        // Memperbarui `terms[”players.index.ungrouped_hint”]` menggunakan tuple yang membawa bagian 1: ”Saat analitika sesi sudah tersedia, daftar ini
        // otomatis berubah menjadi tampila...; bagian 2: ”When session analytics become available, this list will automatically switch to... dalam
        // AddPlayers.
        terms["players.index.ungrouped_hint"] = ("Pemain yang bergabung ke sesi akan tercantum dalam kelompok sesi. Analitika tersedia setelah sesi selesai.", "Players who join a session appear in its group. Analytics are available after the session ends.");
        // Memperbarui `terms[”players.index.grouped_title”]` menggunakan tuple yang membawa bagian 1: ”Pemain per Sesi”; bagian 2: ”Players by Session”
        // dalam AddPlayers.
        terms["players.index.grouped_title"] = ("Pemain per Sesi", "Players by Session");
        // Memperbarui `terms[”players.index.grouped_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Header hasil selalu tersedia; data pemain dan
        // analitika baru ditampilkan setela...; bagian 2: ”Result headers are always available; player data and analytics appear only afte... dalam
        // AddPlayers.
        terms["players.index.grouped_subtitle"] = ("Header hasil selalu tersedia; data pemain dan analitika baru ditampilkan setelah sesi selesai.", "Result headers are always available; player data and analytics appear only after the session ends.");
        // Memperbarui `terms[”players.index.grouped_subtitle.player”]` menggunakan tuple yang membawa bagian 1: ”Header hasil selalu tersedia. Setelah sesi
        // selesai, data seluruh peserta tampil...; bagian 2: ”Result headers are always available. After the session ends, all participant da... dalam
        // AddPlayers.
        terms["players.index.grouped_subtitle.player"] = ("Header hasil selalu tersedia. Setelah sesi selesai, data seluruh peserta tampil dan analitika hanya dapat dibuka untuk akun Anda sendiri.", "Result headers are always available. After the session ends, all participant data appears while analytics remain available only for your own account.");
        // Memperbarui `terms[”players.index.results_pending”]` menggunakan tuple yang membawa bagian 1: ”Data pemain akan ditampilkan setelah sesi
        // selesai.”; bagian 2: ”Player data will appear after the session ends.” dalam AddPlayers.
        terms["players.index.results_pending"] = ("Data pemain akan ditampilkan setelah sesi selesai.", "Player data will appear after the session ends.");
        // Memperbarui `terms[”players.tips.title”]` menggunakan tuple yang membawa bagian 1: ”Tips penggunaan menu Pemain”; bagian 2: ”Players menu tips”
        // dalam AddPlayers.
        terms["players.tips.title"] = ("Tips penggunaan menu Pemain", "Players menu tips");
        // Memperbarui `terms[”players.tip_key.validate”]` menggunakan tuple yang membawa bagian 1: ”Validasi”; bagian 2: ”Validation” dalam AddPlayers.
        terms["players.tip_key.validate"] = ("Validasi", "Validation");
        // Memperbarui `terms[”players.tip_key.tracking”]` menggunakan tuple yang membawa bagian 1: ”Pelacakan”; bagian 2: ”Tracking” dalam AddPlayers.
        terms["players.tip_key.tracking"] = ("Pelacakan", "Tracking");
        // Memperbarui `terms[”players.tip_key.analysis”]` menggunakan tuple yang membawa bagian 1: ”Analisis”; bagian 2: ”Analysis” dalam AddPlayers.
        terms["players.tip_key.analysis"] = ("Analisis", "Analysis");
        // Memperbarui `terms[”players.tip.validate”]` menggunakan tuple yang membawa bagian 1: ”Buka kelompok sesi untuk memastikan nama dan urutan pemain
        // sesuai peserta yang ...; bagian 2: ”Open a session group to confirm that names and player order match the actual pa... dalam AddPlayers.
        terms["players.tip.validate"] = ("Buka kelompok sesi untuk memastikan nama dan urutan pemain sesuai peserta yang benar-benar bermain.", "Open a session group to confirm that names and player order match the actual participants.");
        // Memperbarui `terms[”players.tip.tracking”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan orang yang bermain bersama pada tiap sesi tanpa
        // membuka data pribadi...; bagian 2: ”Compare who played together in each session without opening another player's pr... dalam AddPlayers.
        terms["players.tip.tracking"] = ("Bandingkan orang yang bermain bersama pada tiap sesi tanpa membuka data pribadi pemain lain.", "Compare who played together in each session without opening another player's private data.");
        // Memperbarui `terms[”players.tip.analysis”]` menggunakan tuple yang membawa bagian 1: ”Tekan Lihat Analitika pada baris yang diizinkan untuk
        // membaca hasil, arus kas, ...; bagian 2: ”Select View Analytics on an allowed row to review results, cashflow, and the pl... dalam AddPlayers.
        terms["players.tip.analysis"] = ("Tekan Lihat Analitika pada baris yang diizinkan untuk membaca hasil, arus kas, dan pola keputusan pemain.", "Select View Analytics on an allowed row to review results, cashflow, and the player's decision patterns.");
        // Memperbarui `terms[”players.count_suffix”]` menggunakan tuple yang membawa bagian 1: ”pemain”; bagian 2: ”players” dalam AddPlayers.
        terms["players.count_suffix"] = ("pemain", "players");
        // Memperbarui `terms[”players.winner_badge”]` menggunakan tuple yang membawa bagian 1: ”Juara 1”; bagian 2: ”Winner 1” dalam AddPlayers.
        terms["players.winner_badge"] = ("Juara 1", "Winner 1");
        // Memperbarui `terms[”players.champion.donation_short”]` menggunakan tuple yang membawa bagian 1: ”Donasi”; bagian 2: ”Donation” dalam AddPlayers.
        terms["players.champion.donation_short"] = ("Donasi", "Donation");
        // Memperbarui `terms[”players.champion.pension_short”]` menggunakan tuple yang membawa bagian 1: ”Pensiun”; bagian 2: ”Pension” dalam AddPlayers.
        terms["players.champion.pension_short"] = ("Pensiun", "Pension");
        // Memperbarui `terms[”players.add”]` menggunakan tuple yang membawa bagian 1: ”Tambah”; bagian 2: ”Add” dalam AddPlayers.
        terms["players.add"] = ("Tambah", "Add");
        // Memperbarui `terms[”players.name_placeholder”]` menggunakan tuple yang membawa bagian 1: ”Nama pemain”; bagian 2: ”Player name” dalam AddPlayers.
        terms["players.name_placeholder"] = ("Nama pemain", "Player name");
        // Memperbarui `terms[”players.detail_title”]` menggunakan tuple yang membawa bagian 1: ”Analitika Pemain pada Sesi”; bagian 2: ”Player Session
        // Analytics” dalam AddPlayers.
        terms["players.detail_title"] = ("Analitika Pemain pada Sesi", "Player Session Analytics");
        // Memperbarui `terms[”players.detail.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Lihat hasil akhir, keputusan, dan perjalanan uang pemain
        // dalam satu halaman.”; bagian 2: ”See the player's final result, decisions, and money journey on one page.” dalam AddPlayers.
        terms["players.detail.subtitle"] = ("Lihat hasil akhir, keputusan, dan perjalanan uang pemain dalam satu halaman.", "See the player's final result, decisions, and money journey on one page.");
        // Memperbarui `terms[”players.detail.nav_back_players”]` menggunakan tuple yang membawa bagian 1: ”Kembali ke Direktori Pemain”; bagian 2: ”Back to
        // Player Directory” dalam AddPlayers.
        terms["players.detail.nav_back_players"] = ("Kembali ke Direktori Pemain", "Back to Player Directory");
        // Memperbarui `terms[”players.detail.nav_back_session”]` menggunakan tuple yang membawa bagian 1: ”Kembali ke Analitika Sesi”; bagian 2: ”Back to
        // Session Analytics” dalam AddPlayers.
        terms["players.detail.nav_back_session"] = ("Kembali ke Analitika Sesi", "Back to Session Analytics");
        // Memperbarui `terms[”players.detail.empty_analytics”]` menggunakan tuple yang membawa bagian 1: ”Data gameplay pemain belum tersedia pada sesi
        // ini. Pastikan event pemain sudah ...; bagian 2: ”Player gameplay data is not available in this session yet. Ensure player events... dalam
        // AddPlayers.
        terms["players.detail.empty_analytics"] = ("Data gameplay pemain belum tersedia pada sesi ini. Pastikan event pemain sudah masuk lalu coba muat ulang.", "Player gameplay data is not available in this session yet. Ensure player events are submitted, then refresh.");
        // Memperbarui `terms[”players.error.load_session_analytics_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat analitika sesi. Status:
        // {status}”; bagian 2: ”Failed to load session analytics. Status: {status}” dalam AddPlayers.
        terms["players.error.load_session_analytics_failed"] = ("Gagal memuat analitika sesi. Status: {status}", "Failed to load session analytics. Status: {status}");
        // Memperbarui `terms[”players.error.load_transactions_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat transaksi. Status:
        // {status}”; bagian 2: ”Failed to load transactions. Status: {status}” dalam AddPlayers.
        terms["players.error.load_transactions_failed"] = ("Gagal memuat transaksi. Status: {status}", "Failed to load transactions. Status: {status}");
        // Memperbarui `terms[”players.error.load_gameplay_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat data permainan. Status:
        // {status}”; bagian 2: ”Failed to load gameplay data. Status: {status}” dalam AddPlayers.
        terms["players.error.load_gameplay_failed"] = ("Gagal memuat data permainan. Status: {status}", "Failed to load gameplay data. Status: {status}");
        // Memperbarui `terms[”players.error.load_player_directory_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat daftar pemain. Status:
        // {status}”; bagian 2: ”Failed to load player list. Status: {status}” dalam AddPlayers.
        terms["players.error.load_player_directory_failed"] = ("Gagal memuat daftar pemain. Status: {status}", "Failed to load player list. Status: {status}");
        // Memperbarui `terms[”players.error.load_sessions_grouping_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat daftar sesi untuk
        // pengelompokan pemain. Status: {status}”; bagian 2: ”Failed to load session list for player grouping. Status: {status}” dalam AddPlayers.
        terms["players.error.load_sessions_grouping_failed"] = ("Gagal memuat daftar sesi untuk pengelompokan pemain. Status: {status}", "Failed to load session list for player grouping. Status: {status}");
        // Memperbarui `terms[”players.error.load_session_details_partial”]` menggunakan tuple yang membawa bagian 1: ”Sebagian rincian sesi tidak dapat
        // dimuat; data yang tidak tersedia tidak dihitu...; bagian 2: ”Some session details could not be loaded; unavailable data is not included.” dalam
        // AddPlayers.
        terms["players.error.load_session_details_partial"] = ("Sebagian rincian sesi tidak dapat dimuat; data yang tidak tersedia tidak dihitung.", "Some session details could not be loaded; unavailable data is not included.");
        // Memperbarui `terms[”players.economy_snapshot”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Ekonomi”; bagian 2: ”Economy Snapshot” dalam
        // AddPlayers.
        terms["players.economy_snapshot"] = ("Ringkasan Ekonomi", "Economy Snapshot");
        // Memperbarui `terms[”players.mission_risk”]` menggunakan tuple yang membawa bagian 1: ”Misi dan Risiko”; bagian 2: ”Mission and Risk” dalam
        // AddPlayers.
        terms["players.mission_risk"] = ("Misi dan Risiko", "Mission and Risk");
        // Memperbarui `terms[”players.stats.badge”]` menggunakan tuple yang membawa bagian 1: ”Hasil Pemain”; bagian 2: ”Player Results” dalam AddPlayers.
        terms["players.stats.badge"] = ("Hasil Pemain", "Player Results");
        // Memperbarui `terms[”players.stats.title”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Statistik Pemain”; bagian 2: ”Player Statistics
        // Summary” dalam AddPlayers.
        terms["players.stats.title"] = ("Ringkasan Statistik Pemain", "Player Statistics Summary");
        // Memperbarui `terms[”players.stats.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Lihat kesimpulan utama. Buka analisis dan data permainan
        // untuk mengetahui penye...; bagian 2: ”See the main conclusion, then open the analysis and gameplay data to understand... dalam AddPlayers.
        terms["players.stats.subtitle"] = ("Lihat kesimpulan utama. Buka analisis dan data permainan untuk mengetahui penyebabnya.", "See the main conclusion, then open the analysis and gameplay data to understand why.");
        // Memperbarui `terms[”players.stats.verdict.title”]` menggunakan tuple yang membawa bagian 1: ”Kesimpulan cepat”; bagian 2: ”Quick conclusion”
        // dalam AddPlayers.
        terms["players.stats.verdict.title"] = ("Kesimpulan cepat", "Quick conclusion");
        // Memperbarui `terms[”players.stats.mission.title”]` menggunakan tuple yang membawa bagian 1: ”Misi Koleksi”; bagian 2: ”Collection Mission” dalam
        // AddPlayers.
        terms["players.stats.mission.title"] = ("Misi Koleksi", "Collection Mission");
        // Memperbarui `terms[”players.stats.mission.complete”]` menggunakan tuple yang membawa bagian 1: ”Selesai”; bagian 2: ”Completed” dalam AddPlayers.
        terms["players.stats.mission.complete"] = ("Selesai", "Completed");
        // Memperbarui `terms[”players.stats.mission.incomplete”]` menggunakan tuple yang membawa bagian 1: ”Belum selesai”; bagian 2: ”Not completed” dalam
        // AddPlayers.
        terms["players.stats.mission.incomplete"] = ("Belum selesai", "Not completed");
        // Memperbarui `terms[”players.stats.mission.unavailable”]` menggunakan tuple yang membawa bagian 1: ”Belum ada data”; bagian 2: ”No data available”
        // dalam AddPlayers.
        terms["players.stats.mission.unavailable"] = ("Belum ada data", "No data available");
        // Memperbarui `terms[”players.stats.status.danger”]` menggunakan tuple yang membawa bagian 1: ”Butuh tindakan segera”; bagian 2: ”Immediate action
        // needed” dalam AddPlayers.
        terms["players.stats.status.danger"] = ("Butuh tindakan segera", "Immediate action needed");
        // Memperbarui `terms[”players.stats.status.danger.desc”]` menggunakan tuple yang membawa bagian 1: ”Ada pinjaman belum lunas yang langsung
        // memengaruhi hasil akhir. Prioritaskan pe...; bagian 2: ”An unpaid loan directly affects the final result. Prioritize its cause and repa... dalam
        // AddPlayers.
        terms["players.stats.status.danger.desc"] = ("Ada pinjaman belum lunas yang langsung memengaruhi hasil akhir. Prioritaskan penyebab dan rencana pelunasannya.", "An unpaid loan directly affects the final result. Prioritize its cause and repayment plan.");
        // Memperbarui `terms[”players.stats.status.warning”]` menggunakan tuple yang membawa bagian 1: ”Perlu ditinjau”; bagian 2: ”Needs review” dalam
        // AddPlayers.
        terms["players.stats.status.warning"] = ("Perlu ditinjau", "Needs review");
        // Memperbarui `terms[”players.stats.status.warning.desc”]` menggunakan tuple yang membawa bagian 1: ”Beberapa hasil perlu diperbaiki. Buka bagian
        // analisis untuk melihat keputusan y...; bagian 2: ”Some results need improvement. Open the analysis to see which decisions should ... dalam
        // AddPlayers.
        terms["players.stats.status.warning.desc"] = ("Beberapa hasil perlu diperbaiki. Buka bagian analisis untuk melihat keputusan yang perlu dibahas.", "Some results need improvement. Open the analysis to see which decisions should be discussed.");
        // Memperbarui `terms[”players.stats.status.positive”]` menggunakan tuple yang membawa bagian 1: ”Performa sangat kuat”; bagian 2: ”Very strong
        // performance” dalam AddPlayers.
        terms["players.stats.status.positive"] = ("Performa sangat kuat", "Very strong performance");
        // Memperbarui `terms[”players.stats.status.positive.desc”]` menggunakan tuple yang membawa bagian 1: ”Kas bertambah, kebutuhan lebih seimbang, dan
        // Poin Kebahagiaan tinggi.”; bagian 2: ”Cash increased, needs were more balanced, and Happiness Points were high.” dalam AddPlayers.
        terms["players.stats.status.positive.desc"] = ("Kas bertambah, kebutuhan lebih seimbang, dan Poin Kebahagiaan tinggi.", "Cash increased, needs were more balanced, and Happiness Points were high.");
        // Memperbarui `terms[”players.stats.status.neutral”]` menggunakan tuple yang membawa bagian 1: ”Kondisi cukup stabil”; bagian 2: ”Fairly stable
        // condition” dalam AddPlayers.
        terms["players.stats.status.neutral"] = ("Kondisi cukup stabil", "Fairly stable condition");
        // Memperbarui `terms[”players.stats.status.neutral.desc”]` menggunakan tuple yang membawa bagian 1: ”Tidak ada masalah mendesak, tetapi hasil
        // pemain masih bisa ditingkatkan.”; bagian 2: ”There is no urgent problem, but the player's result can still improve.” dalam AddPlayers.
        terms["players.stats.status.neutral.desc"] = ("Tidak ada masalah mendesak, tetapi hasil pemain masih bisa ditingkatkan.", "There is no urgent problem, but the player's result can still improve.");
        // Memperbarui `terms[”players.stats.status.unavailable”]` menggunakan tuple yang membawa bagian 1: ”Data belum tersedia”; bagian 2: ”Data
        // unavailable” dalam AddPlayers.
        terms["players.stats.status.unavailable"] = ("Data belum tersedia", "Data unavailable");
        // Memperbarui `terms[”players.stats.status.unavailable.desc”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan belum dapat dinilai karena data
        // analitik pemain belum tersedia.”; bagian 2: ”The summary cannot be evaluated because the player's analytics data is unavaila... dalam AddPlayers.
        terms["players.stats.status.unavailable.desc"] = ("Ringkasan belum dapat dinilai karena data analitik pemain belum tersedia.", "The summary cannot be evaluated because the player's analytics data is unavailable.");
        // Memperbarui `terms[”players.stats.pillars.title”]` menggunakan tuple yang membawa bagian 1: ”Tiga pilar hasil pemain”; bagian 2: ”Three
        // player-result pillars” dalam AddPlayers.
        terms["players.stats.pillars.title"] = ("Tiga pilar hasil pemain", "Three player-result pillars");
        // Memperbarui `terms[”players.stats.pillars.desc”]` menggunakan tuple yang membawa bagian 1: ”Keuangan, disiplin bermain, dan Poin Kebahagiaan
        // ditampilkan sekali tanpa pengu...; bagian 2: ”Finances, play discipline, and the Happiness Score are each shown once without ... dalam
        // AddPlayers.
        terms["players.stats.pillars.desc"] = ("Keuangan, disiplin bermain, dan Poin Kebahagiaan ditampilkan sekali tanpa pengulangan.", "Finances, play discipline, and the Happiness Score are each shown once without repetition.");
        // Memperbarui `terms[”players.stats.pillar.main_result”]` menggunakan tuple yang membawa bagian 1: ”Hasil utama”; bagian 2: ”Main result” dalam
        // AddPlayers.
        terms["players.stats.pillar.main_result"] = ("Hasil utama", "Main result");
        // Memperbarui `terms[”players.stats.pillar.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Baiknya:”; bagian 2: ”Ideally:” dalam
        // AddPlayers.
        terms["players.stats.pillar.recommendation"] = ("Baiknya:", "Ideally:");
        // Memperbarui `terms[”players.stats.library.badge”]` menggunakan tuple yang membawa bagian 1: ”Data terpilih”; bagian 2: ”Selected data” dalam
        // AddPlayers.
        terms["players.stats.library.badge"] = ("Data terpilih", "Selected data");
        // Memperbarui `terms[”players.stats.library.title”]` menggunakan tuple yang membawa bagian 1: ”Pustaka Data Pemain”; bagian 2: ”Player Data
        // Library” dalam AddPlayers.
        terms["players.stats.library.title"] = ("Pustaka Data Pemain", "Player Data Library");
        // Memperbarui `terms[”players.stats.library.desc”]` menggunakan tuple yang membawa bagian 1: ”Hanya data permainan dan hasil analisis yang memberi
        // informasi unik. Data tekni...; bagian 2: ”Only gameplay data and analysis results that add unique information are shown. ... dalam AddPlayers.
        terms["players.stats.library.desc"] = ("Hanya data permainan dan hasil analisis yang memberi informasi unik. Data teknis atau nilai yang sudah tampil di bagian lain tidak diulang.", "Only gameplay data and analysis results that add unique information are shown. Technical data and values already shown elsewhere are not repeated.");
        // Memperbarui `terms[”players.stats.library.raw_count”]` menggunakan tuple yang membawa bagian 1: ”data permainan”; bagian 2: ”gameplay data” dalam
        // AddPlayers.
        terms["players.stats.library.raw_count"] = ("data permainan", "gameplay data");
        // Memperbarui `terms[”players.stats.library.derived_count”]` menggunakan tuple yang membawa bagian 1: ”hasil analisis”; bagian 2: ”analysis
        // results” dalam AddPlayers.
        terms["players.stats.library.derived_count"] = ("hasil analisis", "analysis results");
        // Memperbarui `terms[”players.stats.library.session_mode”]` menggunakan tuple yang membawa bagian 1: ”mode sesi”; bagian 2: ”session mode” dalam
        // AddPlayers.
        terms["players.stats.library.session_mode"] = ("mode sesi", "session mode");
        // Memperbarui `terms[”players.stats.library.read_metric”]` menggunakan tuple yang membawa bagian 1: ”Cara membaca”; bagian 2: ”How to read” dalam
        // AddPlayers.
        terms["players.stats.library.read_metric"] = ("Cara membaca", "How to read");
        // Memperbarui `terms[”players.stats.library.context.title”]` menggunakan tuple yang membawa bagian 1: ”Konteks Sesi dan Hasil”; bagian 2: ”Session
        // Context and Outcome” dalam AddPlayers.
        terms["players.stats.library.context.title"] = ("Konteks Sesi dan Hasil", "Session Context and Outcome");
        // Memperbarui `terms[”players.stats.library.context.desc”]` menggunakan tuple yang membawa bagian 1: ”Identitas perhitungan, status akhir
        // permainan, dan catatan sistem.”; bagian 2: ”Calculation identity, final game status, and system notes.” dalam AddPlayers.
        terms["players.stats.library.context.desc"] = ("Identitas perhitungan, status akhir permainan, dan catatan sistem.", "Calculation identity, final game status, and system notes.");
        // Memperbarui `terms[”players.analysis.scorecard.title”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan angka utama”; bagian 2: ”Main numbers
        // summary” dalam AddPlayers.
        terms["players.analysis.scorecard.title"] = ("Ringkasan angka utama", "Main numbers summary");
        // Memperbarui `terms[”players.analysis.badge”]` menggunakan tuple yang membawa bagian 1: ”Penjelasan hasil”; bagian 2: ”Result explanation” dalam
        // AddPlayers.
        terms["players.analysis.badge"] = ("Penjelasan hasil", "Result explanation");
        // Memperbarui `terms[”players.analysis.title”]` menggunakan tuple yang membawa bagian 1: ”Cerita di Balik Hasil Pemain”; bagian 2: ”The Story
        // Behind the Player's Result” dalam AddPlayers.
        terms["players.analysis.title"] = ("Cerita di Balik Hasil Pemain", "The Story Behind the Player's Result");
        // Memperbarui `terms[”players.analysis.desc”]` menggunakan tuple yang membawa bagian 1: ”Baca analisis dari atas ke bawah. Buka rincian untuk
        // melihat sumber data, rumus...; bagian 2: ”Read the analysis from top to bottom. Open the details to see the data source, ... dalam AddPlayers.
        terms["players.analysis.desc"] = ("Baca analisis dari atas ke bawah. Buka rincian untuk melihat sumber data, rumus, dan hitungan pemain. Batas nilai hanya panduan umum, bukan aturan menang atau kalah.", "Read the analysis from top to bottom. Open the details to see the data source, formula, and player's calculation. Value ranges are general guidance, not win-or-lose rules.");
        // Memperbarui `terms[”players.analysis.method”]` menggunakan tuple yang membawa bagian 1: ”Rumus dan cara membaca”; bagian 2: ”Formula and
        // interpretation” dalam AddPlayers.
        terms["players.analysis.method"] = ("Rumus dan cara membaca", "Formula and interpretation");
        // Memperbarui `terms[”players.analysis.details”]` menggunakan tuple yang membawa bagian 1: ”Lihat sumber data dan cara menghitung”; bagian 2: ”View
        // data source and calculation” dalam AddPlayers.
        terms["players.analysis.details"] = ("Lihat sumber data dan cara menghitung", "View data source and calculation");
        // Memperbarui `terms[”players.analysis.reading”]` menggunakan tuple yang membawa bagian 1: ”Artinya:”; bagian 2: ”Meaning:” dalam AddPlayers.
        terms["players.analysis.reading"] = ("Artinya:", "Meaning:");
        // Memperbarui `terms[”players.analysis.chapter.money.title”]` menggunakan tuple yang membawa bagian 1: ”Uang dan usaha”; bagian 2: ”Money and
        // business” dalam AddPlayers.
        terms["players.analysis.chapter.money.title"] = ("Uang dan usaha", "Money and business");
        // Memperbarui `terms[”players.analysis.chapter.money.desc”]` menggunakan tuple yang membawa bagian 1: ”Lihat sumber pemasukan, pengeluaran, dan
        // hasil usaha pemain.”; bagian 2: ”See the player's income sources, spending, and business result.” dalam AddPlayers.
        terms["players.analysis.chapter.money.desc"] = ("Lihat sumber pemasukan, pengeluaran, dan hasil usaha pemain.", "See the player's income sources, spending, and business result.");
        // Memperbarui `terms[”players.analysis.chapter.future.title”]` menggunakan tuple yang membawa bagian 1: ”Keputusan untuk masa depan”; bagian 2:
        // ”Decisions for the future” dalam AddPlayers.
        terms["players.analysis.chapter.future.title"] = ("Keputusan untuk masa depan", "Decisions for the future");
        // Memperbarui `terms[”players.analysis.chapter.future.desc”]` menggunakan tuple yang membawa bagian 1: ”Lihat cara pemain mengelola risiko, utang,
        // dan target masa depan.”; bagian 2: ”See how the player managed risk, debt, and future goals.” dalam AddPlayers.
        terms["players.analysis.chapter.future.desc"] = ("Lihat cara pemain mengelola risiko, utang, dan target masa depan.", "See how the player managed risk, debt, and future goals.");
        // Memperbarui `terms[”players.analysis.chapter.play.title”]` menggunakan tuple yang membawa bagian 1: ”Cara pemain menggunakan giliran”; bagian 2:
        // ”How the player used each turn” dalam AddPlayers.
        terms["players.analysis.chapter.play.title"] = ("Cara pemain menggunakan giliran", "How the player used each turn");
        // Memperbarui `terms[”players.analysis.chapter.play.desc”]` menggunakan tuple yang membawa bagian 1: ”Lihat apakah aksi pemain menghasilkan uang
        // dan menyelesaikan pesanan.”; bagian 2: ”See whether the player's actions earned money and completed orders.” dalam AddPlayers.
        terms["players.analysis.chapter.play.desc"] = ("Lihat apakah aksi pemain menghasilkan uang dan menyelesaikan pesanan.", "See whether the player's actions earned money and completed orders.");
        // Memperbarui `terms[”players.analysis.chapter.wellbeing.title”]` menggunakan tuple yang membawa bagian 1: ”Pilihan yang membentuk kebahagiaan”;
        // bagian 2: ”Choices that shaped happiness” dalam AddPlayers.
        terms["players.analysis.chapter.wellbeing.title"] = ("Pilihan yang membentuk kebahagiaan", "Choices that shaped happiness");
        // Memperbarui `terms[”players.analysis.chapter.wellbeing.desc”]` menggunakan tuple yang membawa bagian 1: ”Lihat keseimbangan kebutuhan, donasi,
        // dan sumber Poin Kebahagiaan.”; bagian 2: ”See need balance, donations, and Happiness Point sources.” dalam AddPlayers.
        terms["players.analysis.chapter.wellbeing.desc"] = ("Lihat keseimbangan kebutuhan, donasi, dan sumber Poin Kebahagiaan.", "See need balance, donations, and Happiness Point sources.");
        // Memperbarui `terms[”players.analysis.net_worth.title”]` menggunakan tuple yang membawa bagian 1: ”Berapa koin tersisa dibandingkan koin awal?”;
        // bagian 2: ”How do remaining coins compare with starting coins?” dalam AddPlayers.
        terms["players.analysis.net_worth.title"] = ("Berapa persen koin naik atau turun dari awal?", "By what percentage have coins increased or decreased from the start?");
        // Memperbarui `terms[”players.analysis.net_worth.desc”]` menggunakan tuple yang membawa bagian 1: ”Membandingkan Koin Tersisa dengan Koin Awal.
        // Nilai 100% berarti sama dengan awa...; bagian 2: ”Compares Remaining Coins with Starting Coins. A value of 100% means unchanged, ... dalam
        // AddPlayers.
        terms["players.analysis.net_worth.desc"] = ("Menunjukkan perubahan Koin Tersisa dari Koin Awal. +20% berarti naik 20%, -20% berarti turun 20%, dan 0% berarti tetap. Tabungan, bahan, dan emas tidak dihitung.", "Shows the change in Remaining Coins from Starting Coins. +20% means a 20% increase, -20% means a 20% decrease, and 0% means unchanged. Savings, ingredients, and gold are excluded.");
        // Memperbarui `terms[”players.analysis.income_diversification.title”]` menggunakan tuple yang membawa bagian 1: ”Apakah pemasukan bergantung pada
        // satu sumber?”; bagian 2: ”Did income rely on one source?” dalam AddPlayers.
        terms["players.analysis.income_diversification.title"] = ("Apakah pemasukan bergantung pada satu sumber?", "Did income rely on one source?");
        // Memperbarui `terms[”players.analysis.income_diversification.desc”]` menggunakan tuple yang membawa bagian 1: ”Melihat apakah pemasukan tersebar
        // di beberapa sumber atau hanya bergantung pada...; bagian 2: ”Shows whether income is spread across several sources or depends on only one.” dalam
        // AddPlayers.
        terms["players.analysis.income_diversification.desc"] = ("Melihat apakah pemasukan tersebar di beberapa sumber atau hanya bergantung pada satu sumber.", "Shows whether income is spread across several sources or depends on only one.");
        // Memperbarui `terms[”players.analysis.expense_efficiency.title”]` menggunakan tuple yang membawa bagian 1: ”Berapa persen pengeluaran digunakan
        // untuk membeli bahan?”; bagian 2: ”What percentage of spending went to ingredient purchases?” dalam AddPlayers.
        terms["players.analysis.expense_efficiency.title"] = ("Berapa persen pengeluaran digunakan untuk membeli bahan?", "What percentage of spending went to ingredient purchases?");
        // Memperbarui `terms[”players.analysis.expense_efficiency.desc”]` menggunakan tuple yang membawa bagian 1: ”Membandingkan Total Biaya Pembelian
        // Bahan dengan Total Koin Keluar. Ini adalah ...; bagian 2: ”Compares Total Ingredient Purchase Cost with Total Outgoing Coins. This is the ...
        // dalam AddPlayers.
        terms["players.analysis.expense_efficiency.desc"] = ("Membandingkan Total Biaya Pembelian Bahan dengan Total Koin Keluar. Ini adalah bagian pengeluaran untuk membeli bahan, bukan persentase keuntungan.", "Compares Total Ingredient Purchase Cost with Total Outgoing Coins. This is the share spent on ingredient purchases, not a profit percentage.");
        // Memperbarui `terms[”players.analysis.business_margin.title”]` menggunakan tuple yang membawa bagian 1: ”Berapa persen pendapatan pesanan menjadi
        // laba?”; bagian 2: ”What percentage of order income became profit?” dalam AddPlayers.
        terms["players.analysis.business_margin.title"] = ("Berapa persen pendapatan pesanan menjadi laba?", "What percentage of order income became profit?");
        // Memperbarui `terms[”players.analysis.business_margin.desc”]` menggunakan tuple yang membawa bagian 1: ”Mengurangi Total Pendapatan Pesanan dengan
        // Biaya Bahan Terpakai, lalu membandin...; bagian 2: ”Subtracts Used Ingredient Cost from Total Order Income, then compares the diffe... dalam
        // AddPlayers.
        terms["players.analysis.business_margin.desc"] = ("Mengurangi Total Pendapatan Pesanan dengan Biaya Bahan Terpakai, lalu membandingkan selisihnya dengan pendapatan pesanan. Bahan yang belum dipakai tidak termasuk.", "Subtracts Used Ingredient Cost from Total Order Income, then compares the difference with order income. Unused ingredients are excluded.");
        // Memperbarui `terms[”players.analysis.risk_appetite.title”]` menggunakan tuple yang membawa bagian 1: ”Berapa persen risiko selesai tanpa tindakan
        // darurat?”; bagian 2: ”What percentage of risks were resolved without emergency action?” dalam AddPlayers.
        terms["players.analysis.risk_appetite.title"] = ("Berapa persen risiko selesai tanpa tindakan darurat?", "What percentage of risks were resolved without emergency action?");
        // Memperbarui `terms[”players.analysis.risk_appetite.desc”]` menggunakan tuple yang membawa bagian 1: ”Membandingkan Risiko Selesai tanpa Tindakan
        // Darurat dengan Kartu Risiko Kehidup...; bagian 2: ”Compares Risks Resolved without Emergency Action with Life Risk Cards Drawn.” dalam
        // AddPlayers.
        terms["players.analysis.risk_appetite.desc"] = ("Membandingkan Risiko Selesai tanpa Tindakan Darurat dengan Kartu Risiko Kehidupan yang Muncul.", "Compares Risks Resolved without Emergency Action with Life Risk Cards Drawn.");
        // Memperbarui `terms[”players.analysis.debt_discipline.title”]` menggunakan tuple yang membawa bagian 1: ”Berapa porsi pinjaman yang belum lunas?”;
        // bagian 2: ”What share is made up of the outstanding loan?” dalam AddPlayers.
        terms["players.analysis.debt_discipline.title"] = ("Berapa porsi pinjaman yang belum lunas?", "What share is made up of the outstanding loan?");
        // Memperbarui `terms[”players.analysis.debt_discipline.desc”]` menggunakan tuple yang membawa bagian 1: ”Membandingkan Sisa Pinjaman dengan
        // gabungan Sisa Pinjaman dan Total Koin Tersis...; bagian 2: ”Compares Outstanding Loan with Outstanding Loan plus Remaining Coins and Saving...
        // dalam AddPlayers.
        terms["players.analysis.debt_discipline.desc"] = ("Membandingkan Sisa Pinjaman dengan gabungan Sisa Pinjaman dan Total Koin Tersisa dan Tabungan. Ini bukan perbandingan pinjaman dengan koin saja.", "Compares Outstanding Loan with Outstanding Loan plus Remaining Coins and Savings Total. This is not a loan-to-cash-only ratio.");
        // Memperbarui `terms[”players.analysis.goal_ambition.title”]` menggunakan tuple yang membawa bagian 1: ”Seberapa jauh target finansial sudah
        // didanai?”; bagian 2: ”How much of the financial goal cost has been funded?” dalam AddPlayers.
        terms["players.analysis.goal_ambition.title"] = ("Berapa target finansial yang berhasil dibeli?", "How many financial goals have been purchased?");
        // Memperbarui `terms[”players.analysis.goal_ambition.desc”]` menggunakan tuple yang membawa bagian 1: ”Membandingkan dana yang diperhitungkan untuk
        // target dengan total biaya target y...; bagian 2: ”Compares funds counted toward goals with the total cost of attempted goals. Sav... dalam
        // AddPlayers.
        terms["players.analysis.goal_ambition.desc"] = ("Saat tabungan mencapai harga target, koin dibayarkan ke bank dan pemain memperoleh kartu target. Lihat jumlah target yang sudah dibeli, biaya pembeliannya, serta tabungan untuk target yang belum dibeli.", "Once savings reach a goal's price, coins are paid to the bank and the player receives the goal card. See purchased goals, their purchase costs, and savings for goals still to be purchased.");
        terms["players.analysis.goal_purchases.result"] = ("Total biaya pembelian: {0} koin. Ini adalah koin yang sudah dibayarkan untuk memperoleh kartu target.", "Total purchase cost: {0} coins. These coins have already been paid to obtain goal cards.");
        terms["players.analysis.goal_purchases.unpaid_loan"] = ("Target ini sudah dibeli. Namun, poin kebahagiaan target saat ini tidak dihitung karena masih ada pinjaman yang belum lunas. Jika pinjaman tetap belum lunas saat sesi berakhir, poin kebahagiaan target hangus sesuai aturan.", "These goals were purchased. However, their Happiness Points currently do not count because a loan remains unpaid. If the loan is still unpaid when the session ends, goal Happiness Points are forfeited under the rules.");
        terms["players.support.formula.goal_purchases"] = ("Target Finansial Berhasil Dibeli = jumlah kartu target yang pembeliannya sudah selesai. Total Biaya Pembelian Target Finansial = jumlah biaya pembelian kartu-kartu tersebut. Tabungan untuk Target Belum Dibeli = sisa setoran pada target yang pembeliannya belum selesai.", "Financial Goals Purchased = number of goal cards whose purchase is complete. Total Financial Goal Purchase Cost = sum of the purchase costs of those cards. Savings for Unpurchased Goals = remaining deposits for goals whose purchase is not complete.");
        // Memperbarui `terms[”players.analysis.action_efficiency.title”]` menggunakan tuple yang membawa bagian 1: ”Berapa persen aksi utama menghasilkan
        // koin?”; bagian 2: ”What percentage of main actions earned coins?” dalam AddPlayers.
        terms["players.analysis.action_efficiency.title"] = ("Berapa persen aksi dipakai untuk mendapatkan koin?", "What percentage of actions were used to earn coins?");
        // Memperbarui `terms[”players.analysis.action_efficiency.desc”]` menggunakan tuple yang membawa bagian 1: ”Membandingkan Jumlah Aksi Utama
        // Penghasil Koin dengan Total Aksi Utama. Aktivit...; bagian 2: ”Compares Income-Earning Main Action Count with Total Main Actions. Activities t...
        // dalam AddPlayers.
        terms["players.analysis.action_efficiency.desc"] = ("Setiap kegiatan yang memakai jatah pemain dihitung sebagai satu aksi. Contoh: membeli bahan lalu menjual masakan dihitung sebagai dua aksi, dan satu di antaranya menghasilkan koin masuk. Persentase ini dihitung dari seluruh aksi yang dilakukan selama sesi.", "Each activity that uses the player's allowance counts as one action. For example, buying ingredients then selling a meal counts as two actions, and one of them brings in coins. This percentage uses all actions taken during the session.");
        // Memperbarui `terms[”players.analysis.meal_success.title”]` menggunakan tuple yang membawa bagian 1: ”Berapa persen bahan sudah digunakan untuk
        // pesanan?”; bagian 2: ”What percentage of ingredients were used for orders?” dalam AddPlayers.
        terms["players.analysis.meal_success.title"] = ("Berapa persen bahan sudah digunakan untuk pesanan?", "What percentage of ingredients were used for orders?");
        // Memperbarui `terms[”players.analysis.meal_success.desc”]` menggunakan tuple yang membawa bagian 1: ”Membandingkan Total Bahan Digunakan pada
        // pesanan selesai dengan Bahan Terkumpul...; bagian 2: ”Compares Total Ingredients Used in completed orders with Ingredients Collected.... dalam
        // AddPlayers.
        terms["players.analysis.meal_success.desc"] = ("Membandingkan Total Bahan Digunakan pada pesanan selesai dengan Bahan Terkumpul. Nilai ini bukan tingkat keberhasilan pesanan.", "Compares Total Ingredients Used in completed orders with Ingredients Collected. This is not an order success rate.");
        // Memperbarui `terms[”players.analysis.planning_horizon.title”]` menggunakan tuple yang membawa bagian 1: ”Berapa persen aksi utama dipakai untuk
        // jangka panjang?”; bagian 2: ”What percentage of main actions supported long-term goals?” dalam AddPlayers.
        terms["players.analysis.planning_horizon.title"] = ("Berapa persen aksi dipakai untuk tabungan, target, asuransi, dan pelunasan?", "What percentage of actions were used for savings, goals, insurance, and repayment?");
        // Memperbarui `terms[”players.analysis.planning_horizon.desc”]` menggunakan tuple yang membawa bagian 1: ”Menghitung bagian Total Aksi Utama yang
        // digunakan untuk menabung, target finans...; bagian 2: ”Measures the share of Total Main Actions used for savings, financial goals, ins... dalam
        // AddPlayers.
        terms["players.analysis.planning_horizon.desc"] = ("Aksi menabung dan membeli target digabung karena pembelian target dari tabungan merupakan hasil proses menabung. Saat dana cukup, pemain membayar dan memperoleh kartu target tanpa memakai jatah aksi tambahan. Aksi mengaktifkan asuransi dan melunasi pinjaman ditampilkan terpisah; perlindungan awal gratis dan klaim asuransi tidak menambah jumlah aksi.", "Saving and goal purchase actions are grouped because a goal purchase from savings completes the saving process. Once funds are sufficient, the player pays and receives the goal card without using an additional action. Insurance activation and loan repayment actions are shown separately; free initial coverage and insurance claims do not add to the action count.");
        // Memperbarui `terms[”players.analysis.fulfillment_diversity.title”]` menggunakan tuple yang membawa bagian 1: ”Seberapa merata kartu kebutuhan
        // yang dimiliki?”; bagian 2: ”How evenly are owned need cards distributed?” dalam AddPlayers.
        terms["players.analysis.fulfillment_diversity.title"] = ("Seberapa merata kartu kebutuhan yang dimiliki?", "How evenly are owned need cards distributed?");
        // Memperbarui `terms[”players.analysis.fulfillment_diversity.desc”]` menggunakan tuple yang membawa bagian 1: ”Membandingkan proporsi kartu
        // kebutuhan primer, sekunder, dan tersier yang masih...; bagian 2: ”Compares the shares of primary, secondary, and tertiary need cards still
        // owned.... dalam AddPlayers.
        terms["players.analysis.fulfillment_diversity.desc"] = ("Membandingkan proporsi kartu kebutuhan primer, sekunder, dan tersier yang masih dimiliki. Nilai ini tidak menyatakan misi koleksi sudah selesai.", "Compares the shares of primary, secondary, and tertiary need cards still owned. This does not indicate that the collection mission is complete.");
        // Memperbarui `terms[”players.analysis.donation_commitment.title”]` menggunakan tuple yang membawa bagian 1: ”Bagaimana keteraturan, porsi, dan
        // keikutsertaan donasi pemain?”; bagian 2: ”How regular, substantial, and frequent were the player's donations?” dalam AddPlayers.
        terms["players.analysis.donation_commitment.title"] = ("Bagaimana keteraturan, porsi, dan keikutsertaan donasi pemain?", "How regular, substantial, and frequent were the player's donations?");
        // Memperbarui `terms[”players.analysis.donation_commitment.desc”]` menggunakan tuple yang membawa bagian 1: ”Menggabungkan Keteraturan Jumlah
        // Donasi, Porsi Donasi dari Koin Tersisa dan Don...; bagian 2: ”Combines Donation Amount Regularity, Donation Share of Remaining and Donated Co...
        // dalam AddPlayers.
        terms["players.analysis.donation_commitment.desc"] = ("Menggabungkan Keteraturan Jumlah Donasi, Porsi Donasi dari Koin Tersisa dan Donasi, serta Persentase Jumat dengan Donasi menjadi satu skor.", "Combines Donation Amount Regularity, Donation Share of Remaining and Donated Coins, and Share of Fridays with Donations into one score.");
        // Memperbarui `terms[”players.analysis.happiness_portfolio.title”]` menggunakan tuple yang membawa bagian 1: ”Dari mana Poin Kebahagiaan berasal?”;
        // bagian 2: ”Where did Happiness Points come from?” dalam AddPlayers.
        terms["players.analysis.happiness_portfolio.title"] = ("Seberapa merata sumber Poin Kebahagiaan?", "How evenly are Happiness Points spread across sources?");
        // Memperbarui `terms[”players.analysis.happiness_portfolio.desc”]` menggunakan tuple yang membawa bagian 1: ”Menunjukkan poin kebahagiaan dari
        // kebutuhan, donasi, emas, pensiun, dan target,...; bagian 2: ”Shows happiness points from needs, donations, gold, pension, and goals, includi...
        // dalam AddPlayers.
        terms["players.analysis.happiness_portfolio.desc"] = ("0% berarti poin kebahagiaan positif hanya berasal dari satu sumber; 100% berarti terbagi sama rata pada enam sumber: kartu kebutuhan, bonus set, donasi, emas, pensiun, dan target finansial. Penalti tidak dihitung sebagai sumber poin kebahagiaan.", "0% means positive Happiness points come from just one source; 100% means they are evenly spread across six sources: need cards, set bonuses, donations, gold, pension, and financial goals. Penalties are not Happiness point sources.");
        // Memperbarui `terms[”players.analysis.happiness_portfolio.beginner.desc”]` menggunakan tuple yang membawa bagian 1: ”Menunjukkan poin kebahagiaan
        // dari kebutuhan, donasi, emas, dan pensiun, termasu...; bagian 2: ”Shows happiness points from needs, donations, gold, and pension, including
        // coll... dalam AddPlayers.
        terms["players.analysis.happiness_portfolio.beginner.desc"] = ("0% berarti poin kebahagiaan positif hanya berasal dari satu sumber; 100% berarti terbagi sama rata pada lima sumber: kartu kebutuhan, bonus set, donasi, emas, dan pensiun. Penalti tidak dihitung sebagai sumber poin kebahagiaan.", "0% means positive Happiness points come from just one source; 100% means they are evenly spread across five sources: need cards, set bonuses, donations, gold, and pension. Penalties are not Happiness point sources.");
        // Memperbarui `terms[”players.analysis.source.net-worth”]` menggunakan tuple yang membawa bagian 1: ”Koin Awal berasal dari set aturan sesi. Koin
        // Tersisa dihitung dari Koin Awal + ...; bagian 2: ”Starting Coins comes from the session ruleset. Remaining Coins equals Starting ... dalam
        // AddPlayers.
        terms["players.analysis.source.net-worth"] = ("Koin Awal berasal dari set aturan sesi. Koin Tersisa dihitung dari Koin Awal + Total Koin Masuk − Total Koin Keluar; tabungan dan nilai kartu tidak ditambahkan.", "Starting Coins comes from the session ruleset. Remaining Coins equals Starting Coins + Total Incoming Coins − Total Outgoing Coins; savings and card values are not added.");
        // Memperbarui `terms[”players.analysis.source.income-diversification”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan Kerja Lepas, Total
        // Pendapatan Pesanan, dan Pendapatan Penjualan Emas...; bagian 2: ”Freelance Income, Total Order Income, and Gold Sale Income come from their
        // reco... dalam AddPlayers.
        terms["players.analysis.source.income-diversification"] = ("Pendapatan Kerja Lepas, Total Pendapatan Pesanan, dan Pendapatan Penjualan Emas berasal dari aktivitas dan transaksi terkait. Persentase Pendapatan per Sumber serta Jumlah Sumber Pendapatan Aktif dihitung dari sumber yang memiliki pemasukan. Pinjaman, penarikan tabungan, dan donasi tidak dihitung.", "Freelance Income, Total Order Income, and Gold Sale Income come from their recorded activities and transactions. Income Share by Source and Active Income Source Count use sources with positive income. Loans, savings withdrawals, and donations are excluded.");
        // Memperbarui `terms[”players.analysis.source.expense-efficiency”]` menggunakan tuple yang membawa bagian 1: ”Total Biaya Pembelian Bahan adalah
        // seluruh koin untuk pembelian bahan, termasuk...; bagian 2: ”Total Ingredient Purchase Cost includes all coins spent on ingredient purchases...
        // dalam AddPlayers.
        terms["players.analysis.source.expense-efficiency"] = ("Total Biaya Pembelian Bahan adalah seluruh koin untuk pembelian bahan, termasuk bahan yang belum dipakai. Total Koin Keluar adalah jumlah seluruh transaksi koin keluar pemain.", "Total Ingredient Purchase Cost includes all coins spent on ingredient purchases, including unused ingredients. Total Outgoing Coins is the sum of all the player's outgoing coin transactions.");
        // Memperbarui `terms[”players.analysis.source.business-margin”]` menggunakan tuple yang membawa bagian 1: ”Total Pendapatan Pesanan berasal dari
        // pesanan selesai. Biaya Bahan Terpakai ber...; bagian 2: ”Total Order Income comes from completed orders. Used Ingredient Cost is the cos... dalam
        // AddPlayers.
        terms["players.analysis.source.business-margin"] = ("Total Pendapatan Pesanan berasal dari pesanan selesai. Biaya Bahan Terpakai berasal dari biaya bahan yang benar-benar dipakai pada pesanan tersebut, bukan seluruh pembelian bahan.", "Total Order Income comes from completed orders. Used Ingredient Cost is the cost of ingredients actually consumed by those orders, not all ingredient purchases.");
        // Memperbarui `terms[”players.analysis.source.risk-appetite”]` menggunakan tuple yang membawa bagian 1: ”Kartu Risiko Kehidupan yang Muncul berasal
        // dari catatan risiko mode Mahir. Risi...; bagian 2: ”Life Risk Cards Drawn comes from Advanced-mode risk records. Risks Resolved wit... dalam
        // AddPlayers.
        terms["players.analysis.source.risk-appetite"] = ("Kartu Risiko Kehidupan yang Muncul berasal dari catatan risiko mode Mahir. Risiko Selesai tanpa Tindakan Darurat mencakup risiko yang sudah dibayar, ditanggung asuransi, atau tidak membutuhkan pembayaran koin, tanpa memakai Tindakan Darurat. Risiko yang masih menunggu penyelesaian belum masuk hitungan ini.", "Life Risk Cards Drawn comes from Advanced-mode risk records. Risks Resolved without Emergency Action includes risks already paid, covered by insurance, or requiring no coin payment, without using an Emergency Action. Risks still awaiting resolution are not yet included in this count.");
        // Memperbarui `terms[”players.analysis.source.debt-discipline”]` menggunakan tuple yang membawa bagian 1: ”Sisa Pinjaman berasal dari jumlah
        // pinjaman dikurangi pembayaran. Total Koin Ter...; bagian 2: ”Outstanding Loan is the borrowed amount minus repayments. Remaining Coins and S...
        // dalam AddPlayers.
        terms["players.analysis.source.debt-discipline"] = ("Sisa Pinjaman berasal dari jumlah pinjaman dikurangi pembayaran. Total Koin Tersisa dan Tabungan adalah Koin Tersisa ditambah Koin dalam Tabungan; setiap komponen bernilai negatif dihitung sebagai nol.", "Outstanding Loan is the borrowed amount minus repayments. Remaining Coins and Savings Total adds Remaining Coins and Coins in Savings, treating each negative component as zero.");
        // Memperbarui `terms[”players.analysis.source.goal-ambition”]` menggunakan tuple yang membawa bagian 1: ”Dana yang Diperhitungkan untuk Target
        // berasal dari koin yang dialokasikan ke ta...; bagian 2: ”Funds Counted Toward Goals come from coins allocated to goals and are capped at... dalam
        // AddPlayers.
        terms["players.analysis.source.goal-ambition"] = ("Target Finansial Berhasil Dibeli dan Total Biaya Pembelian Target Finansial berasal dari catatan pembelian kartu target. Tabungan untuk Target Belum Dibeli berasal dari setoran yang masih tersimpan untuk target yang belum selesai. Biaya target yang sudah dibeli tetap tercatat meskipun saldo tabungannya sudah menjadi nol.", "Financial Goals Purchased and Total Financial Goal Purchase Cost come from completed goal card purchases. Savings for Unpurchased Goals come from deposits still held for unfinished goals. Purchased goal costs remain recorded even when their savings balance has reached zero.");
        // Memperbarui `terms[”players.analysis.source.action-efficiency”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Utama Penghasil Koin dan
        // Total Aksi Utama dihitung dari aktivitas p...; bagian 2: ”Income-Earning Main Action Count and Total Main Actions come from player activi...
        // dalam AddPlayers.
        terms["players.analysis.source.action-efficiency"] = ("Aksi untuk Mendapatkan Koin menghitung kerja lepas dan penjualan masakan yang menghasilkan pemasukan. Total Aksi yang Digunakan menjumlahkan seluruh kegiatan yang memakai jatah aksi sepanjang sesi. Pinjaman dan penarikan tabungan tetap memakai jatah aksi, tetapi koinnya tidak dihitung sebagai pendapatan baru. Penjualan emas menghasilkan pemasukan di luar jatah aksi. Setiap kegiatan yang memakai jatah aksi dihitung satu kali, berapa pun koinnya.", "Actions Used to Earn Coins counts freelance work and meal sales that generate income. Total Actions Used sums all activities that use the action allowance throughout the session. Loans and savings withdrawals still use the action allowance, but their coins are not counted as new income. Gold sales generate income outside the action allowance. Each activity that uses the allowance is counted once, regardless of its coin amount.");
        // Memperbarui `terms[”players.analysis.source.meal-success”]` menggunakan tuple yang membawa bagian 1: ”Bahan Terkumpul berasal dari pembagian awal
        // dan bahan yang diperoleh selama per...; bagian 2: ”Ingredients Collected comes from initial distribution and ingredients acquired ... dalam
        // AddPlayers.
        terms["players.analysis.source.meal-success"] = ("Bahan Terkumpul berasal dari pembagian awal dan bahan yang diperoleh selama permainan. Total Bahan Digunakan adalah jumlah kartu bahan yang dipakai pada pesanan selesai.", "Ingredients Collected comes from initial distribution and ingredients acquired during the game. Total Ingredients Used counts ingredient cards consumed by completed orders.");
        // Memperbarui `terms[”players.analysis.source.planning-horizon”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Menabung, Jumlah Aksi
        // Target Finansial, Jumlah Aksi Asuransi, dan J...; bagian 2: ”Savings Action Count, Financial Goal Action Count, Insurance Action Count, and ...
        // dalam AddPlayers.
        terms["players.analysis.source.planning-horizon"] = ("Aksi untuk Tabungan dan Pembelian Target menggabungkan kegiatan menabung serta pembelian target yang memakai jatah aksi. Pembelian otomatis dari tabungan tetap tercatat sebagai target berhasil dibeli; aksinya sudah dihitung saat menabung. Aksi untuk Mengaktifkan Asuransi hanya menghitung pembayaran premi. Perlindungan awal gratis dan pemakaian asuransi saat risiko tidak memakai jatah aksi tambahan. Aksi untuk Pelunasan Pinjaman menghitung pembayaran pinjaman yang memakai jatah aksi. Total Aksi yang Digunakan mencakup seluruh kegiatan yang memakai jatah aksi sepanjang sesi.", "Actions for Savings and Goal Purchases combines saving activities and goal purchases that use the action allowance. Automatic purchases from savings still count as purchased goals; their actions have already been counted when saving. Actions for Insurance Activation counts only premium payments. Free initial coverage and insurance claims use no additional actions. Actions for Loan Repayment counts repayments that use the action allowance. Total Actions Used covers all activities that use the action allowance throughout the session.");
        // Memperbarui `terms[”players.analysis.source.fulfillment-diversity”]` menggunakan tuple yang membawa bagian 1: ”Persentase Kartu Kebutuhan Primer,
        // Persentase Kartu Kebutuhan Sekunder, dan Per...; bagian 2: ”Primary Need Card Share, Secondary Need Card Share, and Tertiary Need Card Shar...
        // dalam AddPlayers.
        terms["players.analysis.source.fulfillment-diversity"] = ("Persentase Kartu Kebutuhan Primer, Persentase Kartu Kebutuhan Sekunder, dan Persentase Kartu Kebutuhan Tersier dihitung dari jumlah kartu tiap kategori dibandingkan seluruh kartu kebutuhan yang masih dimiliki.", "Primary Need Card Share, Secondary Need Card Share, and Tertiary Need Card Share compare each category's count with all need cards still owned.");
        // Memperbarui `terms[”players.analysis.source.donation-commitment”]` menggunakan tuple yang membawa bagian 1: ”Keteraturan Jumlah Donasi berasal
        // dari variasi nominal donasi. Porsi Donasi dar...; bagian 2: ”Donation Amount Regularity comes from variation in donation amounts. Donation S...
        // dalam AddPlayers.
        terms["players.analysis.source.donation-commitment"] = ("Keteraturan Jumlah Donasi berasal dari variasi nominal donasi. Porsi Donasi dari Koin Tersisa dan Donasi = Total Koin Donasi ÷ (Koin Tersisa yang minimal nol + Total Koin Donasi). Persentase Jumat dengan Donasi membandingkan Jumat saat pemain berdonasi dengan Jumat yang tersedia.", "Donation Amount Regularity comes from variation in donation amounts. Donation Share of Remaining and Donated Coins = Total Donated Coins ÷ (Remaining Coins, floored at zero, + Total Donated Coins). Share of Fridays with Donations compares Fridays with a donation against available Fridays.");
        // Memperbarui `terms[”players.analysis.source.happiness-portfolio”]` menggunakan tuple yang membawa bagian 1: ”Rincian skor akhir: kartu dan bonus
        // set kebutuhan dari kepemilikan akhir; poin ...; bagian 2: ”Final-score breakdown: need-card and set-bonus happiness points from ending own...
        // dalam AddPlayers.
        terms["players.analysis.source.happiness-portfolio"] = ("Poin kebahagiaan dari kartu dan bonus set kebutuhan dihitung dari kartu yang masih dimiliki; poin kebahagiaan donasi dan pensiun berdasarkan peringkat; poin kebahagiaan emas berdasarkan jumlah kartu emas yang masih dimiliki. Poin kebahagiaan target berasal dari kartu target yang sudah dibeli, bukan saldo tabungan. Jika masih ada pinjaman belum lunas, poin kebahagiaan target tidak dihitung meskipun pembeliannya tetap tercatat. Penalti misi dan pinjaman mengurangi total poin kebahagiaan, tetapi tidak masuk rumus pemerataan sumber.", "Happiness points from need cards and set bonuses use cards still owned; donation and pension Happiness Points use rankings; gold Happiness Points use the number of gold cards still held. Goal Happiness Points come from purchased goal cards, not the savings balance. If a loan remains unpaid, goal Happiness Points are excluded even though the purchase remains recorded. Mission and loan penalties reduce total Happiness Points but are excluded from the source-balance formula.");
        // Memperbarui `terms[”players.analysis.source.happiness-portfolio.beginner”]` menggunakan tuple yang membawa bagian 1: ”Rincian skor akhir: kartu
        // dan bonus set kebutuhan dari kepemilikan akhir; poin ...; bagian 2: ”Final-score breakdown: need-card and set-bonus happiness points from ending
        // own... dalam AddPlayers.
        terms["players.analysis.source.happiness-portfolio.beginner"] = ("Poin kebahagiaan dari kartu dan bonus set kebutuhan dihitung dari kartu yang masih dimiliki; poin kebahagiaan donasi dan pensiun berdasarkan peringkat; poin kebahagiaan emas berdasarkan jumlah kartu emas yang masih dimiliki. Penalti misi koleksi mengurangi total poin kebahagiaan, tetapi tidak masuk rumus pemerataan sumber.", "Happiness points from need cards and set bonuses use cards still owned; donation and pension Happiness Points use rankings; gold Happiness Points use the number of gold cards still held. Collection-mission penalties reduce total Happiness Points but are excluded from the source-balance formula.");
        // Memperbarui `terms[”players.evidence.badge”]` menggunakan tuple yang membawa bagian 1: ”Data permainan”; bagian 2: ”Gameplay data” dalam
        // AddPlayers.
        terms["players.evidence.badge"] = ("Data permainan", "Gameplay data");
        // Memperbarui `terms[”players.evidence.title”]` menggunakan tuple yang membawa bagian 1: ”Data Permainan Lengkap”; bagian 2: ”Complete Gameplay
        // Data” dalam AddPlayers.
        terms["players.evidence.title"] = ("Data Permainan Lengkap", "Complete Gameplay Data");
        // Memperbarui `terms[”players.evidence.desc”]` menggunakan tuple yang membawa bagian 1: ”Buka kelompok yang ingin diperiksa. Semua data di bagian
        // ini dipakai untuk menj...; bagian 2: ”Open the group you want to inspect. All data here helps explain the analysis ab... dalam AddPlayers.
        terms["players.evidence.desc"] = ("Buka kelompok yang ingin diperiksa. Semua data di bagian ini dipakai untuk menjelaskan hasil analisis di atas.", "Open the group you want to inspect. All data here helps explain the analysis above.");
        // Memperbarui `terms[”players.evidence.group_suffix”]` menggunakan tuple yang membawa bagian 1: ”kelompok data”; bagian 2: ”data groups” dalam
        // AddPlayers.
        terms["players.evidence.group_suffix"] = ("kelompok data", "data groups");
        // Memperbarui `terms[”players.stats.focus.money.title”]` menggunakan tuple yang membawa bagian 1: ”Kondisi uang”; bagian 2: ”Money condition” dalam
        // AddPlayers.
        terms["players.stats.focus.money.title"] = ("Kondisi uang", "Money condition");
        // Memperbarui `terms[”players.stats.focus.money.question”]` menggunakan tuple yang membawa bagian 1: ”Apakah uang pemain bertambah atau
        // berkurang?”; bagian 2: ”Did the player's money grow or shrink?” dalam AddPlayers.
        terms["players.stats.focus.money.question"] = ("Apakah uang pemain bertambah atau berkurang?", "Did the player's money grow or shrink?");
        // Memperbarui `terms[”players.stats.focus.money.positive”]` menggunakan tuple yang membawa bagian 1: ”Pemasukan lebih besar daripada pengeluaran,
        // sehingga kondisi uang pemain bertum...; bagian 2: ”Income was higher than spending, so the player's money grew.” dalam AddPlayers.
        terms["players.stats.focus.money.positive"] = ("Pemasukan lebih besar daripada pengeluaran, sehingga kondisi uang pemain bertumbuh.", "Income was higher than spending, so the player's money grew.");
        // Memperbarui `terms[”players.stats.focus.money.positive.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Pertahankan surplus sambil
        // memenuhi kebutuhan, kewajiban, dan cadangan kas; sis...; bagian 2: ”Maintain the surplus while covering needs, obligations, and cash reserves;
        // the ... dalam AddPlayers.
        terms["players.stats.focus.money.positive.recommendation"] = ("Pertahankan surplus sambil memenuhi kebutuhan, kewajiban, dan cadangan kas; sisanya dapat diarahkan ke tabungan atau tujuan jangka panjang.", "Maintain the surplus while covering needs, obligations, and cash reserves; the remainder can support savings or long-term goals.");
        // Memperbarui `terms[”players.stats.focus.money.negative”]` menggunakan tuple yang membawa bagian 1: ”Pengeluaran lebih besar daripada pemasukan,
        // sehingga kondisi uang pemain menyus...; bagian 2: ”Spending was higher than income, so the player's money shrank.” dalam AddPlayers.
        terms["players.stats.focus.money.negative"] = ("Pengeluaran lebih besar daripada pemasukan, sehingga kondisi uang pemain menyusut.", "Spending was higher than income, so the player's money shrank.");
        // Memperbarui `terms[”players.stats.focus.money.negative.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Turunkan pengeluaran
        // nonprioritas dan tingkatkan pemasukan sampai arus uang pos...; bagian 2: ”Reduce nonessential spending and increase income until cash flow is
        // positive wi... dalam AddPlayers.
        terms["players.stats.focus.money.negative.recommendation"] = ("Turunkan pengeluaran nonprioritas dan tingkatkan pemasukan sampai arus uang positif tanpa menambah utang untuk kebutuhan rutin.", "Reduce nonessential spending and increase income until cash flow is positive without adding debt for routine needs.");
        // Memperbarui `terms[”players.stats.focus.money.balanced”]` menggunakan tuple yang membawa bagian 1: ”Pemasukan dan pengeluaran seimbang; aktivitas
        // selama permainan tidak menambah s...; bagian 2: ”Income and spending were balanced; gameplay activity did not increase the balan... dalam
        // AddPlayers.
        terms["players.stats.focus.money.balanced"] = ("Pemasukan dan pengeluaran seimbang; aktivitas selama permainan tidak menambah saldo.", "Income and spending were balanced; gameplay activity did not increase the balance.");
        // Memperbarui `terms[”players.stats.focus.money.balanced.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Bangun selisih positif secara
        // konsisten agar tersedia cadangan untuk risiko, ta...; bagian 2: ”Build a consistent positive margin so reserves are available for risks,
        // savings... dalam AddPlayers.
        terms["players.stats.focus.money.balanced.recommendation"] = ("Bangun selisih positif secara konsisten agar tersedia cadangan untuk risiko, tabungan, dan tujuan masa depan.", "Build a consistent positive margin so reserves are available for risks, savings, and future goals.");
        // Memperbarui `terms[”players.stats.focus.behavior.title”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan Kartu Kebutuhan”; bagian 2: ”Need
        // Card Balance” dalam AddPlayers.
        terms["players.stats.focus.behavior.title"] = ("Pemerataan Kartu Kebutuhan", "Need Card Balance");
        // Memperbarui `terms[”players.stats.focus.behavior.question”]` menggunakan tuple yang membawa bagian 1: ”Seberapa seimbang kebutuhan primer,
        // sekunder, dan tersier yang dimiliki pemain?...; bagian 2: ”How balanced are the player's primary, secondary, and tertiary needs?” dalam
        // AddPlayers.
        terms["players.stats.focus.behavior.question"] = ("Seberapa seimbang kebutuhan primer, sekunder, dan tersier yang dimiliki pemain?", "How balanced are the player's primary, secondary, and tertiary needs?");
        // Memperbarui `terms[”players.stats.focus.behavior.balanced”]` menggunakan tuple yang membawa bagian 1: ”Komposisi kebutuhan pemain seimbang di
        // antara tiga kategori.”; bagian 2: ”The player's needs are balanced across all three categories.” dalam AddPlayers.
        terms["players.stats.focus.behavior.balanced"] = ("Komposisi kebutuhan pemain seimbang di antara tiga kategori.", "The player's needs are balanced across all three categories.");
        // Memperbarui `terms[”players.stats.focus.behavior.balanced.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Pertahankan keseimbangan
        // sambil mencocokkan pembelian dengan misi koleksi dan k...; bagian 2: ”Maintain the balance while matching purchases to the collection mission and
        // ava... dalam AddPlayers.
        terms["players.stats.focus.behavior.balanced.recommendation"] = ("Pertahankan keseimbangan sambil mencocokkan pembelian dengan misi koleksi dan kemampuan kas.", "Maintain the balance while matching purchases to the collection mission and available cash.");
        // Memperbarui `terms[”players.stats.focus.behavior.partial”]` menggunakan tuple yang membawa bagian 1: ”Pemain sudah memenuhi lebih dari satu
        // kategori, tetapi komposisinya belum sepen...; bagian 2: ”The player covered more than one category, but the composition is not fully bal... dalam
        // AddPlayers.
        terms["players.stats.focus.behavior.partial"] = ("Pemain sudah memenuhi lebih dari satu kategori, tetapi komposisinya belum sepenuhnya seimbang.", "The player covered more than one category, but the composition is not fully balanced.");
        // Memperbarui `terms[”players.stats.focus.behavior.partial.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Lengkapi kategori yang
        // paling sedikit tanpa mengabaikan target misi koleksi.”; bagian 2: ”Build up the least represented category without ignoring the collection
        // mission... dalam AddPlayers.
        terms["players.stats.focus.behavior.partial.recommendation"] = ("Lengkapi kategori yang paling sedikit tanpa mengabaikan target misi koleksi.", "Build up the least represented category without ignoring the collection mission target.");
        // Memperbarui `terms[”players.stats.focus.behavior.skewed”]` menggunakan tuple yang membawa bagian 1: ”Kepemilikan kebutuhan masih terkonsentrasi
        // pada satu kategori.”; bagian 2: ”Need ownership is still concentrated in one category.” dalam AddPlayers.
        terms["players.stats.focus.behavior.skewed"] = ("Kepemilikan kebutuhan masih terkonsentrasi pada satu kategori.", "Need ownership is still concentrated in one category.");
        // Memperbarui `terms[”players.stats.focus.behavior.skewed.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Tambahkan kategori kebutuhan
        // yang belum terwakili agar profil pemenuhan lebih s...; bagian 2: ”Add missing need categories to create a more balanced fulfillment profile.”
        // dalam AddPlayers.
        terms["players.stats.focus.behavior.skewed.recommendation"] = ("Tambahkan kategori kebutuhan yang belum terwakili agar profil pemenuhan lebih seimbang.", "Add missing need categories to create a more balanced fulfillment profile.");
        // Memperbarui `terms[”players.stats.focus.happiness.title”]` menggunakan tuple yang membawa bagian 1: ”Pembentuk Poin Kebahagiaan”; bagian 2:
        // ”Happiness Score composition” dalam AddPlayers.
        terms["players.stats.focus.happiness.title"] = ("Pembentuk Poin Kebahagiaan", "Happiness Score composition");
        // Memperbarui `terms[”players.stats.focus.happiness.question”]` menggunakan tuple yang membawa bagian 1: ”Apa yang menambah atau mengurangi poin
        // kebahagiaan akhir?”; bagian 2: ”What added to or reduced the final score?” dalam AddPlayers.
        terms["players.stats.focus.happiness.question"] = ("Apa yang menambah atau mengurangi poin kebahagiaan akhir?", "What added to or reduced the final score?");
        // Memperbarui `terms[”players.stats.focus.happiness.with_deduction”]` menggunakan tuple yang membawa bagian 1: ”Sebagian poin kebahagiaan hilang
        // karena penalti misi atau pinjaman; nilai pengu...; bagian 2: ”Some happiness points were lost to mission or loan penalties; the deduction is ...
        // dalam AddPlayers.
        terms["players.stats.focus.happiness.with_deduction"] = ("Sebagian poin kebahagiaan hilang karena penalti misi atau pinjaman; nilai pengurangnya ditampilkan di bawah.", "Some happiness points were lost to mission or loan penalties; the deduction is shown below.");
        // Memperbarui `terms[”players.stats.focus.happiness.with_deduction.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Bangun poin
        // kebahagiaan dari kebutuhan, donasi, dan tujuan yang terjangkau samb...; bagian 2: ”Build happiness points through affordable needs, donations,
        // and goals while pre... dalam AddPlayers.
        terms["players.stats.focus.happiness.with_deduction.recommendation"] = ("Bangun poin kebahagiaan dari kebutuhan, donasi, dan tujuan yang terjangkau sambil mencegah penalti misi serta pinjaman yang tidak mampu dilunasi.", "Build happiness points through affordable needs, donations, and goals while preventing mission penalties and loans that cannot be repaid.");
        // Memperbarui `terms[”players.stats.focus.happiness.no_deduction”]` menggunakan tuple yang membawa bagian 1: ”Tidak ada pengurangan karena misi
        // atau pinjaman; skor berasal dari komponen pen...; bagian 2: ”There were no mission or loan deductions; the score came from positive componen...
        // dalam AddPlayers.
        terms["players.stats.focus.happiness.no_deduction"] = ("Tidak ada pengurangan karena misi atau pinjaman; skor berasal dari komponen penambah.", "There were no mission or loan deductions; the score came from positive components.");
        // Memperbarui `terms[”players.stats.focus.happiness.no_deduction.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Pertahankan skor tanpa
        // mengorbankan arus uang: pilih kebutuhan, donasi, tabunga...; bagian 2: ”Maintain the score without sacrificing cash flow: choose needs,
        // donations, savi... dalam AddPlayers.
        terms["players.stats.focus.happiness.no_deduction.recommendation"] = ("Pertahankan skor tanpa mengorbankan arus uang: pilih kebutuhan, donasi, tabungan, dan tujuan yang sesuai kemampuan.", "Maintain the score without sacrificing cash flow: choose needs, donations, savings, and goals that remain affordable.");
        // Memperbarui `terms[”players.stats.points_added”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan penambah”; bagian 2: ”Happiness
        // Points added” dalam AddPlayers.
        terms["players.stats.points_added"] = ("Poin Kebahagiaan penambah", "Happiness Points added");
        // Memperbarui `terms[”players.stats.points_deducted”]` menggunakan tuple yang membawa bagian 1: ”Pengurangan poin kebahagiaan”; bagian 2:
        // ”Happiness Point deductions” dalam AddPlayers.
        terms["players.stats.points_deducted"] = ("Pengurangan poin kebahagiaan", "Happiness Point deductions");
        // Memperbarui `terms[”players.support.intro.title”]` menggunakan tuple yang membawa bagian 1: ”Cara membaca bagian ini”; bagian 2: ”How to read
        // this section” dalam AddPlayers.
        terms["players.support.intro.title"] = ("Cara membaca bagian ini", "How to read this section");
        // Memperbarui `terms[”players.support.intro.desc”]` menggunakan tuple yang membawa bagian 1: ”Data Permainan adalah catatan langsung dari sesi.
        // Hasil Analisis adalah perhitu...; bagian 2: ”Game Data is recorded directly from the session. Analysis Results are calculate... dalam AddPlayers.
        terms["players.support.intro.desc"] = ("Data Permainan adalah catatan langsung dari sesi. Hasil Analisis adalah perhitungan dari catatan tersebut. Nilai nol tetap merupakan data yang tercatat; tanda pisah berarti data belum tersedia atau tidak berlaku.", "Game Data is recorded directly from the session. Analysis Results are calculated from those records. Zero is still a recorded value; a dash means data is unavailable or not applicable.");
        // Memperbarui `terms[”players.support.kind.raw”]` menggunakan tuple yang membawa bagian 1: ”Catatan permainan”; bagian 2: ”Gameplay record” dalam
        // AddPlayers.
        terms["players.support.kind.raw"] = ("Catatan permainan", "Gameplay record");
        // Memperbarui `terms[”players.support.kind.derived”]` menggunakan tuple yang membawa bagian 1: ”Hasil perhitungan”; bagian 2: ”Calculated result”
        // dalam AddPlayers.
        terms["players.support.kind.derived"] = ("Hasil perhitungan", "Calculated result");
        // Memperbarui `terms[”players.support.mode.advanced”]` menggunakan tuple yang membawa bagian 1: ”Khusus mode mahir”; bagian 2: ”Advanced mode only”
        // dalam AddPlayers.
        terms["players.support.mode.advanced"] = ("Khusus mode mahir", "Advanced mode only");
        // Memperbarui `terms[”players.support.field.meaning”]` menggunakan tuple yang membawa bagian 1: ”Artinya”; bagian 2: ”Meaning” dalam AddPlayers.
        terms["players.support.field.meaning"] = ("Artinya", "Meaning");
        // Memperbarui `terms[”players.support.field.guide”]` menggunakan tuple yang membawa bagian 1: ”Cara membaca”; bagian 2: ”How to read” dalam
        // AddPlayers.
        terms["players.support.field.guide"] = ("Cara membaca", "How to read");
        // Memperbarui `terms[”players.support.field.recommendation”]` menggunakan tuple yang membawa bagian 1: ”Saran”; bagian 2: ”Suggestion” dalam
        // AddPlayers.
        terms["players.support.field.recommendation"] = ("Saran", "Suggestion");
        // Memperbarui `terms[”players.support.field.formula”]` menggunakan tuple yang membawa bagian 1: ”Cara menghitung”; bagian 2: ”How it is calculated”
        // dalam AddPlayers.
        terms["players.support.field.formula"] = ("Cara menghitung", "How it is calculated");
        // Memperbarui `terms[”players.support.field.actual_calculation”]` menggunakan tuple yang membawa bagian 1: ”Hitungan dengan angka pemain”; bagian
        // 2: ”Calculation with the player's numbers” dalam AddPlayers.
        terms["players.support.field.actual_calculation"] = ("Hitungan dengan angka pemain", "Calculation with the player's numbers");
        // Memperbarui `terms[”players.support.field.source”]` menggunakan tuple yang membawa bagian 1: ”Sumber data”; bagian 2: ”Data source” dalam
        // AddPlayers.
        terms["players.support.field.source"] = ("Sumber data", "Data source");
        // Memperbarui `terms[”players.support.state.recorded”]` menggunakan tuple yang membawa bagian 1: ”Tercatat”; bagian 2: ”Recorded” dalam AddPlayers.
        terms["players.support.state.recorded"] = ("Tercatat", "Recorded");
        // Memperbarui `terms[”players.support.state.zero”]` menggunakan tuple yang membawa bagian 1: ”Nilainya nol”; bagian 2: ”Value is zero” dalam
        // AddPlayers.
        terms["players.support.state.zero"] = ("Nilainya nol", "Value is zero");
        // Memperbarui `terms[”players.support.state.unavailable”]` menggunakan tuple yang membawa bagian 1: ”Belum ada data”; bagian 2: ”No data yet” dalam
        // AddPlayers.
        terms["players.support.state.unavailable"] = ("Belum ada data", "No data yet");
        // Memperbarui `terms[”players.support.state.not_applicable”]` menggunakan tuple yang membawa bagian 1: ”Tidak berlaku”; bagian 2: ”Not applicable”
        // dalam AddPlayers.
        terms["players.support.state.not_applicable"] = ("Tidak berlaku", "Not applicable");
        // Memperbarui `terms[”players.support.value.unavailable”]` menggunakan tuple yang membawa bagian 1: ”—”; bagian 2: ”—” dalam AddPlayers.
        terms["players.support.value.unavailable"] = ("—", "—");
        // Memperbarui `terms[”players.support.value.yes”]` menggunakan tuple yang membawa bagian 1: ”Ya”; bagian 2: ”Yes” dalam AddPlayers.
        terms["players.support.value.yes"] = ("Ya", "Yes");
        // Memperbarui `terms[”players.support.value.no”]` menggunakan tuple yang membawa bagian 1: ”Tidak”; bagian 2: ”No” dalam AddPlayers.
        terms["players.support.value.no"] = ("Tidak", "No");
        // Memperbarui `terms[”players.support.value.preparation”]` menggunakan tuple yang membawa bagian 1: ”Persiapan”; bagian 2: ”Setup” dalam
        // AddPlayers.
        terms["players.support.value.preparation"] = ("Persiapan", "Setup");
        // Memperbarui `terms[”players.support.value.preparation_loan”]` menggunakan tuple yang membawa bagian 1: ”Pinjaman awal saat persiapan”; bagian 2:
        // ”Setup loan” dalam AddPlayers.
        terms["players.support.value.preparation_loan"] = ("Pinjaman awal saat persiapan", "Setup loan");
        // Memperbarui `terms[”players.support.value.day_index”]` menggunakan tuple yang membawa bagian 1: ”Hari ke-{0}”; bagian 2: ”Day {0}” dalam
        // AddPlayers.
        terms["players.support.value.day_index"] = ("Hari ke-{0}", "Day {0}");
        // Memperbarui `terms[”players.support.value.rank_position”]` menggunakan tuple yang membawa bagian 1: ”ke-{0}”; bagian 2: ”Rank {0}” dalam
        // AddPlayers.
        terms["players.support.value.rank_position"] = ("ke-{0}", "Rank {0}");
        // Memperbarui `terms[”players.support.value.no_loan_recorded”]` menggunakan tuple yang membawa bagian 1: ”Tidak ada pinjaman tercatat”; bagian 2:
        // ”No recorded loan” dalam AddPlayers.
        terms["players.support.value.no_loan_recorded"] = ("Tidak ada pinjaman tercatat", "No recorded loan");
        // Memperbarui `terms[”players.support.unit.coins”]` menggunakan tuple yang membawa bagian 1: ”koin”; bagian 2: ”coins” dalam AddPlayers.
        terms["players.support.unit.coins"] = ("koin", "coins");
        // Memperbarui `terms[”players.support.unit.points”]` menggunakan tuple yang membawa bagian 1: ”poin kebahagiaan”; bagian 2: ”happiness points”
        // dalam AddPlayers.
        terms["players.support.unit.points"] = ("poin kebahagiaan", "happiness points");
        // Memperbarui `terms[”players.support.unit.percent”]` menggunakan tuple yang membawa bagian 1: ”%”; bagian 2: ”%” dalam AddPlayers.
        terms["players.support.unit.percent"] = ("%", "%");
        // Memperbarui `terms[”players.support.unit.multiplier”]` menggunakan tuple yang membawa bagian 1: ”×”; bagian 2: ”×” dalam AddPlayers.
        terms["players.support.unit.multiplier"] = ("×", "×");
        // Memperbarui `terms[”players.support.unit.rank”]` menggunakan tuple yang membawa bagian 1: ”peringkat”; bagian 2: ”rank” dalam AddPlayers.
        terms["players.support.unit.rank"] = ("peringkat", "rank");
        // Memperbarui `terms[”players.support.unit.score”]` menggunakan tuple yang membawa bagian 1: ”skor”; bagian 2: ”score” dalam AddPlayers.
        terms["players.support.unit.score"] = ("skor", "score");
        // Memperbarui `terms[”players.support.unit.day”]` menggunakan tuple yang membawa bagian 1: ”hari”; bagian 2: ”day” dalam AddPlayers.
        terms["players.support.unit.day"] = ("hari", "day");
        // Memperbarui `terms[”players.support.unit.turn”]` menggunakan tuple yang membawa bagian 1: ”giliran”; bagian 2: ”turn” dalam AddPlayers.
        terms["players.support.unit.turn"] = ("giliran", "turn");
        // Memperbarui `terms[”players.support.unit.action_slot”]` menggunakan tuple yang membawa bagian 1: ”Urutan Aksi”; bagian 2: ”Action Slot” dalam
        // AddPlayers.
        terms["players.support.unit.action_slot"] = ("Urutan Aksi", "Action Slot");
        // Memperbarui `terms[”players.support.unit.ingredient_cards”]` menggunakan tuple yang membawa bagian 1: ”kartu bahan”; bagian 2: ”ingredient cards”
        // dalam AddPlayers.
        terms["players.support.unit.ingredient_cards"] = ("kartu bahan", "ingredient cards");
        // Memperbarui `terms[”players.support.unit.orders”]` menggunakan tuple yang membawa bagian 1: ”pesanan”; bagian 2: ”orders” dalam AddPlayers.
        terms["players.support.unit.orders"] = ("pesanan", "orders");
        // Memperbarui `terms[”players.support.unit.need_cards”]` menggunakan tuple yang membawa bagian 1: ”kartu kebutuhan”; bagian 2: ”need cards” dalam
        // AddPlayers.
        terms["players.support.unit.need_cards"] = ("kartu kebutuhan", "need cards");
        // Memperbarui `terms[”players.support.unit.gold_cards”]` menggunakan tuple yang membawa bagian 1: ”kartu emas”; bagian 2: ”gold cards” dalam
        // AddPlayers.
        terms["players.support.unit.gold_cards"] = ("kartu emas", "gold cards");
        // Memperbarui `terms[”players.support.unit.risk_events”]` menggunakan tuple yang membawa bagian 1: ”kejadian risiko”; bagian 2: ”risk events” dalam
        // AddPlayers.
        terms["players.support.unit.risk_events"] = ("kejadian risiko", "risk events");
        // Memperbarui `terms[”players.support.unit.goals”]` menggunakan tuple yang membawa bagian 1: ”target”; bagian 2: ”goals” dalam AddPlayers.
        terms["players.support.unit.goals"] = ("target", "goals");
        // Memperbarui `terms[”players.support.unit.loans”]` menggunakan tuple yang membawa bagian 1: ”pinjaman”; bagian 2: ”loans” dalam AddPlayers.
        terms["players.support.unit.loans"] = ("pinjaman", "loans");
        // Memperbarui `terms[”players.support.unit.actions”]` menggunakan tuple yang membawa bagian 1: ”aksi”; bagian 2: ”actions” dalam AddPlayers.
        terms["players.support.unit.actions"] = ("aksi", "actions");
        terms["players.support.unit.action_tokens"] = ("aksi", "actions");
        // Memperbarui `terms[”players.support.unit.events”]` menggunakan tuple yang membawa bagian 1: ”kejadian”; bagian 2: ”events” dalam AddPlayers.
        terms["players.support.unit.events"] = ("kejadian", "events");
        // Memperbarui `terms[”players.support.unit.transactions”]` menggunakan tuple yang membawa bagian 1: ”transaksi”; bagian 2: ”transactions” dalam
        // AddPlayers.
        terms["players.support.unit.transactions"] = ("transaksi", "transactions");
        // Memperbarui `terms[”players.support.unit.times”]` menggunakan tuple yang membawa bagian 1: ”kali”; bagian 2: ”times” dalam AddPlayers.
        terms["players.support.unit.times"] = ("kali", "times");
        // Memperbarui `terms[”players.support.unit.sources”]` menggunakan tuple yang membawa bagian 1: ”sumber”; bagian 2: ”sources” dalam AddPlayers.
        terms["players.support.unit.sources"] = ("sumber", "sources");
        // Memperbarui `terms[”players.support.unit.cards”]` menggunakan tuple yang membawa bagian 1: ”kartu”; bagian 2: ”cards” dalam AddPlayers.
        terms["players.support.unit.cards"] = ("kartu", "cards");
        // Memperbarui `terms[”players.support.unit.orders_per_turn”]` menggunakan tuple yang membawa bagian 1: ”pesanan/hari aktif”; bagian 2:
        // ”orders/active day” dalam AddPlayers.
        terms["players.support.unit.orders_per_turn"] = ("pesanan/hari aktif", "orders/active day");
        // Memperbarui `terms[”players.support.unit.ingredients_per_order”]` menggunakan tuple yang membawa bagian 1: ”kartu bahan/pesanan”; bagian 2:
        // ”ingredient cards/order” dalam AddPlayers.
        terms["players.support.unit.ingredients_per_order"] = ("kartu bahan/pesanan", "ingredient cards/order");
        // Memperbarui `terms[”players.support.meaning.raw”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah data yang dicatat selama permainan.”;
        // bagian 2: ”{0} is data recorded during the game.” dalam AddPlayers.
        terms["players.support.meaning.raw"] = ("{0} adalah data yang dicatat selama permainan.", "{0} is data recorded during the game.");
        // Memperbarui `terms[”players.support.meaning.raw_coins”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah jumlah koin yang tercatat.”; bagian
        // 2: ”{0} is the recorded coin amount.” dalam AddPlayers.
        terms["players.support.meaning.raw_coins"] = ("{0} adalah jumlah koin yang tercatat.", "{0} is the recorded coin amount.");
        // Memperbarui `terms[”players.support.meaning.raw_points”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah komponen pembentuk total Poin
        // Kebahagiaan; nilai negatif mengurangi ...; bagian 2: ”{0} contributes to total Happiness Points; a negative value reduces the total.” dalam
        // AddPlayers.
        terms["players.support.meaning.raw_points"] = ("{0} adalah komponen pembentuk total Poin Kebahagiaan; nilai negatif mengurangi totalnya.", "{0} contributes to total Happiness Points; a negative value reduces the total.");
        // Memperbarui `terms[”players.support.meaning.raw_rank”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah posisi pemain dibanding pemain
        // lain.”; bagian 2: ”{0} is the player's position compared with others.” dalam AddPlayers.
        terms["players.support.meaning.raw_rank"] = ("{0} adalah posisi pemain dibanding pemain lain.", "{0} is the player's position compared with others.");
        // Memperbarui `terms[”players.support.meaning.raw_timeline”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan waktu nilai atau kejadian
        // ini muncul.”; bagian 2: ”{0} shows when this value or event appeared.” dalam AddPlayers.
        terms["players.support.meaning.raw_timeline"] = ("{0} menunjukkan waktu nilai atau kejadian ini muncul.", "{0} shows when this value or event appeared.");
        // Memperbarui `terms[”players.support.meaning.raw_count”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah jumlah benda, pilihan, atau
        // kejadian yang tercatat.”; bagian 2: ”{0} is the recorded number of items, choices, or events.” dalam AddPlayers.
        terms["players.support.meaning.raw_count"] = ("{0} adalah jumlah benda, pilihan, atau kejadian yang tercatat.", "{0} is the recorded number of items, choices, or events.");
        // Memperbarui `terms[”players.support.meaning.derived”]` menggunakan tuple yang membawa bagian 1: ”{0} dihitung dari beberapa data permainan.”;
        // bagian 2: ”{0} is calculated from several gameplay records.” dalam AddPlayers.
        terms["players.support.meaning.derived"] = ("{0} dihitung dari beberapa data permainan.", "{0} is calculated from several gameplay records.");
        // Memperbarui `terms[”players.support.meaning.growth”]` menggunakan tuple yang membawa bagian 1: ”{0} membandingkan Koin Tersisa dengan Koin Awal;
        // nilai 100% berarti sama dengan...; bagian 2: ”{0} compares Remaining Coins with Starting Coins; 100% means unchanged, not 100... dalam
        // AddPlayers.
        terms["players.support.meaning.growth"] = ("{0} membandingkan Koin Tersisa dengan Koin Awal; nilai 100% berarti sama dengan awal, bukan bertambah 100%.", "{0} compares Remaining Coins with Starting Coins; 100% means unchanged, not 100% growth.");
        terms["players.support.meaning.cash_growth"] = ("{0} menunjukkan kenaikan atau penurunan koin dari awal; tanda + berarti naik, tanda - berarti turun, dan 0% berarti tetap.", "{0} shows the increase or decrease in coins from the start; + means an increase, - means a decrease, and 0% means unchanged.");
        // Memperbarui `terms[”players.support.meaning.income_mix”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan apakah pemasukan berasal dari
        // beberapa sumber atau bergantung p...; bagian 2: ”{0} shows whether income came from several sources or depended on one source.” dalam AddPlayers.
        terms["players.support.meaning.income_mix"] = ("{0} menunjukkan apakah pemasukan berasal dari beberapa sumber atau bergantung pada satu sumber.", "{0} shows whether income came from several sources or depended on one source.");
        // Memperbarui `terms[”players.support.meaning.expense_efficiency”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan bagian Total Koin
        // Keluar yang digunakan untuk pembelian bahan, ...; bagian 2: ”{0} shows the share of Total Outgoing Coins spent on ingredient purchases, incl...
        // dalam AddPlayers.
        terms["players.support.meaning.expense_efficiency"] = ("{0} menunjukkan bagian Total Koin Keluar yang digunakan untuk pembelian bahan, termasuk bahan yang belum dipakai.", "{0} shows the share of Total Outgoing Coins spent on ingredient purchases, including unused ingredients.");
        // Memperbarui `terms[”players.support.meaning.business”]` menggunakan tuple yang membawa bagian 1: ”{0} membandingkan Total Pendapatan Pesanan
        // dengan Biaya Bahan Terpakai pada pes...; bagian 2: ”{0} compares Total Order Income with Used Ingredient Cost in completed orders.” dalam
        // AddPlayers.
        terms["players.support.meaning.business"] = ("{0} membandingkan Total Pendapatan Pesanan dengan Biaya Bahan Terpakai pada pesanan selesai.", "{0} compares Total Order Income with Used Ingredient Cost in completed orders.");
        // Memperbarui `terms[”players.support.meaning.gold_return”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan untung atau rugi dari
        // seluruh transaksi emas.”; bagian 2: ”{0} shows the gain or loss from all gold transactions.” dalam AddPlayers.
        terms["players.support.meaning.gold_return"] = ("{0} menunjukkan untung atau rugi dari seluruh transaksi emas.", "{0} shows the gain or loss from all gold transactions.");
        // Memperbarui `terms[”players.support.meaning.risk_impact”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan seberapa besar risiko
        // memengaruhi kondisi keuangan pemain.”; bagian 2: ”{0} shows how strongly risk affected the player's finances.” dalam AddPlayers.
        terms["players.support.meaning.risk_impact"] = ("{0} menunjukkan seberapa besar risiko memengaruhi kondisi keuangan pemain.", "{0} shows how strongly risk affected the player's finances.");
        // Memperbarui `terms[”players.support.meaning.risk_protection”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan seberapa baik asuransi
        // mengurangi dampak risiko.”; bagian 2: ”{0} shows how effectively insurance reduced risk impact.” dalam AddPlayers.
        terms["players.support.meaning.risk_protection"] = ("{0} menunjukkan seberapa baik asuransi mengurangi dampak risiko.", "{0} shows how effectively insurance reduced risk impact.");
        // Memperbarui `terms[”players.support.meaning.risk_appetite”]` menggunakan tuple yang membawa bagian 1: ”{0} menggambarkan kecenderungan pemain
        // menerima risiko selama permainan.”; bagian 2: ”{0} describes the player's tendency to accept risk during the game.” dalam AddPlayers.
        terms["players.support.meaning.risk_appetite"] = ("{0} menggambarkan kecenderungan pemain menerima risiko selama permainan.", "{0} describes the player's tendency to accept risk during the game.");
        // Memperbarui `terms[”players.support.meaning.debt”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan besar saldo pinjaman yang belum
        // lunas dibandingkan kas pemain s...; bagian 2: ”{0} shows the outstanding loan balance relative to the player's current cash.” dalam AddPlayers.
        terms["players.support.meaning.debt"] = ("{0} menunjukkan besar saldo pinjaman yang belum lunas dibandingkan kas pemain saat ini.", "{0} shows the outstanding loan balance relative to the player's current cash.");
        // Memperbarui `terms[”players.support.meaning.goals”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan seberapa aktif pemain mengejar dan
        // mendanai target finansial.”; bagian 2: ”{0} shows how actively the player pursued and funded financial goals.” dalam AddPlayers.
        terms["players.support.meaning.goals"] = ("{0} menunjukkan seberapa aktif pemain mengejar dan mendanai target finansial.", "{0} shows how actively the player pursued and funded financial goals.");
        // Memperbarui `terms[”players.support.meaning.actions”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan porsi aksi yang langsung
        // menghasilkan koin masuk dibandingkan s...; bagian 2: ”{0} shows the share of action-slot actions that directly generated incoming coi... dalam
        // AddPlayers.
        terms["players.support.meaning.actions"] = ("{0} menunjukkan bagian aksi yang menghasilkan pemasukan dari kerja lepas dan penjualan masakan. Yang dihitung adalah jumlah aksi sepanjang sesi.", "{0} shows the share of actions that generate income from freelance work and meal sales. It counts actions taken throughout the session.");
        // Memperbarui `terms[”players.support.meaning.orders”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan bagian pesanan makanan yang
        // berhasil diselesaikan dari seluruh ...; bagian 2: ”{0} shows the share of meal-order opportunities that were completed.” dalam AddPlayers.
        terms["players.support.meaning.orders"] = ("{0} menunjukkan bagian pesanan makanan yang berhasil diselesaikan dari seluruh kesempatan.", "{0} shows the share of meal-order opportunities that were completed.");
        // Memperbarui `terms[”players.support.meaning.planning”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan seberapa banyak keputusan yang
        // diarahkan untuk manfaat jangka p...; bagian 2: ”{0} shows how many decisions were aimed at long-term benefits.” dalam AddPlayers.
        terms["players.support.meaning.planning"] = ("{0} menunjukkan bagian aksi yang dipakai untuk menabung, membeli target finansial, membeli asuransi, dan melunasi pinjaman.", "{0} shows the share of actions spent on savings, financial goal purchases, insurance purchases, and loan repayment.");
        // Memperbarui `terms[”players.support.meaning.needs”]` menggunakan tuple yang membawa bagian 1: ”{0} membantu melihat keseimbangan pemenuhan
        // kebutuhan dan pencapaian misi pemai...; bagian 2: ”{0} helps show the balance of need fulfillment and mission achievement.” dalam AddPlayers.
        terms["players.support.meaning.needs"] = ("{0} membantu melihat keseimbangan pemenuhan kebutuhan dan pencapaian misi pemain.", "{0} helps show the balance of need fulfillment and mission achievement.");
        // Memperbarui `terms[”players.support.meaning.need_cards_purchased”]` menggunakan tuple yang membawa bagian 1: ”Jumlah seluruh kartu kebutuhan yang
        // pernah dibeli selama permainan.”; bagian 2: ”Total number of need cards purchased during the game.” dalam AddPlayers.
        terms["players.support.meaning.need_cards_purchased"] = ("Jumlah seluruh kartu kebutuhan yang pernah dibeli selama permainan.", "Total number of need cards purchased during the game.");
        // Memperbarui `terms[”players.support.meaning.need_cards_owned”]` menggunakan tuple yang membawa bagian 1: ”Jumlah kartu kebutuhan yang masih
        // dimiliki setelah aktivitas terakhir.”; bagian 2: ”Number of need cards still owned after the latest activity.” dalam AddPlayers.
        terms["players.support.meaning.need_cards_owned"] = ("Jumlah kartu kebutuhan yang masih dimiliki setelah aktivitas terakhir.", "Number of need cards still owned after the latest activity.");
        // Memperbarui `terms[”players.support.meaning.need_cards_sold_note”]` menggunakan tuple yang membawa bagian 1: ”Kartu yang dijual melalui Tindakan
        // Darurat tidak dihitung.”; bagian 2: ”Cards sold through an Emergency Action are not included.” dalam AddPlayers.
        terms["players.support.meaning.need_cards_sold_note"] = ("Kartu yang dijual melalui Tindakan Darurat tidak dihitung.", "Cards sold through an Emergency Action are not included.");
        // Memperbarui `terms[”players.support.meaning.need_level_owned”]` menggunakan tuple yang membawa bagian 1: ”Jumlah kartu kebutuhan yang masih
        // dimiliki pada tingkat ini. Kartu yang dijual ...; bagian 2: ”Need cards still owned at this tier. Cards sold through an Emergency Action are...
        // dalam AddPlayers.
        terms["players.support.meaning.need_level_owned"] = ("Jumlah kartu kebutuhan yang masih dimiliki pada tingkat ini. Kartu yang dijual melalui Tindakan Darurat tidak termasuk.", "Need cards still owned at this tier. Cards sold through an Emergency Action are excluded.");
        // Memperbarui `terms[”players.support.meaning.mission_need_owned”]` menggunakan tuple yang membawa bagian 1: ”Menunjukkan apakah pemain masih
        // memiliki kartu kebutuhan tersier yang diminta o...; bagian 2: ”Shows whether the player still owns the tertiary need card required by the coll...
        // dalam AddPlayers.
        terms["players.support.meaning.mission_need_owned"] = ("Menunjukkan apakah kartu kebutuhan tersier yang diminta misi pernah dibeli. Penjualan kartu melalui Tindakan Darurat mengurangi kepemilikan saat ini, tetapi tidak menghapus riwayat pembelian untuk misi.", "Shows whether the tertiary need card required by the mission was purchased. Selling it through an Emergency Action reduces current ownership but does not erase the purchase history used for the mission.");
        // Memperbarui `terms[”players.support.meaning.collection_mission”]` menggunakan tuple yang membawa bagian 1: ”Menunjukkan apakah seluruh syarat
        // misi koleksi pribadi pemain sudah terpenuhi p...; bagian 2: ”Shows whether all requirements of the player's private collection mission were ...
        // dalam AddPlayers.
        terms["players.support.meaning.collection_mission"] = ("Menunjukkan apakah seluruh syarat pembelian dalam misi koleksi sudah terpenuhi berdasarkan riwayat permainan. Kartu yang kemudian dijual tetap dihitung untuk misi, tetapi tidak lagi dihitung dalam kartu yang masih dimiliki.", "Shows whether all collection-mission purchase requirements have been met in the game history. Cards sold later still count toward the mission but no longer count as currently owned cards.");
        // Memperbarui `terms[”players.support.meaning.need_purchase_cost”]` menggunakan tuple yang membawa bagian 1: ”Jumlah koin yang dibayar untuk
        // seluruh kartu kebutuhan yang dibeli.”; bagian 2: ”Coins paid for all purchased need cards.” dalam AddPlayers.
        terms["players.support.meaning.need_purchase_cost"] = ("Jumlah koin yang dibayar untuk seluruh kartu kebutuhan yang dibeli.", "Coins paid for all purchased need cards.");
        // Memperbarui `terms[”players.support.meaning.need_balance”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan pemerataan kartu kebutuhan
        // yang dimiliki antara tingkat primer,...; bagian 2: ”{0} shows how evenly owned need cards are distributed across primary, secondary... dalam
        // AddPlayers.
        terms["players.support.meaning.need_balance"] = ("{0} menunjukkan pemerataan kartu kebutuhan yang dimiliki antara tingkat primer, sekunder, dan tersier.", "{0} shows how evenly owned need cards are distributed across primary, secondary, and tertiary tiers.");
        // Memperbarui `terms[”players.support.meaning.donation”]` menggunakan tuple yang membawa bagian 1: ”{0} menggambarkan jumlah, keteraturan, atau
        // komitmen donasi pemain.”; bagian 2: ”{0} describes the amount, consistency, or commitment of the player's donations.... dalam AddPlayers.
        terms["players.support.meaning.donation"] = ("{0} menggambarkan jumlah, keteraturan, atau komitmen donasi pemain.", "{0} describes the amount, consistency, or commitment of the player's donations.");
        // Memperbarui `terms[”players.support.meaning.baseline”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah angka awal untuk melihat perubahan
        // keuangan pemain.”; bagian 2: ”{0} is the starting value used to see financial change.” dalam AddPlayers.
        terms["players.support.meaning.baseline"] = ("{0} adalah angka awal untuk melihat perubahan keuangan pemain.", "{0} is the starting value used to see financial change.");
        // Memperbarui `terms[”players.support.meaning.income”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan jumlah uang yang dihasilkan
        // pemain.”; bagian 2: ”{0} shows how much money the player earned.” dalam AddPlayers.
        terms["players.support.meaning.income"] = ("{0} menunjukkan jumlah uang yang dihasilkan pemain.", "{0} shows how much money the player earned.");
        // Memperbarui `terms[”players.support.meaning.spending”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan jumlah uang yang dikeluarkan
        // pemain.”; bagian 2: ”{0} shows how much money the player spent.” dalam AddPlayers.
        terms["players.support.meaning.spending"] = ("{0} menunjukkan jumlah uang yang dikeluarkan pemain.", "{0} shows how much money the player spent.");
        // Memperbarui `terms[”players.support.meaning.cash_position”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah koin yang masih dimiliki
        // setelah transaksi terakhir; tabungan dan ni...; bagian 2: ”{0} is the coins still held after the latest transaction; savings and card valu...
        // dalam AddPlayers.
        terms["players.support.meaning.cash_position"] = ("{0} adalah koin yang masih dimiliki setelah transaksi terakhir; tabungan dan nilai kartu tidak termasuk.", "{0} is the coins still held after the latest transaction; savings and card values are excluded.");
        // Memperbarui `terms[”players.support.meaning.savings”]` menggunakan tuple yang membawa bagian 1: ”Koin yang masih tersimpan setelah pembayaran
        // target, terpisah dari koin yang di...; bagian 2: ”Coins still saved after goal payments, separate from coins held by the player.” dalam
        // AddPlayers.
        terms["players.support.meaning.savings"] = ("Koin yang masih tersimpan setelah pembayaran target, terpisah dari koin yang dipegang pemain.", "Coins still saved after goal payments, separate from coins held by the player.");
        // Memperbarui `terms[”players.support.meaning.completed_goals”]` menggunakan tuple yang membawa bagian 1: ”Jumlah target finansial yang sudah
        // diselesaikan.”; bagian 2: ”Number of financial goals that have been completed.” dalam AddPlayers.
        terms["players.support.meaning.completed_goals"] = ("Jumlah target finansial yang kartunya sudah diperoleh setelah biaya pembeliannya dibayarkan ke bank.", "Number of financial goals whose cards were obtained after paying their purchase costs to the bank.");
        // Memperbarui `terms[”players.support.meaning.outstanding_loan”]` menggunakan tuple yang membawa bagian 1: ”Koin pinjaman yang masih harus
        // dikembalikan. Nol berarti tidak ada sisa pinjama...; bagian 2: ”Borrowed coins still to be repaid. Zero means no outstanding loan balance.” dalam
        // AddPlayers.
        terms["players.support.meaning.outstanding_loan"] = ("Koin pinjaman yang masih harus dikembalikan. Nol berarti tidak ada sisa pinjaman.", "Borrowed coins still to be repaid. Zero means no outstanding loan balance.");
        // Memperbarui `terms[”players.support.meaning.inventory”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan jumlah dan penggunaan bahan.”;
        // bagian 2: ”{0} shows ingredient amounts and use.” dalam AddPlayers.
        terms["players.support.meaning.inventory"] = ("{0} menunjukkan jumlah dan penggunaan bahan.", "{0} shows ingredient amounts and use.");
        // Memperbarui `terms[”players.support.meaning.business_activity”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan hasil pemain mengubah
        // bahan dan pesanan menjadi pemasukan.”; bagian 2: ”{0} shows how the player turned ingredients and orders into income.” dalam AddPlayers.
        terms["players.support.meaning.business_activity"] = ("{0} menunjukkan hasil pemain mengubah bahan dan pesanan menjadi pemasukan.", "{0} shows how the player turned ingredients and orders into income.");
        // Memperbarui `terms[”players.support.meaning.gold_investment”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan hasil dari membeli,
        // menyimpan, dan menjual emas.”; bagian 2: ”{0} shows the result of buying, holding, and selling gold.” dalam AddPlayers.
        terms["players.support.meaning.gold_investment"] = ("{0} menunjukkan hasil dari membeli, menyimpan, dan menjual emas.", "{0} shows the result of buying, holding, and selling gold.");
        // Memperbarui `terms[”players.support.meaning.pension”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan cadangan pemain pada akhir
        // permainan.”; bagian 2: ”{0} shows the player's reserve at the end of the game.” dalam AddPlayers.
        terms["players.support.meaning.pension"] = ("{0} menunjukkan cadangan pemain pada akhir permainan.", "{0} shows the player's reserve at the end of the game.");
        // Memperbarui `terms[”players.support.meaning.risk”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan biaya risiko dan perlindungan yang
        // digunakan.”; bagian 2: ”{0} shows risk costs and the protection used.” dalam AddPlayers.
        terms["players.support.meaning.risk"] = ("{0} menunjukkan biaya risiko dan perlindungan yang digunakan.", "{0} shows risk costs and the protection used.");
        // Memperbarui `terms[”players.support.meaning.timeline”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan kapan keputusan terjadi dalam
        // permainan.”; bagian 2: ”{0} shows when a decision happened in the game.” dalam AddPlayers.
        terms["players.support.meaning.timeline"] = ("{0} menunjukkan kapan keputusan terjadi dalam permainan.", "{0} shows when a decision happened in the game.");
        // Memperbarui `terms[”players.support.meaning.outcome”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan hasil akhir pemain.”; bagian 2:
        // ”{0} shows the player's final result.” dalam AddPlayers.
        terms["players.support.meaning.outcome"] = ("{0} menunjukkan hasil akhir pemain.", "{0} shows the player's final result.");
        // Memperbarui `terms[”players.support.meaning.session_context”]` menggunakan tuple yang membawa bagian 1: ”{0} memastikan data berasal dari sesi
        // dan pemain yang benar.”; bagian 2: ”{0} confirms that the data belongs to the correct session and player.” dalam AddPlayers.
        terms["players.support.meaning.session_context"] = ("{0} memastikan data berasal dari sesi dan pemain yang benar.", "{0} confirms that the data belongs to the correct session and player.");
        // Memperbarui `terms[”players.support.guide.not_applicable”]` menggunakan tuple yang membawa bagian 1: ”Data ini hanya dipakai pada mode Mahir.”;
        // bagian 2: ”This data is only used in Advanced mode.” dalam AddPlayers.
        terms["players.support.guide.not_applicable"] = ("Data ini hanya dipakai pada mode Mahir.", "This data is only used in Advanced mode.");
        // Memperbarui `terms[”players.support.guide.unavailable”]` menggunakan tuple yang membawa bagian 1: ”Data belum cukup untuk menghitung nilai ini.
        // Tanda pisah bukan berarti nol.”; bagian 2: ”There is not enough data to calculate this value. A dash does not mean zero.” dalam AddPlayers.
        terms["players.support.guide.unavailable"] = ("Data belum cukup untuk menghitung nilai ini. Tanda pisah bukan berarti nol.", "There is not enough data to calculate this value. A dash does not mean zero.");
        // Memperbarui `terms[”players.support.guide.coins”]` menggunakan tuple yang membawa bagian 1: ”Baca sebagai koin, bukan Poin Kebahagiaan.”; bagian
        // 2: ”Read this as coins, not Happiness Points.” dalam AddPlayers.
        terms["players.support.guide.coins"] = ("Baca sebagai koin, bukan Poin Kebahagiaan.", "Read this as coins, not Happiness Points.");
        // Memperbarui `terms[”players.support.guide.points”]` menggunakan tuple yang membawa bagian 1: ”Nilai negatif mengurangi Poin Kebahagiaan.”; bagian
        // 2: ”A negative value reduces Happiness Points.” dalam AddPlayers.
        terms["players.support.guide.points"] = ("Nilai negatif mengurangi Poin Kebahagiaan.", "A negative value reduces Happiness Points.");
        // Memperbarui `terms[”players.support.guide.rank”]` menggunakan tuple yang membawa bagian 1: ”Peringkat 1 adalah yang tertinggi. Angka lebih besar
        // berarti posisi lebih renda...; bagian 2: ”Rank 1 is the highest. A larger number means a lower position.” dalam AddPlayers.
        terms["players.support.guide.rank"] = ("Peringkat 1 adalah yang tertinggi. Angka lebih besar berarti posisi lebih rendah.", "Rank 1 is the highest. A larger number means a lower position.");
        // Memperbarui `terms[”players.support.guide.turn”]` menggunakan tuple yang membawa bagian 1: ”Gunakan angka ini untuk menemukan waktu
        // kejadiannya.”; bagian 2: ”Use this number to find when the event happened.” dalam AddPlayers.
        terms["players.support.guide.turn"] = ("Gunakan angka ini untuk menemukan waktu kejadiannya.", "Use this number to find when the event happened.");
        // Memperbarui `terms[”players.support.guide.count”]` menggunakan tuple yang membawa bagian 1: ”Nilai nol berarti tidak ada yang tercatat, bukan
        // data yang hilang.”; bagian 2: ”Zero means none were recorded, not that data is missing.” dalam AddPlayers.
        terms["players.support.guide.count"] = ("Nilai nol berarti tidak ada yang tercatat, bukan data yang hilang.", "Zero means none were recorded, not that data is missing.");
        // Memperbarui `terms[”players.support.guide.raw”]` menggunakan tuple yang membawa bagian 1: ”Baca bersama data lain dalam kelompok yang sama.”;
        // bagian 2: ”Read this with the other data in the same group.” dalam AddPlayers.
        terms["players.support.guide.raw"] = ("Baca bersama data lain dalam kelompok yang sama.", "Read this with the other data in the same group.");
        // Memperbarui `terms[”players.support.guide.percent”]` menggunakan tuple yang membawa bagian 1: ”Baca dalam rentang 0–100%. Nilai tinggi tidak
        // selalu berarti lebih baik.”; bagian 2: ”Read this on a 0–100% scale. A higher value is not always better.” dalam AddPlayers.
        terms["players.support.guide.percent"] = ("Baca dalam rentang 0–100%. Nilai tinggi tidak selalu berarti lebih baik.", "Read this on a 0–100% scale. A higher value is not always better.");
        // Memperbarui `terms[”players.support.guide.ratio”]` menggunakan tuple yang membawa bagian 1: ”Ini adalah nilai perbandingan, bukan jumlah benda.”;
        // bagian 2: ”This is a comparison value, not an item count.” dalam AddPlayers.
        terms["players.support.guide.ratio"] = ("Ini adalah nilai perbandingan, bukan jumlah benda.", "This is a comparison value, not an item count.");
        // Memperbarui `terms[”players.support.guide.derived”]` menggunakan tuple yang membawa bagian 1: ”Gunakan sebagai petunjuk, lalu periksa data
        // sumbernya.”; bagian 2: ”Use this as a guide, then check its source data.” dalam AddPlayers.
        terms["players.support.guide.derived"] = ("Gunakan sebagai petunjuk, lalu periksa data sumbernya.", "Use this as a guide, then check its source data.");
        // Memperbarui `terms[”players.support.guide.baseline”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan dengan Koin Tersisa untuk melihat
        // apakah koin bertambah atau berkura...; bagian 2: ”Compare with Remaining Coins to see whether coins increased or decreased.” dalam AddPlayers.
        terms["players.support.guide.baseline"] = ("Bandingkan dengan Koin Tersisa untuk melihat apakah koin bertambah atau berkurang.", "Compare with Remaining Coins to see whether coins increased or decreased.");
        // Memperbarui `terms[”players.support.guide.income”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan dengan pengeluaran. Pemasukan yang lebih
        // besar menghasilkan arus kas...; bagian 2: ”Compare it with spending. Higher income produces positive cash flow.” dalam AddPlayers.
        terms["players.support.guide.income"] = ("Bandingkan dengan pengeluaran. Pemasukan yang lebih besar menghasilkan arus kas positif.", "Compare it with spending. Higher income produces positive cash flow.");
        // Memperbarui `terms[”players.support.guide.spending”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan dengan pemasukan dan cari pengeluaran
        // terbesar.”; bagian 2: ”Compare it with income and find the largest spending items.” dalam AddPlayers.
        terms["players.support.guide.spending"] = ("Bandingkan dengan pemasukan dan cari pengeluaran terbesar.", "Compare it with income and find the largest spending items.");
        // Memperbarui `terms[”players.support.guide.cash_position”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan dengan Koin Awal. Selisih positif
        // berarti koin bertambah.”; bagian 2: ”Compare with Starting Coins. A positive difference means coins increased.” dalam AddPlayers.
        terms["players.support.guide.cash_position"] = ("Bandingkan dengan Koin Awal. Selisih positif berarti koin bertambah.", "Compare with Starting Coins. A positive difference means coins increased.");
        // Memperbarui `terms[”players.support.guide.savings”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan Koin dalam Tabungan dengan Koin Awal
        // dan Koin Tersisa.”; bagian 2: ”Compare Coins in Savings with Starting Coins and Remaining Coins.” dalam AddPlayers.
        terms["players.support.guide.savings"] = ("Bandingkan Koin dalam Tabungan dengan Koin Awal dan Koin Tersisa.", "Compare Coins in Savings with Starting Coins and Remaining Coins.");
        // Memperbarui `terms[”players.support.guide.inventory”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan bahan yang diperoleh, dipakai,
        // tersisa, dan terbuang.”; bagian 2: ”Compare ingredients collected, used, remaining, and wasted.” dalam AddPlayers.
        terms["players.support.guide.inventory"] = ("Bandingkan bahan yang diperoleh, dipakai, tersisa, dan terbuang.", "Compare ingredients collected, used, remaining, and wasted.");
        // Memperbarui `terms[”players.support.guide.business_activity”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan Total Pendapatan Pesanan,
        // Pesanan Selesai, dan Biaya Bahan Terpakai....; bagian 2: ”Compare Total Order Income, Completed Orders, and Used Ingredient Cost.” dalam
        // AddPlayers.
        terms["players.support.guide.business_activity"] = ("Bandingkan Total Pendapatan Pesanan, Pesanan Selesai, dan Biaya Bahan Terpakai.", "Compare Total Order Income, Completed Orders, and Used Ingredient Cost.");
        // Memperbarui `terms[”players.support.guide.needs”]` menggunakan tuple yang membawa bagian 1: ”Periksa apakah belanja membantu memenuhi kebutuhan
        // primer dan misi.”; bagian 2: ”Check whether spending helped meet primary needs and missions.” dalam AddPlayers.
        terms["players.support.guide.needs"] = ("Periksa apakah belanja membantu memenuhi kebutuhan primer dan misi.", "Check whether spending helped meet primary needs and missions.");
        // Memperbarui `terms[”players.support.guide.donation”]` menggunakan tuple yang membawa bagian 1: ”Baca jumlah, frekuensi, peringkat, dan poin
        // kebahagiaan donasi bersama-sama.”; bagian 2: ”Read donation amount, frequency, rank, and happiness points together.” dalam AddPlayers.
        terms["players.support.guide.donation"] = ("Baca jumlah, frekuensi, peringkat, dan poin kebahagiaan donasi bersama-sama.", "Read donation amount, frequency, rank, and happiness points together.");
        // Memperbarui `terms[”players.support.guide.gold_investment”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan biaya beli, hasil jual, dan
        // kartu emas yang masih dimiliki. Selisih ...; bagian 2: ”Compare purchase cost, sale proceeds, and gold cards still held. The cashflow d... dalam
        // AddPlayers.
        terms["players.support.guide.gold_investment"] = ("Bandingkan biaya beli, hasil jual, dan kartu emas yang masih dimiliki. Selisih arus kas belum memasukkan nilai kartu emas yang belum dijual.", "Compare purchase cost, sale proceeds, and gold cards still held. The cashflow difference does not include the value of unsold gold cards.");
        // Memperbarui `terms[”players.support.guide.pension”]` menggunakan tuple yang membawa bagian 1: ”Total Dana Pensiun = Koin Tersisa + Nilai Kartu
        // Bahan Tersisa + Koin dalam Tabu...; bagian 2: ”Total Pension Fund = Remaining Coins + Remaining Ingredient Card Value + Coins ... dalam
        // AddPlayers.
        terms["players.support.guide.pension"] = ("Total Dana Pensiun = Koin Tersisa + Nilai Kartu Bahan Tersisa + Koin dalam Tabungan. Setiap kartu bahan tersisa dinilai 1 koin sesuai aturan dana pensiun.", "Total Pension Fund = Remaining Coins + Remaining Ingredient Card Value + Coins in Savings. Each remaining ingredient card is valued at 1 coin under the pension-fund rule.");
        // Memperbarui `terms[”players.support.guide.risk”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan kartu risiko yang muncul, nominal dampak
        // pada kartu, arus kas yang b...; bagian 2: ”Compare risk cards drawn, nominal card impact, actual recorded cashflow, insura... dalam AddPlayers.
        terms["players.support.guide.risk"] = ("Bandingkan kartu risiko yang muncul, nominal dampak pada kartu, arus kas yang benar-benar tercatat, perlindungan asuransi, dan Tindakan Darurat.", "Compare risk cards drawn, nominal card impact, actual recorded cashflow, insurance coverage, and Emergency Actions.");
        // Memperbarui `terms[”players.support.guide.goals”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan target yang dicoba, diselesaikan, dan
        // koin yang ditanam.”; bagian 2: ”Compare goals attempted, completed, and coins invested.” dalam AddPlayers.
        terms["players.support.guide.goals"] = ("Bandingkan target yang dicoba, diselesaikan, dan koin yang ditanam.", "Compare goals attempted, completed, and coins invested.");
        // Memperbarui `terms[”players.support.guide.debt”]` menggunakan tuple yang membawa bagian 1: ”Baca pinjaman, pembayaran, sisa saldo, dan penalti
        // bersama-sama.”; bagian 2: ”Read loans, repayments, remaining balance, and penalties together.” dalam AddPlayers.
        terms["players.support.guide.debt"] = ("Baca pinjaman, pembayaran, sisa saldo, dan penalti bersama-sama.", "Read loans, repayments, remaining balance, and penalties together.");
        // Memperbarui `terms[”players.support.guide.timeline”]` menggunakan tuple yang membawa bagian 1: ”Gunakan hari dan slot aksi untuk menemukan
        // keputusan terkait.”; bagian 2: ”Use the day and action slot to find the related decision.” dalam AddPlayers.
        terms["players.support.guide.timeline"] = ("Gunakan hari dan slot aksi untuk menemukan keputusan terkait.", "Use the day and action slot to find the related decision.");
        // Memperbarui `terms[”players.support.guide.outcome”]` menggunakan tuple yang membawa bagian 1: ”Cocokkan nilai ini dengan hasil akhir, peringkat,
        // dan poin kebahagiaan pemain.”; bagian 2: ”Compare this value with the player's final result, rank, and happiness points.” dalam AddPlayers.
        terms["players.support.guide.outcome"] = ("Cocokkan nilai ini dengan hasil akhir, peringkat, dan poin kebahagiaan pemain.", "Compare this value with the player's final result, rank, and happiness points.");
        // Memperbarui `terms[”players.support.guide.session_context”]` menggunakan tuple yang membawa bagian 1: ”Gunakan untuk memastikan hasil berasal
        // dari sesi dan pemain yang benar.”; bagian 2: ”Use this to confirm that the result belongs to the correct session and player.” dalam AddPlayers.
        terms["players.support.guide.session_context"] = ("Gunakan untuk memastikan hasil berasal dari sesi dan pemain yang benar.", "Use this to confirm that the result belongs to the correct session and player.");
        // Memperbarui `terms[”players.support.recommendation.not_applicable”]` menggunakan tuple yang membawa bagian 1: ”Jangan gunakan data ini untuk
        // menilai sesi dengan mode berbeda.”; bagian 2: ”Do not use this data to assess a session in another mode.” dalam AddPlayers.
        terms["players.support.recommendation.not_applicable"] = ("Jangan gunakan data ini untuk menilai sesi dengan mode berbeda.", "Do not use this data to assess a session in another mode.");
        // Memperbarui `terms[”players.support.recommendation.unavailable”]` menggunakan tuple yang membawa bagian 1: ”Lengkapi catatan permainan sebelum
        // mengambil kesimpulan.”; bagian 2: ”Complete the gameplay records before drawing a conclusion.” dalam AddPlayers.
        terms["players.support.recommendation.unavailable"] = ("Lengkapi catatan permainan sebelum mengambil kesimpulan.", "Complete the gameplay records before drawing a conclusion.");
        // Memperbarui `terms[”players.support.recommendation.baseline”]` menggunakan tuple yang membawa bagian 1: ”Gunakan Koin Awal sebagai pembanding.
        // Utamakan kebutuhan dan usahakan kas akhir...; bagian 2: ”Use Starting Coins as a comparison. Prioritize needs and try to keep ending cas... dalam
        // AddPlayers.
        terms["players.support.recommendation.baseline"] = ("Gunakan Koin Awal sebagai pembanding. Utamakan kebutuhan dan usahakan kas akhir tidak berkurang.", "Use Starting Coins as a comparison. Prioritize needs and try to keep ending cash from decreasing.");
        // Memperbarui `terms[”players.support.recommendation.income”]` menggunakan tuple yang membawa bagian 1: ”Usahakan pemasukan lebih besar dari
        // pengeluaran dan berasal dari beberapa sumbe...; bagian 2: ”Aim for income above spending and from several sources.” dalam AddPlayers.
        terms["players.support.recommendation.income"] = ("Usahakan pemasukan lebih besar dari pengeluaran dan berasal dari beberapa sumber.", "Aim for income above spending and from several sources.");
        // Memperbarui `terms[”players.support.recommendation.income_mix”]` menggunakan tuple yang membawa bagian 1: ”Tambahkan sumber pemasukan agar kas
        // tidak bergantung pada satu kegiatan.”; bagian 2: ”Add income sources so cash does not depend on one activity.” dalam AddPlayers.
        terms["players.support.recommendation.income_mix"] = ("Tambahkan sumber pemasukan agar kas tidak bergantung pada satu kegiatan.", "Add income sources so cash does not depend on one activity.");
        // Memperbarui `terms[”players.support.recommendation.spending”]` menggunakan tuple yang membawa bagian 1: ”Jaga pengeluaran di bawah pemasukan.
        // Dahulukan kebutuhan dan kewajiban.”; bagian 2: ”Keep spending below income. Prioritize needs and obligations.” dalam AddPlayers.
        terms["players.support.recommendation.spending"] = ("Jaga pengeluaran di bawah pemasukan. Dahulukan kebutuhan dan kewajiban.", "Keep spending below income. Prioritize needs and obligations.");
        // Memperbarui `terms[”players.support.recommendation.cash_position”]` menggunakan tuple yang membawa bagian 1: ”Jaga kas akhir tetap positif dan
        // hindari mengandalkan pinjaman yang belum lunas...; bagian 2: ”Keep ending cash positive and avoid relying on unpaid loans.” dalam AddPlayers.
        terms["players.support.recommendation.cash_position"] = ("Jaga kas akhir tetap positif dan hindari mengandalkan pinjaman yang belum lunas.", "Keep ending cash positive and avoid relying on unpaid loans.");
        // Memperbarui `terms[”players.support.recommendation.savings”]` menggunakan tuple yang membawa bagian 1: ”Sisihkan tabungan setelah kebutuhan
        // primer terpenuhi. Jangan berutang untuk pen...; bagian 2: ”Set aside savings after primary needs are met. Do not borrow for routine spendi...
        // dalam AddPlayers.
        terms["players.support.recommendation.savings"] = ("Sisihkan tabungan setelah kebutuhan primer terpenuhi. Jangan berutang untuk pengeluaran rutin.", "Set aside savings after primary needs are met. Do not borrow for routine spending.");
        // Memperbarui `terms[”players.support.recommendation.inventory”]` menggunakan tuple yang membawa bagian 1: ”Sediakan bahan sesuai kebutuhan pesanan
        // dan kurangi bahan terbuang.”; bagian 2: ”Keep enough ingredients for orders and reduce waste.” dalam AddPlayers.
        terms["players.support.recommendation.inventory"] = ("Sediakan bahan sesuai kebutuhan pesanan dan kurangi bahan terbuang.", "Keep enough ingredients for orders and reduce waste.");
        // Memperbarui `terms[”players.support.recommendation.business_activity”]` menggunakan tuple yang membawa bagian 1: ”Pastikan pendapatan pesanan
        // lebih besar daripada biaya bahan.”; bagian 2: ”Make sure order income is higher than ingredient costs.” dalam AddPlayers.
        terms["players.support.recommendation.business_activity"] = ("Pastikan pendapatan pesanan lebih besar daripada biaya bahan.", "Make sure order income is higher than ingredient costs.");
        // Memperbarui `terms[”players.support.recommendation.needs”]` menggunakan tuple yang membawa bagian 1: ”Penuhi kebutuhan primer lebih dahulu. Pilih
        // kebutuhan lain setelah kas aman.”; bagian 2: ”Meet primary needs first. Choose other needs after cash is secure.” dalam AddPlayers.
        terms["players.support.recommendation.needs"] = ("Penuhi kebutuhan primer lebih dahulu. Pilih kebutuhan lain setelah kas aman.", "Meet primary needs first. Choose other needs after cash is secure.");
        // Memperbarui `terms[”players.support.recommendation.donation”]` menggunakan tuple yang membawa bagian 1: ”Donasikan jumlah yang sesuai kemampuan
        // tanpa mengorbankan kebutuhan, tabungan, ...; bagian 2: ”Donate an affordable amount without sacrificing needs, savings, or debt repayme... dalam
        // AddPlayers.
        terms["players.support.recommendation.donation"] = ("Donasikan jumlah yang sesuai kemampuan tanpa mengorbankan kebutuhan, tabungan, atau pembayaran utang.", "Donate an affordable amount without sacrificing needs, savings, or debt repayment.");
        // Memperbarui `terms[”players.support.recommendation.gold_investment”]` menggunakan tuple yang membawa bagian 1: ”Jaga kas tetap aman. Nilai hasil
        // jual hanya untuk kartu yang sudah dijual; peri...; bagian 2: ”Keep cash secure. Sale proceeds cover only cards already sold; review cards sti...
        // dalam AddPlayers.
        terms["players.support.recommendation.gold_investment"] = ("Jaga kas tetap aman. Nilai hasil jual hanya untuk kartu yang sudah dijual; periksa kartu yang masih dimiliki secara terpisah.", "Keep cash secure. Sale proceeds cover only cards already sold; review cards still held separately.");
        // Memperbarui `terms[”players.support.recommendation.pension”]` menggunakan tuple yang membawa bagian 1: ”Bangun dana akhir sedikit demi sedikit
        // dan gunakan lebih dari satu jenis aset.”; bagian 2: ”Build the final fund gradually and use more than one asset type.” dalam AddPlayers.
        terms["players.support.recommendation.pension"] = ("Bangun dana akhir sedikit demi sedikit dan gunakan lebih dari satu jenis aset.", "Build the final fund gradually and use more than one asset type.");
        // Memperbarui `terms[”players.support.recommendation.risk”]` menggunakan tuple yang membawa bagian 1: ”Siapkan kas, asuransi, atau pilihan darurat
        // sebelum dampak kartu risiko harus d...; bagian 2: ”Prepare cash, insurance, or an emergency option before a risk-card effect must ... dalam
        // AddPlayers.
        terms["players.support.recommendation.risk"] = ("Siapkan kas, asuransi, atau pilihan darurat sebelum dampak kartu risiko harus diselesaikan.", "Prepare cash, insurance, or an emergency option before a risk-card effect must be resolved.");
        // Memperbarui `terms[”players.support.recommendation.protection”]` menggunakan tuple yang membawa bagian 1: ”Gunakan perlindungan untuk risiko
        // utama dengan biaya yang masih terjangkau.”; bagian 2: ”Use protection for major risks at an affordable cost.” dalam AddPlayers.
        terms["players.support.recommendation.protection"] = ("Gunakan perlindungan untuk risiko utama dengan biaya yang masih terjangkau.", "Use protection for major risks at an affordable cost.");
        // Memperbarui `terms[”players.support.recommendation.goals”]` menggunakan tuple yang membawa bagian 1: ”Pilih target yang jelas, realistis, dan
        // dapat didanai secara bertahap.”; bagian 2: ”Choose clear, realistic goals that can be funded gradually.” dalam AddPlayers.
        terms["players.support.recommendation.goals"] = ("Pilih target yang jelas, realistis, dan dapat didanai secara bertahap.", "Choose clear, realistic goals that can be funded gradually.");
        // Memperbarui `terms[”players.support.recommendation.debt”]` menggunakan tuple yang membawa bagian 1: ”Gunakan pinjaman untuk hal penting dan
        // lunasi sesuai kemampuan.”; bagian 2: ”Use loans for important needs and repay them within your means.” dalam AddPlayers.
        terms["players.support.recommendation.debt"] = ("Gunakan pinjaman untuk hal penting dan lunasi sesuai kemampuan.", "Use loans for important needs and repay them within your means.");
        // Memperbarui `terms[”players.support.recommendation.timeline”]` menggunakan tuple yang membawa bagian 1: ”Dahulukan kebutuhan, cadangan kas,
        // pemasukan, kewajiban, lalu target dan invest...; bagian 2: ”Prioritize needs, cash reserves, income, obligations, then goals and investment...
        // dalam AddPlayers.
        terms["players.support.recommendation.timeline"] = ("Dahulukan kebutuhan, cadangan kas, pemasukan, kewajiban, lalu target dan investasi.", "Prioritize needs, cash reserves, income, obligations, then goals and investments.");
        // Memperbarui `terms[”players.support.recommendation.planning”]` menggunakan tuple yang membawa bagian 1: ”Siapkan masa depan setelah kebutuhan
        // saat ini dan cadangan kas aman.”; bagian 2: ”Prepare for the future after current needs and cash reserves are secure.” dalam AddPlayers.
        terms["players.support.recommendation.planning"] = ("Siapkan masa depan setelah kebutuhan saat ini dan cadangan kas aman.", "Prepare for the future after current needs and cash reserves are secure.");
        // Memperbarui `terms[”players.support.recommendation.outcome”]` menggunakan tuple yang membawa bagian 1: ”Nilai hasil bersama kas, kebutuhan, dan
        // kewajiban. Poin Kebahagiaan tinggi belu...; bagian 2: ”Judge the result with cash, needs, and obligations. High happiness points do no... dalam
        // AddPlayers.
        terms["players.support.recommendation.outcome"] = ("Nilai hasil bersama kas, kebutuhan, dan kewajiban. Poin Kebahagiaan tinggi belum tentu berarti kondisi sehat.", "Judge the result with cash, needs, and obligations. High happiness points do not always mean a healthy position.");
        // Memperbarui `terms[”players.support.recommendation.session_context”]` menggunakan tuple yang membawa bagian 1: ”Gunakan data ini hanya untuk
        // memastikan sumber hasilnya benar.”; bagian 2: ”Use this data only to confirm the result's source.” dalam AddPlayers.
        terms["players.support.recommendation.session_context"] = ("Gunakan data ini hanya untuk memastikan sumber hasilnya benar.", "Use this data only to confirm the result's source.");
        // Memperbarui `terms[”players.support.recommendation.wealth”]` menggunakan tuple yang membawa bagian 1: ”Setelah kebutuhan dan kewajiban selesai,
        // usahakan kas akhir lebih besar dari Ko...; bagian 2: ”After needs and obligations are settled, aim for ending cash above Starting Coi... dalam
        // AddPlayers.
        terms["players.support.recommendation.wealth"] = ("Setelah kebutuhan dan kewajiban selesai, usahakan kas akhir lebih besar dari Koin Awal.", "After needs and obligations are settled, aim for ending cash above Starting Coins.");
        // Memperbarui `terms[”players.support.recommendation.cash_recover”]` menggunakan tuple yang membawa bagian 1: ”Kas berada di bawah Koin Awal.
        // Kurangi pengeluaran terbesar tanpa menambah pinj...; bagian 2: ”Cash is below Starting Coins. Reduce the largest spending without adding
        // loans.... dalam AddPlayers.
        terms["players.support.recommendation.cash_recover"] = ("Kas berada di bawah Koin Awal. Kurangi pengeluaran terbesar tanpa menambah pinjaman.", "Cash is below Starting Coins. Reduce the largest spending without adding loans.");
        // Memperbarui `terms[”players.support.recommendation.income_mix_recover”]` menggunakan tuple yang membawa bagian 1: ”Pemasukan masih bergantung
        // pada sedikit sumber. Tambahkan sumber lain yang sesu...; bagian 2: ”Income still depends on a few sources. Add another suitable source.” dalam
        // AddPlayers.
        terms["players.support.recommendation.income_mix_recover"] = ("Pemasukan masih bergantung pada sedikit sumber. Tambahkan sumber lain yang sesuai.", "Income still depends on a few sources. Add another suitable source.");
        // Memperbarui `terms[”players.support.recommendation.productive_spending”]` menggunakan tuple yang membawa bagian 1: ”Baca bersama Persentase Laba
        // Pesanan dan Bahan Tersisa untuk melihat apakah pem...; bagian 2: ”Read this with Order Profit Margin and Remaining Ingredients to see whether
        // ing... dalam AddPlayers.
        terms["players.support.recommendation.productive_spending"] = ("Baca bersama Persentase Laba Pesanan dan Bahan Tersisa untuk melihat apakah pembelian bahan sudah mendukung pesanan yang menghasilkan laba.", "Read this with Order Profit Margin and Remaining Ingredients to see whether ingredient purchases supported profitable orders.");
        // Memperbarui `terms[”players.support.recommendation.business_recover”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan belum menutup biaya
        // bahan. Kurangi bahan tersisa atau selesaikan leb...; bagian 2: ”Income has not covered ingredient costs. Reduce leftover ingredients or
        // complet... dalam AddPlayers.
        terms["players.support.recommendation.business_recover"] = ("Pendapatan belum menutup biaya bahan. Kurangi bahan tersisa atau selesaikan lebih banyak pesanan.", "Income has not covered ingredient costs. Reduce leftover ingredients or complete more orders.");
        // Memperbarui `terms[”players.support.recommendation.risk_reduce”]` menggunakan tuple yang membawa bagian 1: ”Risiko dan biayanya tinggi. Kurangi
        // risiko mahal dan siapkan perlindungan.”; bagian 2: ”Risk and its cost are high. Reduce costly risks and prepare protection.” dalam AddPlayers.
        terms["players.support.recommendation.risk_reduce"] = ("Risiko dan biayanya tinggi. Kurangi risiko mahal dan siapkan perlindungan.", "Risk and its cost are high. Reduce costly risks and prepare protection.");
        // Memperbarui `terms[”players.support.recommendation.debt_reduce”]` menggunakan tuple yang membawa bagian 1: ”Sisa pinjaman tinggi dibanding kas.
        // Dahulukan pembayaran dan hindari pinjaman b...; bagian 2: ”The remaining loan is high compared with cash. Prioritize repayment and avoid n...
        // dalam AddPlayers.
        terms["players.support.recommendation.debt_reduce"] = ("Sisa pinjaman tinggi dibanding kas. Dahulukan pembayaran dan hindari pinjaman baru.", "The remaining loan is high compared with cash. Prioritize repayment and avoid new loans.");
        // Memperbarui `terms[”players.support.recommendation.goals_focus”]` menggunakan tuple yang membawa bagian 1: ”Pilih satu target yang paling
        // realistis dan danai secara bertahap.”; bagian 2: ”Choose the most realistic goal and fund it gradually.” dalam AddPlayers.
        terms["players.support.recommendation.goals_focus"] = ("Pilih satu target yang paling realistis dan danai secara bertahap.", "Choose the most realistic goal and fund it gradually.");
        // Memperbarui `terms[”players.support.recommendation.action_income”]` menggunakan tuple yang membawa bagian 1: ”Aksi penghasil uang masih sedikit.
        // Tambahkan jika kas belum aman.”; bagian 2: ”There are few money-earning actions. Add more if cash is not secure.” dalam AddPlayers.
        terms["players.support.recommendation.action_income"] = ("Aksi penghasil uang masih sedikit. Tambahkan jika kas belum aman.", "There are few money-earning actions. Add more if cash is not secure.");
        // Memperbarui `terms[”players.support.recommendation.action_balance”]` menggunakan tuple yang membawa bagian 1: ”Sisakan aksi untuk kebutuhan,
        // kewajiban, dan rencana masa depan.”; bagian 2: ”Keep some actions for needs, obligations, and future plans.” dalam AddPlayers.
        terms["players.support.recommendation.action_balance"] = ("Sisakan aksi untuk kebutuhan, kewajiban, dan rencana masa depan.", "Keep some actions for needs, obligations, and future plans.");
        // Memperbarui `terms[”players.support.recommendation.orders_improve”]` menggunakan tuple yang membawa bagian 1: ”Sesuaikan pembelian bahan dengan
        // kebutuhan pesanan berikutnya.”; bagian 2: ”Match ingredient purchases to the next order's needs.” dalam AddPlayers.
        terms["players.support.recommendation.orders_improve"] = ("Sesuaikan pembelian bahan dengan kebutuhan pesanan berikutnya.", "Match ingredient purchases to the next order's needs.");
        // Memperbarui `terms[”players.support.recommendation.planning_build”]` menggunakan tuple yang membawa bagian 1: ”Sisihkan aksi untuk tabungan,
        // target, atau asuransi setelah kas dan kebutuhan a...; bagian 2: ”Use some actions for savings, goals, or insurance after cash and needs are
        // secu... dalam AddPlayers.
        terms["players.support.recommendation.planning_build"] = ("Sisihkan aksi untuk tabungan, target, atau asuransi setelah kas dan kebutuhan aman.", "Use some actions for savings, goals, or insurance after cash and needs are secure.");
        // Memperbarui `terms[”players.support.recommendation.needs_balance”]` menggunakan tuple yang membawa bagian 1: ”Kebutuhan masih menumpuk pada satu
        // tingkat. Lengkapi tingkat yang tertinggal se...; bagian 2: ”Needs are concentrated in one level. Complete the missing levels after cash is ...
        // dalam AddPlayers.
        terms["players.support.recommendation.needs_balance"] = ("Kebutuhan masih menumpuk pada satu tingkat. Lengkapi tingkat yang tertinggal setelah kas aman.", "Needs are concentrated in one level. Complete the missing levels after cash is secure.");
        // Memperbarui `terms[”players.support.recommendation.donation_stabilize”]` menggunakan tuple yang membawa bagian 1: ”Gunakan jumlah donasi yang
        // lebih tetap dan sesuai kemampuan.”; bagian 2: ”Use a more consistent and affordable donation amount.” dalam AddPlayers.
        terms["players.support.recommendation.donation_stabilize"] = ("Gunakan jumlah donasi yang lebih tetap dan sesuai kemampuan.", "Use a more consistent and affordable donation amount.");
        // Memperbarui `terms[”players.support.recommendation.outcome_recover”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan negatif. Periksa
        // penalti misi dan pinjaman lebih dahulu.”; bagian 2: ”Happiness Points are negative. Check mission and loan penalties first.” dalam AddPlayers.
        terms["players.support.recommendation.outcome_recover"] = ("Poin Kebahagiaan negatif. Periksa penalti misi dan pinjaman lebih dahulu.", "Happiness Points are negative. Check mission and loan penalties first.");
        // Memperbarui `terms[”players.support.recommendation.balance”]` menggunakan tuple yang membawa bagian 1: ”Jangan nilai hasil ini sendirian.
        // Cocokkan dengan kas, kebutuhan, risiko, dan k...; bagian 2: ”Do not judge this result alone. Compare it with cash, needs, risk, and obligati...
        // dalam AddPlayers.
        terms["players.support.recommendation.balance"] = ("Jangan nilai hasil ini sendirian. Cocokkan dengan kas, kebutuhan, risiko, dan kewajiban.", "Do not judge this result alone. Compare it with cash, needs, risk, and obligations.");
        // Memperbarui `terms[”players.support.guide.wealth_exceptional”]` menggunakan tuple yang membawa bagian 1: ”300% atau lebih: Koin Tersisa
        // setidaknya tiga kali Koin Awal. Ini perbandingan ...; bagian 2: ”300% or more: Remaining Coins is at least three times Starting Coins. This is
        // a... dalam AddPlayers.
        terms["players.support.guide.wealth_exceptional"] = ("Naik 200% atau lebih: Koin Tersisa setidaknya tiga kali Koin Awal.", "An increase of 200% or more: Remaining Coins are at least three times Starting Coins.");
        // Memperbarui `terms[”players.support.guide.wealth_strong”]` menggunakan tuple yang membawa bagian 1: ”200–300%: Koin Tersisa setidaknya dua kali
        // Koin Awal.”; bagian 2: ”200–300%: Remaining Coins is at least twice Starting Coins.” dalam AddPlayers.
        terms["players.support.guide.wealth_strong"] = ("Naik 100% hingga kurang dari 200%: Koin Tersisa setidaknya dua kali Koin Awal.", "An increase of 100% to less than 200%: Remaining Coins are at least twice Starting Coins.");
        // Memperbarui `terms[”players.support.guide.wealth_growing”]` menggunakan tuple yang membawa bagian 1: ”100–200%: Koin Tersisa sama dengan atau
        // lebih besar dari Koin Awal, tetapi belu...; bagian 2: ”100–200%: Remaining Coins is equal to or greater than Starting Coins, but less ... dalam
        // AddPlayers.
        terms["players.support.guide.wealth_growing"] = ("Naik lebih dari 0% dan kurang dari 100%: Koin Tersisa bertambah, tetapi belum dua kali Koin Awal.", "An increase of more than 0% and less than 100%: Remaining Coins have grown, but have not yet doubled.");
        terms["players.support.guide.wealth_unchanged"] = ("0%: Koin Tersisa sama dengan Koin Awal.", "0%: Remaining Coins equal Starting Coins.");
        // Memperbarui `terms[”players.support.guide.wealth_declining”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 100%: Koin Tersisa lebih kecil
        // dari Koin Awal.”; bagian 2: ”Below 100%: Remaining Coins is lower than Starting Coins.” dalam AddPlayers.
        terms["players.support.guide.wealth_declining"] = ("Nilai negatif: Koin Tersisa berkurang dari Koin Awal.", "A negative value: Remaining Coins have decreased from Starting Coins.");
        // Memperbarui `terms[”players.support.guide.income_diverse”]` menggunakan tuple yang membawa bagian 1: ”Pemasukan cukup tersebar di beberapa sumber
        // sehingga tidak bergantung pada satu...; bagian 2: ”Income is spread across several sources and does not depend on one activity.” dalam
        // AddPlayers.
        terms["players.support.guide.income_diverse"] = ("Pemasukan cukup tersebar di beberapa sumber sehingga tidak bergantung pada satu aktivitas.", "Income is spread across several sources and does not depend on one activity.");
        // Memperbarui `terms[”players.support.guide.income_mixed”]` menggunakan tuple yang membawa bagian 1: ”Pemasukan mulai tersebar, tetapi satu atau
        // dua sumber masih memberi kontribusi ...; bagian 2: ”Income has started to diversify, but one or two sources still provide the main ... dalam
        // AddPlayers.
        terms["players.support.guide.income_mixed"] = ("Pemasukan mulai tersebar, tetapi satu atau dua sumber masih memberi kontribusi utama.", "Income has started to diversify, but one or two sources still provide the main contribution.");
        // Memperbarui `terms[”players.support.guide.income_concentrated”]` menggunakan tuple yang membawa bagian 1: ”Pemasukan masih bergantung pada
        // sedikit sumber. Periksa sumber pendapatan terbe...; bagian 2: ”Income still depends on few sources. Check the largest income source.” dalam
        // AddPlayers.
        terms["players.support.guide.income_concentrated"] = ("Pemasukan masih bergantung pada sedikit sumber. Periksa sumber pendapatan terbesarnya.", "Income still depends on few sources. Check the largest income source.");
        // Memperbarui `terms[”players.support.guide.expense_efficiency”]` menggunakan tuple yang membawa bagian 1: ”Dari setiap 100 koin yang keluar, nilai
        // ini menunjukkan bagian untuk pembelian ...; bagian 2: ”For every 100 outgoing coins, this shows the share spent on ingredients. A high... dalam
        // AddPlayers.
        terms["players.support.guide.expense_efficiency"] = ("Dari setiap 100 koin yang keluar, nilai ini menunjukkan bagian untuk pembelian bahan. Nilai tinggi tidak otomatis berarti lebih untung; lihat Persentase Laba Pesanan.", "For every 100 outgoing coins, this shows the share spent on ingredients. A higher value does not automatically mean more profit; check Order Profit Margin.");
        // Memperbarui `terms[”players.support.guide.return_positive”]` menggunakan tuple yang membawa bagian 1: ”Nilai positif berarti kegiatan ini
        // menghasilkan keuntungan.”; bagian 2: ”A positive value means this activity produced a gain.” dalam AddPlayers.
        terms["players.support.guide.return_positive"] = ("Nilai positif berarti kegiatan ini menghasilkan keuntungan.", "A positive value means this activity produced a gain.");
        // Memperbarui `terms[”players.support.guide.return_negative”]` menggunakan tuple yang membawa bagian 1: ”Nilai negatif berarti biaya lebih besar
        // daripada hasil yang diterima.”; bagian 2: ”A negative value means costs were higher than returns.” dalam AddPlayers.
        terms["players.support.guide.return_negative"] = ("Nilai negatif berarti biaya lebih besar daripada hasil yang diterima.", "A negative value means costs were higher than returns.");
        // Memperbarui `terms[”players.support.guide.return_even”]` menggunakan tuple yang membawa bagian 1: ”Nilai nol berarti tidak ada keuntungan maupun
        // kerugian bersih.”; bagian 2: ”Zero means there was no net gain or loss.” dalam AddPlayers.
        terms["players.support.guide.return_even"] = ("Nilai nol berarti tidak ada keuntungan maupun kerugian bersih.", "Zero means there was no net gain or loss.");
        // Memperbarui `terms[”players.support.guide.business_efficient”]` menggunakan tuple yang membawa bagian 1: ”Di atas 2: setiap koin biaya bahan
        // menghasilkan lebih dari dua koin pendapatan....; bagian 2: ”Above 2: each coin spent on ingredients produced more than two coins of income....
        // dalam AddPlayers.
        terms["players.support.guide.business_efficient"] = ("Di atas 2: setiap koin biaya bahan menghasilkan lebih dari dua koin pendapatan.", "Above 2: each coin spent on ingredients produced more than two coins of income.");
        // Memperbarui `terms[”players.support.guide.business_inefficient”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 1,5: hasil pesanan masih
        // rendah dibandingkan biaya bahan.”; bagian 2: ”Below 1.5: order returns are still low compared with ingredient costs.” dalam AddPlayers.
        terms["players.support.guide.business_inefficient"] = ("Di bawah 1,5: hasil pesanan masih rendah dibandingkan biaya bahan.", "Below 1.5: order returns are still low compared with ingredient costs.");
        // Memperbarui `terms[”players.support.guide.business_moderate”]` menggunakan tuple yang membawa bagian 1: ”Antara 1,5–2: usaha menghasilkan
        // pemasukan, tetapi efisiensinya masih sedang.”; bagian 2: ”Between 1.5–2: the business generated income, but efficiency is still moderate....
        // dalam AddPlayers.
        terms["players.support.guide.business_moderate"] = ("Antara 1,5–2: usaha menghasilkan pemasukan, tetapi efisiensinya masih sedang.", "Between 1.5–2: the business generated income, but efficiency is still moderate.");
        // Memperbarui `terms[”players.support.guide.risk_high”]` menggunakan tuple yang membawa bagian 1: ”Di atas 30%: biaya risiko mengambil bagian besar
        // dari pemasukan.”; bagian 2: ”Above 30%: risk costs consumed a large share of income.” dalam AddPlayers.
        terms["players.support.guide.risk_high"] = ("Di atas 30%: biaya risiko mengambil bagian besar dari pemasukan.", "Above 30%: risk costs consumed a large share of income.");
        // Memperbarui `terms[”players.support.guide.risk_low”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 10%: dampak biaya risiko terhadap
        // pemasukan relatif kecil.”; bagian 2: ”Below 10%: risk costs had a relatively small impact on income.” dalam AddPlayers.
        terms["players.support.guide.risk_low"] = ("Di bawah 10%: dampak biaya risiko terhadap pemasukan relatif kecil.", "Below 10%: risk costs had a relatively small impact on income.");
        // Memperbarui `terms[”players.support.guide.risk_moderate”]` menggunakan tuple yang membawa bagian 1: ”10–30%: dampak risiko berada pada tingkat
        // menengah.”; bagian 2: ”10–30%: risk impact is at a moderate level.” dalam AddPlayers.
        terms["players.support.guide.risk_moderate"] = ("10–30%: dampak risiko berada pada tingkat menengah.", "10–30%: risk impact is at a moderate level.");
        // Memperbarui `terms[”players.support.guide.risk_readiness_complete”]` menggunakan tuple yang membawa bagian 1: ”100%: seluruh risiko diselesaikan
        // tanpa Tindakan Darurat.”; bagian 2: ”100%: every risk was resolved without an Emergency Action.” dalam AddPlayers.
        terms["players.support.guide.risk_readiness_complete"] = ("100%: seluruh risiko diselesaikan tanpa Tindakan Darurat.", "100%: every risk was resolved without an Emergency Action.");
        // Memperbarui `terms[”players.support.guide.risk_readiness_most”]` menggunakan tuple yang membawa bagian 1: ”50–99%: sebagian besar risiko
        // diselesaikan tanpa Tindakan Darurat.”; bagian 2: ”50–99%: most risks were resolved without an Emergency Action.” dalam AddPlayers.
        terms["players.support.guide.risk_readiness_most"] = ("50% hingga kurang dari 100%: sedikitnya separuh risiko sudah selesai tanpa Tindakan Darurat. Sisanya memakai Tindakan Darurat atau belum selesai.", "50% to less than 100%: at least half of risks were resolved without an Emergency Action. The rest used an Emergency Action or are still unresolved.");
        // Memperbarui `terms[”players.support.guide.risk_readiness_limited”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 50%: lebih dari separuh
        // risiko memerlukan Tindakan Darurat.”; bagian 2: ”Below 50%: more than half of risks required an Emergency Action.” dalam AddPlayers.
        terms["players.support.guide.risk_readiness_limited"] = ("Di bawah 50%: kurang dari separuh risiko tercatat selesai tanpa Tindakan Darurat. Sisanya memakai Tindakan Darurat atau masih menunggu penyelesaian.", "Below 50%: fewer than half of risks are recorded as resolved without an Emergency Action. The rest used an Emergency Action or are still awaiting resolution.");
        // Memperbarui `terms[”players.support.guide.appetite_cautious”]` menggunakan tuple yang membawa bagian 1: ”0–25: gabungan frekuensi penerimaan dan
        // biaya risiko relatif rendah.”; bagian 2: ”0–25: the combination of acceptance frequency and risk cost is relatively low.” dalam AddPlayers.
        terms["players.support.guide.appetite_cautious"] = ("0–25: gabungan frekuensi penerimaan dan biaya risiko relatif rendah.", "0–25: the combination of acceptance frequency and risk cost is relatively low.");
        // Memperbarui `terms[”players.support.guide.appetite_balanced”]` menggunakan tuple yang membawa bagian 1: ”25–75: gabungan frekuensi penerimaan dan
        // biaya risiko berada pada tingkat menen...; bagian 2: ”25–75: the combination of acceptance frequency and risk cost is moderate.” dalam
        // AddPlayers.
        terms["players.support.guide.appetite_balanced"] = ("25–75: gabungan frekuensi penerimaan dan biaya risiko berada pada tingkat menengah.", "25–75: the combination of acceptance frequency and risk cost is moderate.");
        // Memperbarui `terms[”players.support.guide.appetite_high”]` menggunakan tuple yang membawa bagian 1: ”Di atas 75: pemain sering menerima risiko
        // berbiaya besar dibandingkan modal awa...; bagian 2: ”Above 75: the player often accepted risks with high costs relative to starting ... dalam
        // AddPlayers.
        terms["players.support.guide.appetite_high"] = ("Di atas 75: pemain sering menerima risiko berbiaya besar dibandingkan modal awal.", "Above 75: the player often accepted risks with high costs relative to starting capital.");
        // Memperbarui `terms[”players.support.guide.debt_low”]` menggunakan tuple yang membawa bagian 1: ”0–25%: penggunaan utang relatif rendah
        // dibandingkan kondisi keuangan pemain.”; bagian 2: ”0–25%: debt usage is relatively low compared with the player's finances.” dalam AddPlayers.
        terms["players.support.guide.debt_low"] = ("0–25%: penggunaan utang relatif rendah dibandingkan kondisi keuangan pemain.", "0–25%: debt usage is relatively low compared with the player's finances.");
        // Memperbarui `terms[”players.support.guide.debt_moderate”]` menggunakan tuple yang membawa bagian 1: ”25–75%: pinjaman digunakan dalam tingkat
        // menengah dan perlu dipantau bersama pe...; bagian 2: ”25–75%: loans were used at a moderate level and should be reviewed with repayme... dalam
        // AddPlayers.
        terms["players.support.guide.debt_moderate"] = ("25–75%: pinjaman digunakan dalam tingkat menengah dan perlu dipantau bersama pelunasannya.", "25–75%: loans were used at a moderate level and should be reviewed with repayment.");
        // Memperbarui `terms[”players.support.guide.debt_high”]` menggunakan tuple yang membawa bagian 1: ”Di atas 75%: beban pinjaman tinggi dibandingkan
        // kondisi keuangan pemain.”; bagian 2: ”Above 75%: the loan burden is high compared with the player's finances.” dalam AddPlayers.
        terms["players.support.guide.debt_high"] = ("Di atas 75%: beban pinjaman tinggi dibandingkan kondisi keuangan pemain.", "Above 75%: the loan burden is high compared with the player's finances.");
        // Memperbarui `terms[”players.support.guide.goals_complete”]` menggunakan tuple yang membawa bagian 1: ”100%: kebutuhan dana seluruh target yang
        // dicoba sudah terpenuhi. Sisa tabungan ...; bagian 2: ”100%: the funding requirement of every attempted goal has been met. Remaining s... dalam
        // AddPlayers.
        terms["players.support.guide.goals_complete"] = ("100%: kebutuhan dana seluruh target yang dicoba sudah terpenuhi. Sisa tabungan ditampilkan terpisah.", "100%: the funding requirement of every attempted goal has been met. Remaining savings are shown separately.");
        // Memperbarui `terms[”players.support.guide.goals_strong”]` menggunakan tuple yang membawa bagian 1: ”67–99%: setidaknya dua pertiga biaya target
        // yang dicoba telah didanai.”; bagian 2: ”67–99%: at least two thirds of the attempted goal cost has been funded.” dalam AddPlayers.
        terms["players.support.guide.goals_strong"] = ("67–99%: setidaknya dua pertiga biaya target yang dicoba telah didanai.", "67–99%: at least two thirds of the attempted goal cost has been funded.");
        // Memperbarui `terms[”players.support.guide.goals_moderate”]` menggunakan tuple yang membawa bagian 1: ”34–67%: sebagian biaya target yang dicoba
        // telah didanai, tetapi belum mencapai ...; bagian 2: ”34–67%: some attempted goal cost has been funded, but less than two thirds.” dalam
        // AddPlayers.
        terms["players.support.guide.goals_moderate"] = ("34–67%: sebagian biaya target yang dicoba telah didanai, tetapi belum mencapai dua pertiga.", "34–67%: some attempted goal cost has been funded, but less than two thirds.");
        // Memperbarui `terms[”players.support.guide.goals_limited”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 34%: kurang dari sepertiga biaya
        // target yang dicoba telah didanai.”; bagian 2: ”Below 34%: less than one third of the attempted goal cost has been funded.” dalam AddPlayers.
        terms["players.support.guide.goals_limited"] = ("Di bawah 34%: kurang dari sepertiga biaya target yang dicoba telah didanai.", "Below 34%: less than one third of the attempted goal cost has been funded.");
        // Memperbarui `terms[”players.support.guide.loan_repaid”]` menggunakan tuple yang membawa bagian 1: ”Mendekati 100% berarti seluruh pinjaman
        // berhasil dilunasi.”; bagian 2: ”Close to 100% means all loans were successfully repaid.” dalam AddPlayers.
        terms["players.support.guide.loan_repaid"] = ("Mendekati 100% berarti seluruh pinjaman berhasil dilunasi.", "Close to 100% means all loans were successfully repaid.");
        // Memperbarui `terms[”players.support.guide.loan_remaining”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 100% berarti masih ada pinjaman
        // yang belum sepenuhnya dilunasi.”; bagian 2: ”Below 100% means some loans were not fully repaid.” dalam AddPlayers.
        terms["players.support.guide.loan_remaining"] = ("Di bawah 100% berarti masih ada pinjaman yang belum sepenuhnya dilunasi.", "Below 100% means some loans were not fully repaid.");
        // Memperbarui `terms[”players.support.guide.action_income”]` menggunakan tuple yang membawa bagian 1: ”Di atas 60%: sebagian besar aksi diarahkan
        // untuk membangun pemasukan.”; bagian 2: ”Above 60%: most actions were directed toward building income.” dalam AddPlayers.
        terms["players.support.guide.action_income"] = ("Di atas 60%: sebagian besar aksi diarahkan untuk membangun pemasukan.", "Above 60%: most actions were directed toward building income.");
        // Memperbarui `terms[”players.support.guide.action_exploration”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 40%: sebagian besar aksi
        // digunakan untuk kegiatan yang tidak langsung ...; bagian 2: ”Below 40%: most actions were used for activities that did not directly generate...
        // dalam AddPlayers.
        terms["players.support.guide.action_exploration"] = ("Di bawah 40%: sebagian besar aksi digunakan untuk kegiatan yang tidak langsung menghasilkan koin masuk.", "Below 40%: most actions were used for activities that did not directly generate incoming coins.");
        // Memperbarui `terms[”players.support.guide.action_balanced”]` menggunakan tuple yang membawa bagian 1: ”40–60%: aksi pemasukan dan aksi lainnya
        // relatif seimbang.”; bagian 2: ”40–60%: income actions and other actions were relatively balanced.” dalam AddPlayers.
        terms["players.support.guide.action_balanced"] = ("40–60%: aksi pemasukan dan aksi lainnya relatif seimbang.", "40–60%: income actions and other actions were relatively balanced.");
        // Memperbarui `terms[”players.support.guide.orders_strong”]` menggunakan tuple yang membawa bagian 1: ”80% atau lebih: perencanaan bahan mendukung
        // keberhasilan sebagian besar pesanan...; bagian 2: ”80% or more: ingredient planning supported success on most orders.” dalam AddPlayers.
        terms["players.support.guide.orders_strong"] = ("80% atau lebih: perencanaan bahan mendukung keberhasilan sebagian besar pesanan.", "80% or more: ingredient planning supported success on most orders.");
        // Memperbarui `terms[”players.support.guide.orders_review”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 60%: banyak kesempatan pesanan
        // belum berhasil dimanfaatkan.”; bagian 2: ”Below 60%: many order opportunities were not successfully used.” dalam AddPlayers.
        terms["players.support.guide.orders_review"] = ("Di bawah 60%: banyak kesempatan pesanan belum berhasil dimanfaatkan.", "Below 60%: many order opportunities were not successfully used.");
        // Memperbarui `terms[”players.support.guide.orders_moderate”]` menggunakan tuple yang membawa bagian 1: ”60–80%: keberhasilan pesanan berada pada
        // tingkat menengah.”; bagian 2: ”60–80%: order success is at a moderate level.” dalam AddPlayers.
        terms["players.support.guide.orders_moderate"] = ("60–80%: keberhasilan pesanan berada pada tingkat menengah.", "60–80%: order success is at a moderate level.");
        // Memperbarui `terms[”players.support.guide.planning_long”]` menggunakan tuple yang membawa bagian 1: ”Di atas 40%: banyak keputusan diarahkan
        // untuk tujuan jangka panjang.”; bagian 2: ”Above 40%: many decisions were directed toward long-term goals.” dalam AddPlayers.
        terms["players.support.guide.planning_long"] = ("Di atas 40%: lebih dari empat dari setiap sepuluh aksi yang digunakan dipakai untuk tabungan, target, asuransi, atau pelunasan.", "Above 40%: more than four out of every ten actions used went toward savings, goals, insurance, or repayment.");
        // Memperbarui `terms[”players.support.guide.planning_short”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 20%: keputusan lebih berfokus pada
        // kebutuhan saat ini.”; bagian 2: ”Below 20%: decisions focused more on current needs.” dalam AddPlayers.
        terms["players.support.guide.planning_short"] = ("Di bawah 20%: kurang dari dua dari setiap sepuluh aksi yang digunakan dipakai untuk tabungan, target, asuransi, atau pelunasan.", "Below 20%: fewer than two out of every ten actions used went toward savings, goals, insurance, or repayment.");
        // Memperbarui `terms[”players.support.guide.planning_balanced”]` menggunakan tuple yang membawa bagian 1: ”20–40%: keputusan jangka pendek dan
        // jangka panjang cukup seimbang.”; bagian 2: ”20–40%: short- and long-term decisions were fairly balanced.” dalam AddPlayers.
        terms["players.support.guide.planning_balanced"] = ("20–40%: dua sampai empat dari setiap sepuluh aksi yang digunakan dipakai untuk tabungan, target, asuransi, atau pelunasan.", "20–40%: two to four out of every ten actions used went toward savings, goals, insurance, or repayment.");
        // Memperbarui `terms[”players.support.guide.need_balance_high”]` menggunakan tuple yang membawa bagian 1: ”67% atau lebih: kepemilikan kebutuhan
        // cukup merata antara tingkat primer, sekun...; bagian 2: ”67% or more: need ownership is fairly balanced across primary, secondary, and t... dalam
        // AddPlayers.
        terms["players.support.guide.need_balance_high"] = ("67% atau lebih: kepemilikan kebutuhan cukup merata antara tingkat primer, sekunder, dan tersier.", "67% or more: need ownership is fairly balanced across primary, secondary, and tertiary tiers.");
        // Memperbarui `terms[”players.support.guide.need_balance_moderate”]` menggunakan tuple yang membawa bagian 1: ”34–67%: kebutuhan mulai tersebar,
        // tetapi satu tingkat masih lebih dominan.”; bagian 2: ”34–67%: needs are becoming distributed, but one tier remains more dominant.” dalam
        // AddPlayers.
        terms["players.support.guide.need_balance_moderate"] = ("34–67%: kebutuhan mulai tersebar, tetapi satu tingkat masih lebih dominan.", "34–67%: needs are becoming distributed, but one tier remains more dominant.");
        // Memperbarui `terms[”players.support.guide.need_balance_low”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 34%: kepemilikan kebutuhan
        // sangat terkonsentrasi pada satu tingkat.”; bagian 2: ”Below 34%: need ownership is highly concentrated in one tier.” dalam AddPlayers.
        terms["players.support.guide.need_balance_low"] = ("Di bawah 34%: kepemilikan kebutuhan sangat terkonsentrasi pada satu tingkat.", "Below 34%: need ownership is highly concentrated in one tier.");
        // Memperbarui `terms[”players.support.guide.growth_strong”]` menggunakan tuple yang membawa bagian 1: ”Lebih dari 3 kali modal awal menunjukkan
        // pertumbuhan uang yang kuat.”; bagian 2: ”More than three times starting capital shows strong money growth.” dalam AddPlayers.
        terms["players.support.guide.growth_strong"] = ("Lebih dari 3 kali modal awal menunjukkan pertumbuhan uang yang kuat.", "More than three times starting capital shows strong money growth.");
        // Memperbarui `terms[”players.support.guide.growth_declining”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 1 kali modal awal berarti uang
        // akhir lebih kecil daripada modal awal.”; bagian 2: ”Below one times starting capital means ending money is lower than starting capi... dalam
        // AddPlayers.
        terms["players.support.guide.growth_declining"] = ("Di bawah 1 kali modal awal berarti uang akhir lebih kecil daripada modal awal.", "Below one times starting capital means ending money is lower than starting capital.");
        // Memperbarui `terms[”players.support.guide.growth_steady”]` menggunakan tuple yang membawa bagian 1: ”Antara 1–3 kali modal awal menunjukkan
        // pertumbuhan bertahap.”; bagian 2: ”Between one and three times starting capital shows gradual growth.” dalam AddPlayers.
        terms["players.support.guide.growth_steady"] = ("Antara 1–3 kali modal awal menunjukkan pertumbuhan bertahap.", "Between one and three times starting capital shows gradual growth.");
        // Memperbarui `terms[”players.support.guide.donation_aggressive”]` menggunakan tuple yang membawa bagian 1: ”Di atas 30%: pemain memberi porsi
        // besar dari kekayaannya untuk donasi.”; bagian 2: ”Above 30%: the player gave a large share of wealth to donations.” dalam AddPlayers.
        terms["players.support.guide.donation_aggressive"] = ("Di atas 30%: pemain memberi porsi besar dari kekayaannya untuk donasi.", "Above 30%: the player gave a large share of wealth to donations.");
        // Memperbarui `terms[”players.support.guide.donation_conservative”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 10%: pemain memberi porsi
        // kecil dari kekayaannya untuk donasi.”; bagian 2: ”Below 10%: the player gave a small share of wealth to donations.” dalam AddPlayers.
        terms["players.support.guide.donation_conservative"] = ("Di bawah 10%: pemain memberi porsi kecil dari kekayaannya untuk donasi.", "Below 10%: the player gave a small share of wealth to donations.");
        // Memperbarui `terms[”players.support.guide.donation_moderate”]` menggunakan tuple yang membawa bagian 1: ”10–30%: porsi donasi berada pada tingkat
        // menengah.”; bagian 2: ”10–30%: the donation share is at a moderate level.” dalam AddPlayers.
        terms["players.support.guide.donation_moderate"] = ("10–30%: porsi donasi berada pada tingkat menengah.", "10–30%: the donation share is at a moderate level.");
        // Memperbarui `terms[”players.support.guide.donation_stable”]` menggunakan tuple yang membawa bagian 1: ”Nilai mendekati nol berarti jumlah donasi
        // antar-Jumat sangat konsisten.”; bagian 2: ”A value close to zero means Friday donation amounts were very consistent.” dalam AddPlayers.
        terms["players.support.guide.donation_stable"] = ("Nilai mendekati nol berarti jumlah donasi antar-Jumat sangat konsisten.", "A value close to zero means Friday donation amounts were very consistent.");
        // Memperbarui `terms[”players.support.guide.donation_variable”]` menggunakan tuple yang membawa bagian 1: ”Semakin besar nilainya, semakin
        // berubah-ubah jumlah donasi antar-Jumat.”; bagian 2: ”The larger the value, the more Friday donation amounts varied.” dalam AddPlayers.
        terms["players.support.guide.donation_variable"] = ("Semakin besar nilainya, semakin berubah-ubah jumlah donasi antar-Jumat.", "The larger the value, the more Friday donation amounts varied.");
        // Memperbarui `terms[”players.support.guide.donation_commitment_strong”]` menggunakan tuple yang membawa bagian 1: ”67 atau lebih: nominal relatif
        // konsisten, proporsional terhadap kas, dan partis...; bagian 2: ”67 or more: amounts were relatively consistent, proportional to cash, and
        // Frida... dalam AddPlayers.
        terms["players.support.guide.donation_commitment_strong"] = ("67 atau lebih: nominal relatif konsisten, proporsional terhadap kas, dan partisipasi Jumat tinggi.", "67 or more: amounts were relatively consistent, proportional to cash, and Friday participation was high.");
        // Memperbarui `terms[”players.support.guide.donation_commitment_moderate”]` menggunakan tuple yang membawa bagian 1: ”34–67: konsistensi, proporsi,
        // atau partisipasi donasi masih berada pada tingkat...; bagian 2: ”34–67: donation consistency, proportion, or participation remained moderate.”
        // dalam AddPlayers.
        terms["players.support.guide.donation_commitment_moderate"] = ("34–67: konsistensi, proporsi, atau partisipasi donasi masih berada pada tingkat menengah.", "34–67: donation consistency, proportion, or participation remained moderate.");
        // Memperbarui `terms[”players.support.guide.donation_commitment_weak”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 34: konsistensi,
        // proporsi, atau partisipasi donasi masih rendah.”; bagian 2: ”Below 34: donation consistency, proportion, or participation remained low.” dalam
        // AddPlayers.
        terms["players.support.guide.donation_commitment_weak"] = ("Di bawah 34: konsistensi, proporsi, atau partisipasi donasi masih rendah.", "Below 34: donation consistency, proportion, or participation remained low.");
        // Memperbarui `terms[”players.support.guide.mission_complete”]` menggunakan tuple yang membawa bagian 1: ”Nilai 1 berarti syarat misi pemain
        // tercapai.”; bagian 2: ”A value of 1 means the player's mission requirements were achieved.” dalam AddPlayers.
        terms["players.support.guide.mission_complete"] = ("Nilai 1 berarti syarat misi pemain tercapai.", "A value of 1 means the player's mission requirements were achieved.");
        // Memperbarui `terms[”players.support.guide.mission_incomplete”]` menggunakan tuple yang membawa bagian 1: ”Nilai 0 berarti syarat misi pemain
        // belum tercapai.”; bagian 2: ”A value of 0 means the player's mission requirements were not achieved.” dalam AddPlayers.
        terms["players.support.guide.mission_incomplete"] = ("Nilai 0 berarti syarat misi pemain belum tercapai.", "A value of 0 means the player's mission requirements were not achieved.");
        // Memperbarui `terms[”players.support.guide.participation_high”]` menggunakan tuple yang membawa bagian 1: ”Pemain berdonasi pada hampir semua
        // kesempatan hari Jumat.”; bagian 2: ”The player donated on nearly every Friday opportunity.” dalam AddPlayers.
        terms["players.support.guide.participation_high"] = ("Pemain berdonasi pada hampir semua kesempatan hari Jumat.", "The player donated on nearly every Friday opportunity.");
        // Memperbarui `terms[”players.support.guide.participation_partial”]` menggunakan tuple yang membawa bagian 1: ”Pemain hanya berdonasi pada sebagian
        // kesempatan hari Jumat.”; bagian 2: ”The player donated on only some Friday opportunities.” dalam AddPlayers.
        terms["players.support.guide.participation_partial"] = ("Pemain hanya berdonasi pada sebagian kesempatan hari Jumat.", "The player donated on only some Friday opportunities.");
        // Memperbarui `terms[”players.support.formula.net_worth”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa dibanding Koin Awal = Koin
        // Tersisa ÷ Koin Awal × 100%. Nilai 100% ...; bagian 2: ”Remaining Coins Compared with Starting Coins = Remaining Coins ÷ Starting Coins... dalam
        // AddPlayers.
        terms["players.support.formula.net_worth"] = ("Perubahan Koin dari Awal = (Koin Tersisa − Koin Awal) ÷ Koin Awal × 100%. Contoh: 10 menjadi 12 berarti +20%; 10 menjadi 8 berarti -20%.", "Coin Change from Start = (Remaining Coins − Starting Coins) ÷ Starting Coins × 100%. For example: 10 to 12 means +20%; 10 to 8 means -20%.");
        // Memperbarui `terms[”players.support.formula.income_diversification”]` menggunakan tuple yang membawa bagian 1: ”Rasio Diversifikasi Pendapatan =
        // (Pendapatan Pekerjaan Bebas + Pendapatan Pesan...; bagian 2: ”Income Diversification Ratio = (Freelance Income + Meal Income + Gold Income + ...
        // dalam AddPlayers.
        terms["players.support.formula.income_diversification"] = ("Rasio Diversifikasi Pendapatan = (Pendapatan Pekerjaan Bebas + Pendapatan Pesanan Makanan + Pendapatan Emas + Donasi Diterima) ÷ Total Pendapatan × 100%. Indeks Diversifikasi Pendapatan menunjukkan pemerataan antar-sumber.", "Income Diversification Ratio = (Freelance Income + Meal Income + Gold Income + Donations Received) ÷ Total Income × 100%. Income Diversification Index shows balance across sources.");
        // Memperbarui `terms[”players.support.formula.income_diversification_index”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan Sumber
        // Pendapatan = [1 − jumlah kuadrat setiap (Persentase Pendapata...; bagian 2: ”Income Source Balance = [1 − the sum of squared (Income Share by
        // Source ÷ 100)]... dalam AddPlayers.
        terms["players.support.formula.income_diversification_index"] = ("Pemerataan Sumber Pendapatan = [1 − jumlah kuadrat setiap (Persentase Pendapatan per Sumber ÷ 100)] ÷ [1 − (1 ÷ Jumlah Sumber Pendapatan Aktif)] × 100%. Satu sumber aktif menghasilkan 0%; tanpa pendapatan, nilai belum dapat dihitung.", "Income Source Balance = [1 − the sum of squared (Income Share by Source ÷ 100)] ÷ [1 − (1 ÷ Active Income Source Count)] × 100%. One active source gives 0%; with no income, the value cannot be calculated.");
        // Memperbarui `terms[”players.support.formula.expense_efficiency”]` menggunakan tuple yang membawa bagian 1: ”Persentase Pengeluaran untuk Membeli
        // Bahan = Total Biaya Pembelian Bahan ÷ Tota...; bagian 2: ”Ingredient Purchase Share of Spending = Total Ingredient Purchase Cost ÷ Total ...
        // dalam AddPlayers.
        terms["players.support.formula.expense_efficiency"] = ("Persentase Pengeluaran untuk Membeli Bahan = Total Biaya Pembelian Bahan ÷ Total Koin Keluar × 100%.", "Ingredient Purchase Share of Spending = Total Ingredient Purchase Cost ÷ Total Outgoing Coins × 100%.");
        // Memperbarui `terms[”players.support.formula.business_margin”]` menggunakan tuple yang membawa bagian 1: ”Persentase Laba Pesanan = (Total
        // Pendapatan Pesanan − Biaya Bahan Terpakai) ÷ T...; bagian 2: ”Order Profit Margin = (Total Order Income − Used Ingredient Cost) ÷ Total Order...
        // dalam AddPlayers.
        terms["players.support.formula.business_margin"] = ("Persentase Laba Pesanan = (Total Pendapatan Pesanan − Biaya Bahan Terpakai) ÷ Total Pendapatan Pesanan × 100%.", "Order Profit Margin = (Total Order Income − Used Ingredient Cost) ÷ Total Order Income × 100%.");
        // Memperbarui `terms[”players.support.formula.business_efficiency”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan pesanan ÷ biaya bahan.”;
        // bagian 2: ”Order income ÷ ingredient costs.” dalam AddPlayers.
        terms["players.support.formula.business_efficiency"] = ("Pendapatan pesanan ÷ biaya bahan.", "Order income ÷ ingredient costs.");
        // Memperbarui `terms[”players.support.formula.gold_roi”]` menggunakan tuple yang membawa bagian 1: ”(Hasil penjualan emas − biaya pembelian emas) ÷
        // biaya pembelian × 100%.”; bagian 2: ”(Gold sale proceeds − gold purchase costs) ÷ purchase costs × 100%.” dalam AddPlayers.
        terms["players.support.formula.gold_roi"] = ("(Hasil penjualan emas − biaya pembelian emas) ÷ biaya pembelian × 100%.", "(Gold sale proceeds − gold purchase costs) ÷ purchase costs × 100%.");
        // Memperbarui `terms[”players.support.formula.risk_exposure”]` menggunakan tuple yang membawa bagian 1: ”Total biaya risiko ÷ total pemasukan ×
        // 100%.”; bagian 2: ”Total risk costs ÷ total income × 100%.” dalam AddPlayers.
        terms["players.support.formula.risk_exposure"] = ("Total biaya risiko ÷ total pemasukan × 100%.", "Total risk costs ÷ total income × 100%.");
        // Memperbarui `terms[”players.support.formula.risk_mitigation”]` menggunakan tuple yang membawa bagian 1: ”Risiko yang ditangani asuransi ÷ seluruh
        // risiko yang terjadi × 100%.”; bagian 2: ”Risks covered by insurance ÷ all risks encountered × 100%.” dalam AddPlayers.
        terms["players.support.formula.risk_mitigation"] = ("Risiko yang ditangani asuransi ÷ seluruh risiko yang terjadi × 100%.", "Risks covered by insurance ÷ all risks encountered × 100%.");
        // Memperbarui `terms[”players.support.formula.risk_appetite”]` menggunakan tuple yang membawa bagian 1: ”Persentase Risiko Selesai tanpa Tindakan
        // Darurat = Risiko Selesai tanpa Tindaka...; bagian 2: ”Share of Risks Resolved without Emergency Action = Risks Resolved without Emerg... dalam
        // AddPlayers.
        terms["players.support.formula.risk_appetite"] = ("Persentase Risiko Selesai tanpa Tindakan Darurat = Risiko Selesai tanpa Tindakan Darurat ÷ Kartu Risiko Kehidupan yang Muncul × 100%.", "Share of Risks Resolved without Emergency Action = Risks Resolved without Emergency Action ÷ Life Risk Cards Drawn × 100%.");
        // Memperbarui `terms[”players.support.formula.debt_leverage”]` menggunakan tuple yang membawa bagian 1: ”Porsi Sisa Pinjaman = Sisa Pinjaman ÷
        // (Sisa Pinjaman + Total Koin Tersisa dan T...; bagian 2: ”Outstanding Loan Share = Outstanding Loan ÷ (Outstanding Loan + Remaining Coins... dalam
        // AddPlayers.
        terms["players.support.formula.debt_leverage"] = ("Porsi Sisa Pinjaman = Sisa Pinjaman ÷ (Sisa Pinjaman + Total Koin Tersisa dan Tabungan) × 100%.", "Outstanding Loan Share = Outstanding Loan ÷ (Outstanding Loan + Remaining Coins and Savings Total) × 100%.");
        // Memperbarui `terms[”players.support.formula.loan_discipline”]` menggunakan tuple yang membawa bagian 1: ”Pinjaman yang lunas ÷ seluruh pinjaman
        // yang diambil × 100%.”; bagian 2: ”Repaid loans ÷ all loans taken × 100%.” dalam AddPlayers.
        terms["players.support.formula.loan_discipline"] = ("Pinjaman yang lunas ÷ seluruh pinjaman yang diambil × 100%.", "Repaid loans ÷ all loans taken × 100%.");
        // Memperbarui `terms[”players.support.formula.debt_ratio”]` menggunakan tuple yang membawa bagian 1: ”Jumlah pinjaman belum lunas ÷ seluruh
        // pinjaman yang diambil.”; bagian 2: ”Unpaid loans ÷ all loans taken.” dalam AddPlayers.
        terms["players.support.formula.debt_ratio"] = ("Jumlah pinjaman belum lunas ÷ seluruh pinjaman yang diambil.", "Unpaid loans ÷ all loans taken.");
        // Memperbarui `terms[”players.support.formula.goal_ambition”]` menggunakan tuple yang membawa bagian 1: ”Target yang dicoba + (koin yang ditanamkan
        // pada target ÷ kekayaan bersih akhir ...; bagian 2: ”Goals attempted + (coins invested in goals ÷ ending net worth × 100). A separat... dalam
        // AddPlayers.
        terms["players.support.formula.goal_ambition"] = ("Target yang dicoba + (koin yang ditanamkan pada target ÷ kekayaan bersih akhir × 100). Indeks 0–100 yang lebih mudah dibandingkan ditampilkan terpisah.", "Goals attempted + (coins invested in goals ÷ ending net worth × 100). A separate 0–100 comparison index is also shown.");
        // Memperbarui `terms[”players.support.formula.goal_ambition_index”]` menggunakan tuple yang membawa bagian 1: ”Progres Pendanaan Target Finansial =
        // Dana yang Diperhitungkan untuk Target ÷ To...; bagian 2: ”Financial Goal Funding Progress = Funds Counted Toward Goals ÷ Total Cost of At...
        // dalam AddPlayers.
        terms["players.support.formula.goal_ambition_index"] = ("Progres Pendanaan Target Finansial = Dana yang Diperhitungkan untuk Target ÷ Total Biaya Target Finansial yang Dicoba × 100%. Dana tiap target dibatasi sebesar biaya target; sisa tabungan setelah target selesai tidak dihitung dua kali.", "Financial Goal Funding Progress = Funds Counted Toward Goals ÷ Total Cost of Attempted Financial Goals × 100%. Funds for each goal are capped at its cost; savings left after completion are not counted twice.");
        // Memperbarui `terms[”players.support.formula.action_efficiency”]` menggunakan tuple yang membawa bagian 1: ”Persentase Aksi Utama Penghasil Koin =
        // Jumlah Aksi Utama Penghasil Koin ÷ Total...; bagian 2: ”Income-Earning Main Action Share = Income-Earning Main Action Count ÷ Total Mai... dalam
        // AddPlayers.
        terms["players.support.formula.action_efficiency"] = ("Persentase Aksi untuk Mendapatkan Koin = Aksi untuk Mendapatkan Koin ÷ Total Aksi yang Digunakan × 100%.", "Share of Actions Used to Earn Coins = Actions Used to Earn Coins ÷ Total Actions Used × 100%.");
        // Memperbarui `terms[”players.support.formula.order_success”]` menggunakan tuple yang membawa bagian 1: ”Persentase Bahan yang Terpakai = Total
        // Bahan Digunakan ÷ Bahan Terkumpul × 100%...; bagian 2: ”Ingredient Utilization = Total Ingredients Used ÷ Ingredients Collected × 100%.... dalam
        // AddPlayers.
        terms["players.support.formula.order_success"] = ("Persentase Bahan yang Terpakai = Total Bahan Digunakan ÷ Bahan Terkumpul × 100%.", "Ingredient Utilization = Total Ingredients Used ÷ Ingredients Collected × 100%.");
        // Memperbarui `terms[”players.support.formula.planning”]` menggunakan tuple yang membawa bagian 1: ”Persentase Aksi Utama untuk Jangka Panjang =
        // (Jumlah Aksi Menabung + Jumlah Aks...; bagian 2: ”Long-Term Main Action Share = (Savings Action Count + Financial Goal Action Cou... dalam
        // AddPlayers.
        terms["players.support.formula.planning"] = ("Persentase Aksi untuk Tabungan, Target, Asuransi, dan Pelunasan = (Aksi untuk Tabungan dan Pembelian Target + Aksi untuk Mengaktifkan Asuransi + Aksi untuk Pelunasan Pinjaman) ÷ Total Aksi yang Digunakan × 100%.", "Share of Actions for Savings, Goals, Insurance, and Repayment = (Actions for Savings and Goal Purchases + Actions for Insurance Activation + Actions for Loan Repayment) ÷ Total Actions Used × 100%.");
        // Memperbarui `terms[”players.support.formula.fulfillment”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan Kartu Kebutuhan = [1 −
        // ((Persentase Kartu Kebutuhan Primer ÷ 100)² +...; bagian 2: ”Need Card Balance = [1 − ((Primary Need Card Share ÷ 100)² + (Secondary Need Ca...
        // dalam AddPlayers.
        terms["players.support.formula.fulfillment"] = ("Pemerataan Kartu Kebutuhan = [1 − ((Persentase Kartu Kebutuhan Primer ÷ 100)² + (Persentase Kartu Kebutuhan Sekunder ÷ 100)² + (Persentase Kartu Kebutuhan Tersier ÷ 100)²)] ÷ (1 − ⅓) × 100%.", "Need Card Balance = [1 − ((Primary Need Card Share ÷ 100)² + (Secondary Need Card Share ÷ 100)² + (Tertiary Need Card Share ÷ 100)²)] ÷ (1 − ⅓) × 100%.");
        // Memperbarui `terms[”players.support.formula.fulfillment_document”]` menggunakan tuple yang membawa bagian 1: ”√(jumlah primer² + jumlah sekunder²
        // + jumlah tersier²) ÷ seluruh kartu kebutuha...; bagian 2: ”√(primary count² + secondary count² + tertiary count²) ÷ all need cards × 100%....
        // dalam AddPlayers.
        terms["players.support.formula.fulfillment_document"] = ("√(jumlah primer² + jumlah sekunder² + jumlah tersier²) ÷ seluruh kartu kebutuhan × 100%.", "√(primary count² + secondary count² + tertiary count²) ÷ all need cards × 100%.");
        // Memperbarui `terms[”players.support.formula.growth”]` menggunakan tuple yang membawa bagian 1: ”Koin bersih akhir ÷ koin awal.”; bagian 2:
        // ”Ending net coins ÷ starting coins.” dalam AddPlayers.
        terms["players.support.formula.growth"] = ("Koin bersih akhir ÷ koin awal.", "Ending net coins ÷ starting coins.");
        // Memperbarui `terms[”players.support.formula.donation_aggressiveness”]` menggunakan tuple yang membawa bagian 1: ”Total koin donasi ÷ koin bersih
        // akhir × 100%.”; bagian 2: ”Total donated coins ÷ ending net coins × 100%.” dalam AddPlayers.
        terms["players.support.formula.donation_aggressiveness"] = ("Total koin donasi ÷ koin bersih akhir × 100%.", "Total donated coins ÷ ending net coins × 100%.");
        // Memperbarui `terms[”players.support.formula.donation_stability”]` menggunakan tuple yang membawa bagian 1: ”100 − simpangan baku nominal donasi,
        // dibatasi 0–100%. Indeks konsistensi relati...; bagian 2: ”100 − the standard deviation of donation amounts, capped at 0–100%. A consisten...
        // dalam AddPlayers.
        terms["players.support.formula.donation_stability"] = ("100 − simpangan baku nominal donasi, dibatasi 0–100%. Indeks konsistensi relatif terhadap rata-rata donasi ditampilkan terpisah.", "100 − the standard deviation of donation amounts, capped at 0–100%. A consistency index relative to the average donation is shown separately.");
        // Memperbarui `terms[”players.support.formula.donation_stability_index”]` menggunakan tuple yang membawa bagian 1: ”[1 − (simpangan baku ÷
        // rata-rata donasi)] × 100%, dibatasi 0–100%.”; bagian 2: ”[1 − (standard deviation ÷ average donation)] × 100%, capped at 0–100%.” dalam
        // AddPlayers.
        terms["players.support.formula.donation_stability_index"] = ("[1 − (simpangan baku ÷ rata-rata donasi)] × 100%, dibatasi 0–100%.", "[1 − (standard deviation ÷ average donation)] × 100%, capped at 0–100%.");
        // Memperbarui `terms[”players.support.formula.donation_commitment”]` menggunakan tuple yang membawa bagian 1: ”Skor Komitmen Donasi = Keteraturan
        // Jumlah Donasi × (Porsi Donasi dari Koin Ters...; bagian 2: ”Donation Commitment Score = Donation Amount Regularity × (Donation Share of Rem...
        // dalam AddPlayers.
        terms["players.support.formula.donation_commitment"] = ("Skor Komitmen Donasi = Keteraturan Jumlah Donasi × (Porsi Donasi dari Koin Tersisa dan Donasi ÷ 100) × (Persentase Jumat dengan Donasi ÷ 100), dibatasi 0–100.", "Donation Commitment Score = Donation Amount Regularity × (Donation Share of Remaining and Donated Coins ÷ 100) × (Share of Fridays with Donations ÷ 100), capped at 0–100.");
        // Memperbarui `terms[”players.support.formula.happiness_portfolio”]` menggunakan tuple yang membawa bagian 1: ”Total Poin Kebahagiaan = Poin
        // Kebahagiaan Kartu Kebutuhan + Poin Kebahagiaan Bo...; bagian 2: ”Total Happiness Points = Need Card Happiness Points + Need Set Bonus Happiness
        // ... dalam AddPlayers.
        terms["players.support.formula.happiness_portfolio"] = ("Pemerataan Sumber Poin Kebahagiaan = [1 − jumlah (poin kebahagiaan tiap sumber ÷ total poin kebahagiaan positif)²] ÷ (1 − 1/6) × 100%. Sumber: Poin Kebahagiaan Kartu Kebutuhan + Poin Kebahagiaan Bonus Set Kebutuhan + Poin Kebahagiaan dari Donasi + Poin Kebahagiaan Emas + Poin Kebahagiaan Dana Pensiun + Poin Kebahagiaan Target Finansial. Sumber bernilai nol tetap dihitung; penalti dikecualikan. Belum tersedia jika tidak ada poin kebahagiaan positif.", "Happiness Point Source Balance = [1 − sum of (Happiness points per source ÷ total positive Happiness points)²] ÷ (1 − 1/6) × 100%. Sources: Need Card Happiness Points + Need Set Bonus Happiness Points + Happiness Points from Donations + Gold Happiness Points + Pension Happiness Points + Financial Goal Happiness Points. Zero-valued sources are included; penalties are excluded. Unavailable when there are no positive Happiness points.");
        // Memperbarui `terms[”players.support.formula.happiness_portfolio.beginner”]` menggunakan tuple yang membawa bagian 1: ”Total Poin Kebahagiaan =
        // Poin Kebahagiaan Kartu Kebutuhan + Poin Kebahagiaan Bo...; bagian 2: ”Total Happiness Points = Need Card Happiness Points + Need Set Bonus
        // Happiness ... dalam AddPlayers.
        terms["players.support.formula.happiness_portfolio.beginner"] = ("Pemerataan Sumber Poin Kebahagiaan = [1 − jumlah (poin kebahagiaan tiap sumber ÷ total poin kebahagiaan positif)²] ÷ (1 − 1/5) × 100%. Sumber: Poin Kebahagiaan Kartu Kebutuhan + Poin Kebahagiaan Bonus Set Kebutuhan + Poin Kebahagiaan dari Donasi + Poin Kebahagiaan Emas + Poin Kebahagiaan Dana Pensiun. Sumber bernilai nol tetap dihitung; penalti dikecualikan. Belum tersedia jika tidak ada poin kebahagiaan positif.", "Happiness Point Source Balance = [1 − sum of (Happiness points per source ÷ total positive Happiness points)²] ÷ (1 − 1/5) × 100%. Sources: Need Card Happiness Points + Need Set Bonus Happiness Points + Happiness Points from Donations + Gold Happiness Points + Pension Happiness Points. Zero-valued sources are included; penalties are excluded. Unavailable when there are no positive Happiness points.");
        // Memperbarui `terms[”players.stats.insights_title”]` menggunakan tuple yang membawa bagian 1: ”Prioritas pembahasan”; bagian 2: ”Discussion
        // priorities” dalam AddPlayers.
        terms["players.stats.insights_title"] = ("Prioritas pembahasan", "Discussion priorities");
        // Memperbarui `terms[”players.stats.insights_desc”]` menggunakan tuple yang membawa bagian 1: ”Baca dari atas; hal yang paling perlu dibahas
        // ditempatkan lebih dahulu.”; bagian 2: ”Read from the top; the most important discussion point appears first.” dalam AddPlayers.
        terms["players.stats.insights_desc"] = ("Baca dari atas; hal yang paling perlu dibahas ditempatkan lebih dahulu.", "Read from the top; the most important discussion point appears first.");
        // Memperbarui `terms[”players.stats.chart.cashflow_snapshot”]` menggunakan tuple yang membawa bagian 1: ”Perbandingan Uang Pemain”; bagian 2:
        // ”Player Money Comparison” dalam AddPlayers.
        terms["players.stats.chart.cashflow_snapshot"] = ("Perbandingan Uang Pemain", "Player Money Comparison");
        // Memperbarui `terms[”players.stats.chart.happiness_composition”]` menggunakan tuple yang membawa bagian 1: ”Asal Poin Kebahagiaan Akhir”; bagian
        // 2: ”Final Happiness Point Sources” dalam AddPlayers.
        terms["players.stats.chart.happiness_composition"] = ("Asal Poin Kebahagiaan Akhir", "Final Happiness Point Sources");
        // Memperbarui `terms[”players.stats.chart.behavior_snapshot”]` menggunakan tuple yang membawa bagian 1: ”Pilihan dan Keberagaman Kebutuhan”; bagian
        // 2: ”Player Choices and Need Diversity” dalam AddPlayers.
        terms["players.stats.chart.behavior_snapshot"] = ("Pilihan dan Keberagaman Kebutuhan", "Player Choices and Need Diversity");
        // Memperbarui `terms[”players.stats.clear”]` menggunakan tuple yang membawa bagian 1: ”Lunas”; bagian 2: ”Clear” dalam AddPlayers.
        terms["players.stats.clear"] = ("Lunas", "Clear");
        // Memperbarui `terms[”players.stats.unpaid”]` menggunakan tuple yang membawa bagian 1: ”Belum Lunas”; bagian 2: ”Unpaid” dalam AddPlayers.
        terms["players.stats.unpaid"] = ("Belum Lunas", "Unpaid");
        // Memperbarui `terms[”players.stats.insight.cashflow_negative.title”]` menggunakan tuple yang membawa bagian 1: ”Pengeluaran lebih besar dari
        // pemasukan”; bagian 2: ”Spending is higher than income” dalam AddPlayers.
        terms["players.stats.insight.cashflow_negative.title"] = ("Pengeluaran lebih besar dari pemasukan", "Spending is higher than income");
        // Memperbarui `terms[”players.stats.insight.cashflow_negative.desc”]` menggunakan tuple yang membawa bagian 1: ”Pemain mengeluarkan lebih banyak
        // uang daripada yang diterima. Periksa pengeluar...; bagian 2: ”The player spent more money than they received. Review spending, risks, and ass...
        // dalam AddPlayers.
        terms["players.stats.insight.cashflow_negative.desc"] = ("Pemain mengeluarkan lebih banyak uang daripada yang diterima. Periksa pengeluaran, risiko, dan pembelian asetnya.", "The player spent more money than they received. Review spending, risks, and asset purchases.");
        // Memperbarui `terms[”players.stats.insight.loan_unpaid.title”]` menggunakan tuple yang membawa bagian 1: ”Masih ada pinjaman yang belum dibayar”;
        // bagian 2: ”A loan is still unpaid” dalam AddPlayers.
        terms["players.stats.insight.loan_unpaid.title"] = ("Masih ada pinjaman yang belum dibayar", "A loan is still unpaid");
        // Memperbarui `terms[”players.stats.insight.loan_unpaid.desc”]` menggunakan tuple yang membawa bagian 1: ”Pinjaman yang belum lunas mengurangi poin
        // kebahagiaan akhir. Tinjau kapan pinja...; bagian 2: ”An unpaid loan reduces the final score. Review when it was taken and why it was... dalam
        // AddPlayers.
        terms["players.stats.insight.loan_unpaid.desc"] = ("Pinjaman yang belum lunas mengurangi poin kebahagiaan akhir. Tinjau kapan pinjaman diambil dan mengapa belum dibayar.", "An unpaid loan reduces the final score. Review when it was taken and why it was not repaid.");
        // Memperbarui `terms[”players.stats.insight.happiness_low.title”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan akhir masih rendah”;
        // bagian 2: ”The final score is still low” dalam AddPlayers.
        terms["players.stats.insight.happiness_low.title"] = ("Poin Kebahagiaan akhir masih rendah", "The final score is still low");
        // Memperbarui `terms[”players.stats.insight.happiness_low.desc”]` menggunakan tuple yang membawa bagian 1: ”Periksa poin kebahagiaan dari
        // kebutuhan, donasi, emas, dan tabungan, lalu lihat...; bagian 2: ”Review happiness points from needs, donations, gold, and savings, then check
        // fo... dalam AddPlayers.
        terms["players.stats.insight.happiness_low.desc"] = ("Periksa poin kebahagiaan dari kebutuhan, donasi, emas, dan tabungan, lalu lihat apakah ada pengurangan karena misi atau pinjaman.", "Review happiness points from needs, donations, gold, and savings, then check for mission or loan deductions.");
        // Memperbarui `terms[”players.stats.insight.need_diversity_low.title”]` menggunakan tuple yang membawa bagian 1: ”Pemenuhan kebutuhan belum
        // beragam”; bagian 2: ”Need fulfillment lacks diversity” dalam AddPlayers.
        terms["players.stats.insight.need_diversity_low.title"] = ("Pemenuhan kebutuhan belum beragam", "Need fulfillment lacks diversity");
        // Memperbarui `terms[”players.stats.insight.need_diversity_low.desc”]` menggunakan tuple yang membawa bagian 1: ”Kepemilikan masih berat pada satu
        // kategori. Tinjau komposisi primer, sekunder, ...; bagian 2: ”Ownership is concentrated in one category. Review the primary, secondary, terti...
        // dalam AddPlayers.
        terms["players.stats.insight.need_diversity_low.desc"] = ("Kepemilikan masih berat pada satu kategori. Tinjau komposisi primer, sekunder, tersier, dan target misi koleksi.", "Ownership is concentrated in one category. Review the primary, secondary, tertiary, and collection-mission mix.");
        // Memperbarui `terms[”players.stats.insight.need_cards_missing.title”]` menggunakan tuple yang membawa bagian 1: ”Belum ada kartu kebutuhan”;
        // bagian 2: ”No need cards yet” dalam AddPlayers.
        terms["players.stats.insight.need_cards_missing.title"] = ("Belum ada kartu kebutuhan", "No need cards yet");
        // Memperbarui `terms[”players.stats.insight.need_cards_missing.desc”]` menggunakan tuple yang membawa bagian 1: ”Pemain belum memiliki kartu
        // kebutuhan. Karena itu, pemerataan kebutuhan belum d...; bagian 2: ”The player does not own any need cards, so need balance cannot be calculated
        // an... dalam AddPlayers.
        terms["players.stats.insight.need_cards_missing.desc"] = ("Pemain belum memiliki kartu kebutuhan. Karena itu, pemerataan kebutuhan belum dapat dihitung dan misi koleksi belum terpenuhi.", "The player does not own any need cards, so need balance cannot be calculated and the collection mission is incomplete.");
        // Memperbarui `terms[”players.stats.need_balance.none”]` menggunakan tuple yang membawa bagian 1: ”Belum dapat dihitung”; bagian 2: ”Cannot be
        // calculated yet” dalam AddPlayers.
        terms["players.stats.need_balance.none"] = ("Belum dapat dihitung", "Cannot be calculated yet");
        // Memperbarui `terms[”players.stats.insight.data_unavailable.title”]` menggunakan tuple yang membawa bagian 1: ”Data analitik belum tersedia”;
        // bagian 2: ”Analytics data unavailable” dalam AddPlayers.
        terms["players.stats.insight.data_unavailable.title"] = ("Data analitik belum tersedia", "Analytics data unavailable");
        // Memperbarui `terms[”players.stats.insight.data_unavailable.desc”]` menggunakan tuple yang membawa bagian 1: ”Belum ada data yang cukup untuk
        // menentukan prioritas pembahasan pemain.”; bagian 2: ”There is not enough data yet to determine the player's discussion priorities.” dalam
        // AddPlayers.
        terms["players.stats.insight.data_unavailable.desc"] = ("Belum ada data yang cukup untuk menentukan prioritas pembahasan pemain.", "There is not enough data yet to determine the player's discussion priorities.");
        // Memperbarui `terms[”players.stats.insight.stable_profile.title”]` menggunakan tuple yang membawa bagian 1: ”Tidak ada masalah utama”; bagian 2:
        // ”No major issue found” dalam AddPlayers.
        terms["players.stats.insight.stable_profile.title"] = ("Tidak ada masalah utama", "No major issue found");
        // Memperbarui `terms[”players.stats.insight.stable_profile.desc”]` menggunakan tuple yang membawa bagian 1: ”Tidak ada risiko besar. Pertahankan
        // keputusan yang sudah efektif dan pilih satu...; bagian 2: ”There is no major risk. Keep the decisions that worked and choose one area to i...
        // dalam AddPlayers.
        terms["players.stats.insight.stable_profile.desc"] = ("Tidak ada risiko besar. Pertahankan keputusan yang sudah efektif dan pilih satu area untuk ditingkatkan.", "There is no major risk. Keep the decisions that worked and choose one area to improve.");
        // Memperbarui `terms[”players.cashflow_journey.starting_cash”]` menggunakan tuple yang membawa bagian 1: ”Koin Awal”; bagian 2: ”Starting Coins”
        // dalam AddPlayers.
        terms["players.cashflow_journey.starting_cash"] = ("Koin Awal", "Starting Coins");
        // Memperbarui `terms[”players.cashflow_journey.ending_cash”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa”; bagian 2: ”Remaining Coins”
        // dalam AddPlayers.
        terms["players.cashflow_journey.ending_cash"] = ("Koin Tersisa", "Remaining Coins");
        // Memperbarui `terms[”players.cashflow_journey.total_transactions”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Semua Transaksi”; bagian 2:
        // ”All Transactions” dalam AddPlayers.
        terms["players.cashflow_journey.total_transactions"] = ("Jumlah Semua Transaksi", "All Transactions");
        // Memperbarui `terms[”players.cashflow_journey.total_in”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Masuk”; bagian 2: ”Total Incoming
        // Coins” dalam AddPlayers.
        terms["players.cashflow_journey.total_in"] = ("Total Koin Masuk", "Total Incoming Coins");
        // Memperbarui `terms[”players.cashflow_journey.total_out”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Keluar”; bagian 2: ”Total Outgoing
        // Coins” dalam AddPlayers.
        terms["players.cashflow_journey.total_out"] = ("Total Koin Keluar", "Total Outgoing Coins");
        // Memperbarui `terms[”players.cashflow_journey.net”]` menggunakan tuple yang membawa bagian 1: ”Selisih Koin Masuk dan Keluar”; bagian 2: ”Incoming
        // and Outgoing Coin Difference” dalam AddPlayers.
        terms["players.cashflow_journey.net"] = ("Selisih Koin Masuk dan Keluar", "Incoming and Outgoing Coin Difference");
        // Memperbarui `terms[”players.cashflow_journey.running_peak”]` menggunakan tuple yang membawa bagian 1: ”Saldo Tertinggi Selama Permainan”; bagian
        // 2: ”Highest Balance During the Game” dalam AddPlayers.
        terms["players.cashflow_journey.running_peak"] = ("Saldo Tertinggi Selama Permainan", "Highest Balance During the Game");
        // Memperbarui `terms[”players.cashflow_journey.running_lowest”]` menggunakan tuple yang membawa bagian 1: ”Saldo Terendah Selama Permainan”; bagian
        // 2: ”Lowest Balance During the Game” dalam AddPlayers.
        terms["players.cashflow_journey.running_lowest"] = ("Saldo Terendah Selama Permainan", "Lowest Balance During the Game");
        // Memperbarui `terms[”players.cashflow_journey.period_start”]` menggunakan tuple yang membawa bagian 1: ”Transaksi Pertama”; bagian 2: ”First
        // Transaction” dalam AddPlayers.
        terms["players.cashflow_journey.period_start"] = ("Transaksi Pertama", "First Transaction");
        // Memperbarui `terms[”players.cashflow_journey.period_end”]` menggunakan tuple yang membawa bagian 1: ”Transaksi Terakhir”; bagian 2: ”Last
        // Transaction” dalam AddPlayers.
        terms["players.cashflow_journey.period_end"] = ("Transaksi Terakhir", "Last Transaction");
        // Memperbarui `terms[”players.cashflow_journey.in_count”]` menggunakan tuple yang membawa bagian 1: ”Berapa Kali Uang Masuk”; bagian 2: ”Number of
        // Incoming Transactions” dalam AddPlayers.
        terms["players.cashflow_journey.in_count"] = ("Berapa Kali Uang Masuk", "Number of Incoming Transactions");
        // Memperbarui `terms[”players.cashflow_journey.out_count”]` menggunakan tuple yang membawa bagian 1: ”Berapa Kali Uang Keluar”; bagian 2: ”Number
        // of Outgoing Transactions” dalam AddPlayers.
        terms["players.cashflow_journey.out_count"] = ("Berapa Kali Uang Keluar", "Number of Outgoing Transactions");
        // Memperbarui `terms[”players.cashflow_journey.chart_title”]` menggunakan tuple yang membawa bagian 1: ”Perubahan Saldo di Setiap Transaksi”;
        // bagian 2: ”Balance Changes at Each Transaction” dalam AddPlayers.
        terms["players.cashflow_journey.chart_title"] = ("Perubahan Saldo di Setiap Transaksi", "Balance Changes at Each Transaction");
        // Memperbarui `terms[”players.cashflow_journey.series_name”]` menggunakan tuple yang membawa bagian 1: ”Saldo Setelah Transaksi”; bagian 2:
        // ”Balance After Each Transaction” dalam AddPlayers.
        terms["players.cashflow_journey.series_name"] = ("Saldo Setelah Transaksi", "Balance After Each Transaction");
        // Memperbarui `terms[”players.cashflow_journey.formula”]` menggunakan tuple yang membawa bagian 1: ”Saldo setelah transaksi = Koin Awal + akumulasi
        // Koin Masuk − akumulasi Koin Kel...; bagian 2: ”Balance after a transaction = Starting Coins + accumulated Incoming Coins − acc... dalam
        // AddPlayers.
        terms["players.cashflow_journey.formula"] = ("Saldo setelah transaksi = Koin Awal + akumulasi Koin Masuk − akumulasi Koin Keluar sampai transaksi tersebut.", "Balance after a transaction = Starting Coins + accumulated Incoming Coins − accumulated Outgoing Coins up to that transaction.");
        // Memperbarui `terms[”players.cashflow_journey.quick_check”]` menggunakan tuple yang membawa bagian 1: ”Koin Awal {0} + Total Koin Masuk {1} −
        // Total Koin Keluar {2} = Koin Tersisa {3}...; bagian 2: ”Starting Coins {0} + Total Incoming Coins {1} − Total Outgoing Coins {2} = Rema... dalam
        // AddPlayers.
        terms["players.cashflow_journey.quick_check"] = ("Koin Awal {0} + Total Koin Masuk {1} − Total Koin Keluar {2} = Koin Tersisa {3}.", "Starting Coins {0} + Total Incoming Coins {1} − Total Outgoing Coins {2} = Remaining Coins {3}.");
        // Memperbarui `terms[”players.cashflow_journey.empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada data transaksi untuk pemain ini di sesi
        // tersebut.”; bagian 2: ”No transaction data is available for this player in this session.” dalam AddPlayers.
        terms["players.cashflow_journey.empty"] = ("Belum ada data transaksi untuk pemain ini di sesi tersebut.", "No transaction data is available for this player in this session.");
        // Memperbarui `terms[”players.details.metric”]` menggunakan tuple yang membawa bagian 1: ”Variabel”; bagian 2: ”Data” dalam AddPlayers.
        terms["players.details.metric"] = ("Variabel", "Data");
        // Memperbarui `terms[”players.details.series”]` menggunakan tuple yang membawa bagian 1: ”Seri”; bagian 2: ”Series” dalam AddPlayers.
        terms["players.details.series"] = ("Seri", "Series");
        // Memperbarui `terms[”players.details.transaction_label”]` menggunakan tuple yang membawa bagian 1: ”Transaksi”; bagian 2: ”Transaction” dalam
        // AddPlayers.
        terms["players.details.transaction_label"] = ("Transaksi", "Transaction");
        // Memperbarui `terms[”players.details.points_value”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan/Nilai”; bagian 2: ”Happiness
        // Points/Value” dalam AddPlayers.
        terms["players.details.points_value"] = ("Poin Kebahagiaan/Nilai", "Happiness Points/Value");
        // Memperbarui `terms[”players.details.metric_fallback”]` menggunakan tuple yang membawa bagian 1: ”Data”; bagian 2: ”Data” dalam AddPlayers.
        terms["players.details.metric_fallback"] = ("Data", "Data");
        // Memperbarui `terms[”players.details.item”]` menggunakan tuple yang membawa bagian 1: ”Item”; bagian 2: ”Item” dalam AddPlayers.
        terms["players.details.item"] = ("Item", "Item");
        // Memperbarui `terms[”players.details.combined_snapshot”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Gabungan”; bagian 2: ”Combined
        // Snapshot” dalam AddPlayers.
        terms["players.details.combined_snapshot"] = ("Ringkasan Gabungan", "Combined Snapshot");
        // Memperbarui `terms[”players.details.source_calc_label”]` menggunakan tuple yang membawa bagian 1: ”Asal Data & Penjelasan”; bagian 2: ”Data
        // Source & Explanation” dalam AddPlayers.
        terms["players.details.source_calc_label"] = ("Asal Data & Penjelasan", "Data Source & Explanation");
        // Memperbarui `terms[”players.details.source_calc_template”]` menggunakan tuple yang membawa bagian 1: ”{0}. Penjelasan: {1}”; bagian 2: ”{0}.
        // Explanation: {1}” dalam AddPlayers.
        terms["players.details.source_calc_template"] = ("{0}. Penjelasan: {1}", "{0}. Explanation: {1}");
        // Memperbarui `terms[”players.details.source_calc_default”]` menggunakan tuple yang membawa bagian 1: ”agregasi aktivitas sesi tervalidasi.
        // Penjelasan: nilai mengikuti formula pada S...; bagian 2: ”Validated session activity aggregation. Explanation: the value follows formulas...
        // dalam AddPlayers.
        terms["players.details.source_calc_default"] = ("agregasi aktivitas sesi tervalidasi. Penjelasan: nilai mengikuti formula pada Set Aturan aktif.", "Validated session activity aggregation. Explanation: the value follows formulas in the active ruleset.");
        // Memperbarui `terms[”players.details.source_raw_summary”]` menggunakan tuple yang membawa bagian 1: ”agregasi aktivitas sesi. Penjelasan: nilai
        // diringkas dari transaksi atau aksi p...; bagian 2: ”Session activity aggregation. Explanation: the value is summarized from transac... dalam
        // AddPlayers.
        terms["players.details.source_raw_summary"] = ("agregasi aktivitas sesi. Penjelasan: nilai diringkas dari transaksi atau aksi pada kelompok ini.", "Session activity aggregation. Explanation: the value is summarized from transactions or actions in this group.");
        // Memperbarui `terms[”players.details.source_derived_summary”]` menggunakan tuple yang membawa bagian 1: ”agregasi aktivitas sesi tervalidasi.
        // Penjelasan: nilai turunan mengikuti formul...; bagian 2: ”Validated session activity aggregation. Explanation: the derived value follows ...
        // dalam AddPlayers.
        terms["players.details.source_derived_summary"] = ("agregasi aktivitas sesi tervalidasi. Penjelasan: nilai turunan mengikuti formula pada Set Aturan aktif.", "Validated session activity aggregation. Explanation: the derived value follows formulas from the active ruleset.");
        // Memperbarui `terms[”players.details.source_line_raw_template”]` menggunakan tuple yang membawa bagian 1: ”{0}.”; bagian 2: ”{0}.” dalam
        // AddPlayers.
        terms["players.details.source_line_raw_template"] = ("{0}.", "{0}.");
        // Memperbarui `terms[”players.details.source_line_derived_template”]` menggunakan tuple yang membawa bagian 1: ”{0}. Penjelasan: nilai turunan
        // dihitung dari data permainan sesuai Set Aturan a...; bagian 2: ”{0}. Explanation: the derived value is calculated from gameplay data using the
        // ... dalam AddPlayers.
        terms["players.details.source_line_derived_template"] = ("{0}. Penjelasan: nilai turunan dihitung dari data permainan sesuai Set Aturan aktif.", "{0}. Explanation: the derived value is calculated from gameplay data using the active ruleset.");
        // Memperbarui `terms[”players.details.selected_detail”]` menggunakan tuple yang membawa bagian 1: ”Detail data terpilih”; bagian 2: ”Selected data
        // details” dalam AddPlayers.
        terms["players.details.selected_detail"] = ("Detail data terpilih", "Selected data details");
        // Memperbarui `terms[”players.details.tap_hint”]` menggunakan tuple yang membawa bagian 1: ”Ketuk batang atau titik pada diagram untuk melihat
        // nilai, fungsi, dan asal data...; bagian 2: ”Tap bars or points in the chart to view the value, function, and data source.” dalam AddPlayers.
        terms["players.details.tap_hint"] = ("Ketuk batang atau titik pada diagram untuk melihat nilai, fungsi, dan asal datanya.", "Tap bars or points in the chart to view the value, function, and data source.");
        // Memperbarui `terms[”players.details.no_series”]` menggunakan tuple yang membawa bagian 1: ”Belum ada rangkaian data untuk ditampilkan.”; bagian
        // 2: ”No data series are available to display.” dalam AddPlayers.
        terms["players.details.no_series"] = ("Belum ada rangkaian data untuk ditampilkan.", "No data series are available to display.");
        // Memperbarui `terms[”players.details.no_points”]` menggunakan tuple yang membawa bagian 1: ”Belum ada titik numerik untuk ditampilkan.”; bagian 2:
        // ”No numeric points are available to display.” dalam AddPlayers.
        terms["players.details.no_points"] = ("Belum ada titik numerik untuk ditampilkan.", "No numeric points are available to display.");
        // Memperbarui `terms[”players.details.invalid_payload”]` menggunakan tuple yang membawa bagian 1: ”Payload diagram tidak valid.”; bagian 2: ”Chart
        // payload is invalid.” dalam AddPlayers.
        terms["players.details.invalid_payload"] = ("Payload diagram tidak valid.", "Chart payload is invalid.");
        // Memperbarui `terms[”players.details.transaction.opening_cash”]` menggunakan tuple yang membawa bagian 1: ”Koin Awal”; bagian 2: ”Starting Coins”
        // dalam AddPlayers.
        terms["players.details.transaction.opening_cash"] = ("Koin Awal", "Starting Coins");
        // Memperbarui `terms[”players.details.transaction.opening_cash_with_amount”]` menggunakan tuple yang membawa bagian 1: ”Koin Awal ({0})”; bagian 2:
        // ”Starting Coins ({0})” dalam AddPlayers.
        terms["players.details.transaction.opening_cash_with_amount"] = ("Koin Awal ({0})", "Starting Coins ({0})");
        // Memperbarui `terms[”players.details.transaction.cash_in”]` menggunakan tuple yang membawa bagian 1: ”Koin Masuk”; bagian 2: ”Incoming Coins”
        // dalam AddPlayers.
        terms["players.details.transaction.cash_in"] = ("Koin Masuk", "Incoming Coins");
        // Memperbarui `terms[”players.details.transaction.cash_out”]` menggunakan tuple yang membawa bagian 1: ”Koin Keluar”; bagian 2: ”Outgoing Coins”
        // dalam AddPlayers.
        terms["players.details.transaction.cash_out"] = ("Koin Keluar", "Outgoing Coins");
        // Memperbarui `terms[”players.details.transaction.category.donation”]` menggunakan tuple yang membawa bagian 1: ”Donasi”; bagian 2: ”Donation”
        // dalam AddPlayers.
        terms["players.details.transaction.category.donation"] = ("Donasi", "Donation");
        // Memperbarui `terms[”players.details.transaction.category.gold_trade”]` menggunakan tuple yang membawa bagian 1: ”Transaksi Emas”; bagian 2: ”Gold
        // Trade” dalam AddPlayers.
        terms["players.details.transaction.category.gold_trade"] = ("Transaksi Emas", "Gold Trade");
        // Memperbarui `terms[”players.details.transaction.category.ingredient”]` menggunakan tuple yang membawa bagian 1: ”Pembelian Bahan”; bagian 2:
        // ”Ingredient Purchase” dalam AddPlayers.
        terms["players.details.transaction.category.ingredient"] = ("Pembelian Bahan", "Ingredient Purchase");
        // Memperbarui `terms[”players.details.transaction.category.order”]` menggunakan tuple yang membawa bagian 1: ”Pesanan Makanan”; bagian 2: ”Meal
        // Order” dalam AddPlayers.
        terms["players.details.transaction.category.order"] = ("Pesanan Makanan", "Meal Order");
        // Memperbarui `terms[”players.details.transaction.category.freelance”]` menggunakan tuple yang membawa bagian 1: ”Kerja Lepas”; bagian 2:
        // ”Freelance Work” dalam AddPlayers.
        terms["players.details.transaction.category.freelance"] = ("Kerja Lepas", "Freelance Work");
        // Memperbarui `terms[”players.details.transaction.category.need_primary”]` menggunakan tuple yang membawa bagian 1: ”Kebutuhan Primer”; bagian 2:
        // ”Primary Need” dalam AddPlayers.
        terms["players.details.transaction.category.need_primary"] = ("Kebutuhan Primer", "Primary Need");
        // Memperbarui `terms[”players.details.transaction.category.need_secondary”]` menggunakan tuple yang membawa bagian 1: ”Kebutuhan Sekunder”; bagian
        // 2: ”Secondary Need” dalam AddPlayers.
        terms["players.details.transaction.category.need_secondary"] = ("Kebutuhan Sekunder", "Secondary Need");
        // Memperbarui `terms[”players.details.transaction.category.need_tertiary”]` menggunakan tuple yang membawa bagian 1: ”Kebutuhan Tersier”; bagian 2:
        // ”Tertiary Need” dalam AddPlayers.
        terms["players.details.transaction.category.need_tertiary"] = ("Kebutuhan Tersier", "Tertiary Need");
        // Memperbarui `terms[”players.details.transaction.category.saving_deposit”]` menggunakan tuple yang membawa bagian 1: ”Setoran Tabungan”; bagian 2:
        // ”Saving Deposit” dalam AddPlayers.
        terms["players.details.transaction.category.saving_deposit"] = ("Setoran Tabungan", "Saving Deposit");
        // Memperbarui `terms[”players.details.transaction.category.saving_withdraw”]` menggunakan tuple yang membawa bagian 1: ”Penarikan Tabungan”; bagian
        // 2: ”Saving Withdrawal” dalam AddPlayers.
        terms["players.details.transaction.category.saving_withdraw"] = ("Penarikan Tabungan", "Saving Withdrawal");
        // Memperbarui `terms[”players.details.transaction.category.risk_life”]` menggunakan tuple yang membawa bagian 1: ”Dampak Risiko Kehidupan”; bagian
        // 2: ”Life Risk Impact” dalam AddPlayers.
        terms["players.details.transaction.category.risk_life"] = ("Dampak Risiko Kehidupan", "Life Risk Impact");
        // Memperbarui `terms[”players.details.transaction.category.loan_taken”]` menggunakan tuple yang membawa bagian 1: ”Pinjaman Diambil”; bagian 2:
        // ”Loan Taken” dalam AddPlayers.
        terms["players.details.transaction.category.loan_taken"] = ("Pinjaman Diambil", "Loan Taken");
        // Memperbarui `terms[”players.details.transaction.category.loan_repaid”]` menggunakan tuple yang membawa bagian 1: ”Pelunasan Pinjaman”; bagian 2:
        // ”Loan Repaid” dalam AddPlayers.
        terms["players.details.transaction.category.loan_repaid"] = ("Pelunasan Pinjaman", "Loan Repaid");
        // Memperbarui `terms[”players.details.transaction.category.insurance_premium”]` menggunakan tuple yang membawa bagian 1: ”Premi Asuransi”; bagian
        // 2: ”Insurance Premium” dalam AddPlayers.
        terms["players.details.transaction.category.insurance_premium"] = ("Premi Asuransi", "Insurance Premium");
        // Memperbarui `terms[”players.details.transaction.category.insurance_claim”]` menggunakan tuple yang membawa bagian 1: ”Klaim Asuransi”; bagian 2:
        // ”Insurance Claim” dalam AddPlayers.
        terms["players.details.transaction.category.insurance_claim"] = ("Klaim Asuransi", "Insurance Claim");
        // Memperbarui `terms[”players.details.transaction.category.emergency_option”]` menggunakan tuple yang membawa bagian 1: ”Tindakan Darurat”; bagian
        // 2: ”Emergency Actions” dalam AddPlayers.
        terms["players.details.transaction.category.emergency_option"] = ("Tindakan Darurat", "Emergency Actions");
        // Memperbarui `terms[”players.details.formula.coins_net_end”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa = Koin Awal + Total Koin
        // Masuk − Total Koin Keluar.”; bagian 2: ”Remaining Coins = Starting Coins + Total Incoming Coins − Total Outgoing Coins.... dalam AddPlayers.
        terms["players.details.formula.coins_net_end"] = ("Koin Tersisa = Koin Awal + Total Koin Masuk − Total Koin Keluar.", "Remaining Coins = Starting Coins + Total Incoming Coins − Total Outgoing Coins.");
        // Memperbarui `terms[”players.details.formula.raw.cash_in_sum”]` menggunakan tuple yang membawa bagian 1: ”Menjumlahkan semua transaksi kas masuk
        // pemain pada domain ini.”; bagian 2: ”Sums all player cash-in transactions in this domain.” dalam AddPlayers.
        terms["players.details.formula.raw.cash_in_sum"] = ("Menjumlahkan semua transaksi kas masuk pemain pada domain ini.", "Sums all player cash-in transactions in this domain.");
        // Memperbarui `terms[”players.details.formula.raw.cash_out_sum”]` menggunakan tuple yang membawa bagian 1: ”Menjumlahkan semua transaksi kas keluar
        // pemain pada domain ini.”; bagian 2: ”Sums all player cash-out transactions in this domain.” dalam AddPlayers.
        terms["players.details.formula.raw.cash_out_sum"] = ("Menjumlahkan semua transaksi kas keluar pemain pada domain ini.", "Sums all player cash-out transactions in this domain.");
        // Memperbarui `terms[”players.details.formula.raw.per_turn_aggregate”]` menggunakan tuple yang membawa bagian 1: ”Agregasi nilai per slot aksi
        // berdasarkan transaksi/aksi yang tercatat pada slot...; bagian 2: ”Per-action-slot aggregation based on transactions/actions recorded in that
        // slot... dalam AddPlayers.
        terms["players.details.formula.raw.per_turn_aggregate"] = ("Agregasi nilai per slot aksi berdasarkan transaksi/aksi yang tercatat pada slot tersebut.", "Per-action-slot aggregation based on transactions/actions recorded in that slot.");
        // Memperbarui `terms[”players.details.formula.derived.happiness_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Kebutuhan +
        // Bonus + Poin Kebahagiaan Donasi + Poin Kebahagiaan...; bagian 2: ”Need Happiness Points + Bonus + Donation Happiness Points + Gold Happiness
        // Poin... dalam AddPlayers.
        terms["players.details.formula.derived.happiness_points"] = ("Poin Kebahagiaan Kebutuhan + Bonus + Poin Kebahagiaan Donasi + Poin Kebahagiaan Emas + Poin Kebahagiaan Pensiun + Poin Kebahagiaan Target Tabungan - Penalti Misi - Penalti Pinjaman", "Need Happiness Points + Bonus + Donation Happiness Points + Gold Happiness Points + Pension Happiness Points + Saving Goal Happiness Points - Mission Penalty - Loan Penalty");
        // Memperbarui `terms[”players.details.formula.derived.net_cashflow”]` menggunakan tuple yang membawa bagian 1: ”Selisih Koin Masuk dan Keluar =
        // Total Koin Masuk − Total Koin Keluar.”; bagian 2: ”Incoming and Outgoing Coin Difference = Total Incoming Coins − Total Outgoing C... dalam
        // AddPlayers.
        terms["players.details.formula.derived.net_cashflow"] = ("Selisih Koin Masuk dan Keluar = Total Koin Masuk − Total Koin Keluar.", "Incoming and Outgoing Coin Difference = Total Incoming Coins − Total Outgoing Coins.");
        // Memperbarui `terms[”players.details.formula.derived.donation_points”]` menggunakan tuple yang membawa bagian 1: ”Total Donasi x bobot poin
        // kebahagiaan donasi (set aturan aktif)”; bagian 2: ”Total Donations x donation-happiness point weight (active ruleset)” dalam AddPlayers.
        terms["players.details.formula.derived.donation_points"] = ("Total Donasi x bobot poin kebahagiaan donasi (set aturan aktif)", "Total Donations x donation-happiness point weight (active ruleset)");
        // Memperbarui `terms[”players.details.formula.derived.pension_points”]` menggunakan tuple yang membawa bagian 1: ”Akumulasi dana pensiun akhir x
        // bobot poin kebahagiaan pensiun (set aturan aktif...; bagian 2: ”End-game pension accumulation x pension-happiness point weight (active
        // ruleset)... dalam AddPlayers.
        terms["players.details.formula.derived.pension_points"] = ("Akumulasi dana pensiun akhir x bobot poin kebahagiaan pensiun (set aturan aktif)", "End-game pension accumulation x pension-happiness point weight (active ruleset)");
        // Memperbarui `terms[”players.details.formula.derived.saving_goal_points”]` menggunakan tuple yang membawa bagian 1: ”Pencapaian target tabungan x
        // bobot poin kebahagiaan target tabungan (set aturan...; bagian 2: ”Saving-goal achievement x saving-goal happiness point weight (active ruleset)”
        // dalam AddPlayers.
        terms["players.details.formula.derived.saving_goal_points"] = ("Pencapaian target tabungan x bobot poin kebahagiaan target tabungan (set aturan aktif)", "Saving-goal achievement x saving-goal happiness point weight (active ruleset)");
        // Memperbarui `terms[”players.details.formula.derived.need_points”]` menggunakan tuple yang membawa bagian 1: ”Akumulasi poin kebahagiaan kartu
        // kebutuhan sesuai aturan sesi”; bagian 2: ”Accumulated need-card happiness points based on session rules” dalam AddPlayers.
        terms["players.details.formula.derived.need_points"] = ("Akumulasi poin kebahagiaan kartu kebutuhan sesuai aturan sesi", "Accumulated need-card happiness points based on session rules");
        // Memperbarui `terms[”players.details.formula.derived.penalty”]` menggunakan tuple yang membawa bagian 1: ”Akumulasi penalti dari
        // pelanggaran/ketidakpatuhan selama sesi”; bagian 2: ”Accumulated penalties from violations/non-compliance during the session” dalam AddPlayers.
        terms["players.details.formula.derived.penalty"] = ("Akumulasi penalti dari pelanggaran/ketidakpatuhan selama sesi", "Accumulated penalties from violations/non-compliance during the session");
        // Memperbarui `terms[”players.details.formula.derived.points_weight”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan dihitung sesuai
        // bobot pada set aturan aktif”; bagian 2: ”Happiness Points are calculated using weights from the active ruleset” dalam AddPlayers.
        terms["players.details.formula.derived.points_weight"] = ("Poin Kebahagiaan dihitung sesuai bobot pada set aturan aktif", "Happiness Points are calculated using weights from the active ruleset");
        // Memperbarui `terms[”players.details.formula.derived.default”]` menggunakan tuple yang membawa bagian 1: ”Nilai turunan dihitung dari variabel
        // mentah sesuai aturan pada ruleset aktif.”; bagian 2: ”Derived value is calculated from raw variables using formulas in the active rul... dalam
        // AddPlayers.
        terms["players.details.formula.derived.default"] = ("Nilai turunan dihitung dari variabel mentah sesuai aturan pada ruleset aktif.", "Derived value is calculated from raw variables using formulas in the active ruleset.");
        // Memperbarui `terms[”players.details.raw_title”]` menggunakan tuple yang membawa bagian 1: ”Data Permainan”; bagian 2: ”Gameplay Data” dalam
        // AddPlayers.
        terms["players.details.raw_title"] = ("Data Permainan", "Gameplay Data");
        // Memperbarui `terms[”players.details.raw_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Data permainan divisualkan per kelompok. Ringkasan
        // gabungan tampil sebagai bata...; bagian 2: ”Gameplay data is visualized by group. Combined summaries use bars, while change... dalam AddPlayers.
        terms["players.details.raw_subtitle"] = ("Data permainan divisualkan per kelompok. Ringkasan gabungan tampil sebagai batang, sedangkan urutan perubahan tampil sebagai garis. Arahkan kursor atau ketuk batang atau titik untuk melihat fungsi, asal data, dan nilainya.", "Gameplay data is visualized by group. Combined summaries use bars, while changes over time use lines. Hover over or tap a bar or point to view its function, source, and value.");
        // Memperbarui `terms[”players.details.derived_title”]` menggunakan tuple yang membawa bagian 1: ”Hasil Analisis Permainan”; bagian 2: ”Gameplay
        // Analysis Results” dalam AddPlayers.
        terms["players.details.derived_title"] = ("Hasil Analisis Permainan", "Gameplay Analysis Results");
        // Memperbarui `terms[”players.details.derived_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Hasil analisis disajikan per kelompok:
        // perbandingan utama dalam batang dan peru...; bagian 2: ”Analysis results are organized by group: key comparisons use bars and sequentia... dalam
        // AddPlayers.
        terms["players.details.derived_subtitle"] = ("Hasil analisis disajikan per kelompok: perbandingan utama dalam batang dan perubahan berurutan dalam garis. Arahkan kursor atau ketuk batang atau titik untuk melihat fungsi, asal data, dan nilainya.", "Analysis results are organized by group: key comparisons use bars and sequential changes use lines. Hover over or tap a bar or point to view its function, source, and value.");
        // Memperbarui `terms[”players.details.chart_count_suffix”]` menggunakan tuple yang membawa bagian 1: ”diagram”; bagian 2: ”charts” dalam
        // AddPlayers.
        terms["players.details.chart_count_suffix"] = ("diagram", "charts");
        // Memperbarui `terms[”players.details.chart_domain_empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada titik numerik untuk divisualkan
        // pada domain ini.”; bagian 2: ”No numeric points are currently available for this domain.” dalam AddPlayers.
        terms["players.details.chart_domain_empty"] = ("Belum ada titik numerik untuk divisualkan pada domain ini.", "No numeric points are currently available for this domain.");
        // Memperbarui `terms[”players.details.chart_scroll_region”]` menggunakan tuple yang membawa bagian 1: ”Diagram dapat digeser secara horizontal”;
        // bagian 2: ”Charts can be scrolled horizontally” dalam AddPlayers.
        terms["players.details.chart_scroll_region"] = ("Diagram dapat digeser secara horizontal", "Charts can be scrolled horizontally");
        // Memperbarui `terms[”players.details.chart_scroll_hint”]` menggunakan tuple yang membawa bagian 1: ”Geser ke samping jika daftar diagram melebihi
        // lebar panel.”; bagian 2: ”Scroll sideways if the chart list exceeds the panel width.” dalam AddPlayers.
        terms["players.details.chart_scroll_hint"] = ("Geser ke samping jika daftar diagram melebihi lebar panel.", "Scroll sideways if the chart list exceeds the panel width.");
        // Memperbarui `terms[”players.details.raw.coin_finance.title”]` menggunakan tuple yang membawa bagian 1: ”Koin dan Keuangan”; bagian 2: ”Coins and
        // Finances” dalam AddPlayers.
        terms["players.details.raw.coin_finance.title"] = ("Koin dan Keuangan", "Coins and Finances");
        // Memperbarui `terms[”players.details.raw.coin_finance.desc”]` menggunakan tuple yang membawa bagian 1: ”Koin Awal, Koin Tersisa, riwayat
        // transaksi, perjalanan saldo, serta waktu pinja...; bagian 2: ”Starting Coins, Remaining Coins, transaction history, balance journey, and when...
        // dalam AddPlayers.
        terms["players.details.raw.coin_finance.desc"] = ("Koin Awal, Koin Tersisa, riwayat transaksi, perjalanan saldo, serta waktu pinjaman, risiko, dan aktivitas terakhir tercatat.", "Starting Coins, Remaining Coins, transaction history, balance journey, and when the first loan, first risk, and latest activity were recorded.");
        // Memperbarui `terms[”players.details.raw.coin_finance.beginner.desc”]` menggunakan tuple yang membawa bagian 1: ”Koin Awal, Koin Tersisa, riwayat
        // transaksi, perjalanan saldo, dan aktivitas ter...; bagian 2: ”Starting Coins, Remaining Coins, transaction history, balance journey, and the ...
        // dalam AddPlayers.
        terms["players.details.raw.coin_finance.beginner.desc"] = ("Koin Awal, Koin Tersisa, riwayat transaksi, perjalanan saldo, dan aktivitas terakhir tercatat.", "Starting Coins, Remaining Coins, transaction history, balance journey, and the latest recorded activity.");
        // Memperbarui `terms[”players.details.raw.ingredients.title”]` menggunakan tuple yang membawa bagian 1: ”Kartu Bahan”; bagian 2: ”Ingredient Cards”
        // dalam AddPlayers.
        terms["players.details.raw.ingredients.title"] = ("Kartu Bahan", "Ingredient Cards");
        // Memperbarui `terms[”players.details.raw.ingredients.desc”]` menggunakan tuple yang membawa bagian 1: ”Jumlah bahan yang diperoleh, digunakan
        // untuk tiap pesanan, masih tersisa, serta...; bagian 2: ”Ingredients collected, used for each order, still remaining, and their purchase... dalam
        // AddPlayers.
        terms["players.details.raw.ingredients.desc"] = ("Jumlah bahan yang diperoleh, digunakan untuk tiap pesanan, masih tersisa, serta biaya pembeliannya.", "Ingredients collected, used for each order, still remaining, and their purchase cost.");
        // Memperbarui `terms[”players.details.order_number”]` menggunakan tuple yang membawa bagian 1: ”Pesanan ke-”; bagian 2: ”Order No.” dalam
        // AddPlayers.
        terms["players.details.order_number"] = ("Pesanan ke-", "Order No.");
        // Memperbarui `terms[”players.details.ingredients.cards_used”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Kartu Bahan yang Digunakan”;
        // bagian 2: ”Ingredient Cards Used” dalam AddPlayers.
        terms["players.details.ingredients.cards_used"] = ("Jumlah Kartu Bahan yang Digunakan", "Ingredient Cards Used");
        // Memperbarui `terms[”players.details.meal_orders.income”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan yang Dihasilkan”; bagian 2:
        // ”Income Earned” dalam AddPlayers.
        terms["players.details.meal_orders.income"] = ("Pendapatan yang Dihasilkan", "Income Earned");
        // Memperbarui `terms[”players.details.ingredients.discarded_note”]` menggunakan tuple yang membawa bagian 1: ”Sebanyak {0} kartu bahan dibuang
        // tanpa dipakai dan tidak termasuk jumlah tersis...; bagian 2: ”{0} ingredient cards were discarded unused and are not included in the remainin...
        // dalam AddPlayers.
        terms["players.details.ingredients.discarded_note"] = ("Sebanyak {0} kartu bahan dibuang tanpa dipakai dan tidak termasuk jumlah tersisa.", "{0} ingredient cards were discarded unused and are not included in the remaining count.");
        // Memperbarui `terms[”players.details.raw.meal_orders.title”]` menggunakan tuple yang membawa bagian 1: ”Pesanan Makanan”; bagian 2: ”Meal Orders”
        // dalam AddPlayers.
        terms["players.details.raw.meal_orders.title"] = ("Pesanan Makanan", "Meal Orders");
        // Memperbarui `terms[”players.details.raw.meal_orders.desc”]` menggunakan tuple yang membawa bagian 1: ”Jumlah pesanan selesai, pendapatan setiap
        // pesanan, dan rata-rata pesanan per ha...; bagian 2: ”Completed order count, income from each order, and average orders per day on wh... dalam
        // AddPlayers.
        terms["players.details.raw.meal_orders.desc"] = ("Jumlah pesanan selesai, pendapatan setiap pesanan, dan rata-rata pesanan per hari ketika pemain melakukan aksi.", "Completed order count, income from each order, and average orders per day when the player took actions.");
        // Memperbarui `terms[”players.details.raw.needs.title”]` menggunakan tuple yang membawa bagian 1: ”Kartu Kebutuhan”; bagian 2: ”Need Cards” dalam
        // AddPlayers.
        terms["players.details.raw.needs.title"] = ("Kartu Kebutuhan", "Need Cards");
        // Memperbarui `terms[”players.details.raw.needs.desc”]` menggunakan tuple yang membawa bagian 1: ”Kebutuhan primer, sekunder, tersier, biaya, dan
        // hasil misi koleksi.”; bagian 2: ”Primary, secondary, and tertiary needs, costs, and collection-mission results.” dalam AddPlayers.
        terms["players.details.raw.needs.desc"] = ("Kebutuhan primer, sekunder, tersier, biaya, dan hasil misi koleksi.", "Primary, secondary, and tertiary needs, costs, and collection-mission results.");
        // Memperbarui `terms[”players.details.raw.donations.title”]` menggunakan tuple yang membawa bagian 1: ”Donasi”; bagian 2: ”Donations” dalam
        // AddPlayers.
        terms["players.details.raw.donations.title"] = ("Donasi", "Donations");
        // Memperbarui `terms[”players.details.raw.donations.desc”]` menggunakan tuple yang membawa bagian 1: ”Jumlah donasi, peringkat, kartu juara, dan
        // Poin Kebahagiaan.”; bagian 2: ”Donation amounts, ranks, champion cards, and Happiness Points.” dalam AddPlayers.
        terms["players.details.raw.donations.desc"] = ("Jumlah donasi, peringkat, kartu juara, dan Poin Kebahagiaan.", "Donation amounts, ranks, champion cards, and Happiness Points.");
        // Memperbarui `terms[”players.details.raw.gold.title”]` menggunakan tuple yang membawa bagian 1: ”Investasi Emas”; bagian 2: ”Gold Investment”
        // dalam AddPlayers.
        terms["players.details.raw.gold.title"] = ("Investasi Emas", "Gold Investment");
        // Memperbarui `terms[”players.details.raw.gold.desc”]` menggunakan tuple yang membawa bagian 1: ”Pembelian, penjualan, sisa kartu emas, dan selisih
        // koin masuk dan keluar dari e...; bagian 2: ”Gold purchases, sales, remaining cards, and the difference between incoming and... dalam AddPlayers.
        terms["players.details.raw.gold.desc"] = ("Pembelian, penjualan, sisa kartu emas, dan selisih koin masuk dan keluar dari emas.", "Gold purchases, sales, remaining cards, and the difference between incoming and outgoing coins from gold.");
        // Memperbarui `terms[”players.details.raw.pension.title”]` menggunakan tuple yang membawa bagian 1: ”Dana Pensiun”; bagian 2: ”Pension Fund” dalam
        // AddPlayers.
        terms["players.details.raw.pension.title"] = ("Dana Pensiun", "Pension Fund");
        // Memperbarui `terms[”players.details.raw.pension.desc”]` menggunakan tuple yang membawa bagian 1: ”Nilai kartu bahan tersisa, koin dalam tabungan,
        // total dana, peringkat, dan Poin...; bagian 2: ”Remaining ingredient-card value, coins in savings, total fund, rank, and Pensio... dalam
        // AddPlayers.
        terms["players.details.raw.pension.desc"] = ("Nilai kartu bahan tersisa, koin dalam tabungan, total dana, peringkat, dan Poin Kebahagiaan Dana Pensiun.", "Remaining ingredient-card value, coins in savings, total fund, rank, and Pension Happiness Points.");
        // Memperbarui `terms[”players.details.raw.life_risk.title”]` menggunakan tuple yang membawa bagian 1: ”Risiko Kehidupan”; bagian 2: ”Life Risks”
        // dalam AddPlayers.
        terms["players.details.raw.life_risk.title"] = ("Risiko Kehidupan", "Life Risks");
        // Memperbarui `terms[”players.details.raw.life_risk.desc”]` menggunakan tuple yang membawa bagian 1: ”Kartu Risiko Kehidupan yang muncul, nominal
        // dampak pada kartu, perlindungan asu...; bagian 2: ”Life Risk Cards drawn, nominal card impact, insurance coverage, and Emergency A... dalam
        // AddPlayers.
        terms["players.details.raw.life_risk.desc"] = ("Kartu Risiko Kehidupan yang muncul, nominal dampak pada kartu, perlindungan asuransi, dan Tindakan Darurat. Arus kas nyatanya tercatat pada Riwayat Transaksi.", "Life Risk Cards drawn, nominal card impact, insurance coverage, and Emergency Actions. Actual cashflow is recorded in Transaction History.");
        // Memperbarui `terms[”players.details.raw.financial_goals.title”]` menggunakan tuple yang membawa bagian 1: ”Target Finansial”; bagian 2:
        // ”Financial Goals” dalam AddPlayers.
        terms["players.details.raw.financial_goals.title"] = ("Target Finansial", "Financial Goals");
        // Memperbarui `terms[”players.details.raw.financial_goals.desc”]` menggunakan tuple yang membawa bagian 1: ”Hasil target, tabungan yang tersisa,
        // dan pinjaman yang belum dilunasi.”; bagian 2: ”Goal results, remaining savings, and outstanding loans.” dalam AddPlayers.
        terms["players.details.raw.financial_goals.desc"] = ("Hasil target, tabungan yang tersisa, dan pinjaman yang belum dilunasi.", "Goal results, remaining savings, and outstanding loans.");
        // Memperbarui `terms[”players.details.financial_goals.attempted_note”]` menggunakan tuple yang membawa bagian 1: ”Dari {0} target yang mulai
        // didanai.”; bagian 2: ”Out of {0} goals that received funding.” dalam AddPlayers.
        terms["players.details.financial_goals.attempted_note"] = ("Dari {0} target yang mulai didanai.", "Out of {0} goals that received funding.");
        // Memperbarui `terms[”players.details.raw.actions.title”]` menggunakan tuple yang membawa bagian 1: ”Penggunaan Aksi”; bagian 2: ”Action Use” dalam
        // AddPlayers.
        terms["players.details.raw.actions.title"] = ("Penggunaan Aksi", "Action Use");
        // Memperbarui `terms[”players.details.raw.actions.desc”]` menggunakan tuple yang membawa bagian 1: ”Aksi utama setiap hari, jumlah jenis aksi yang
        // berbeda, dan pengulangannya.”; bagian 2: ”Main actions each day, distinct action types, and repetitions.” dalam AddPlayers.
        terms["players.details.raw.actions.desc"] = ("Setiap kegiatan dalam tabel dihitung sebagai satu aksi. Lihat jumlah aksi setiap hari dan kegiatan yang dipilih pemain.", "Each activity in this table counts as one action. See the number of actions each day and the activities the player chose.");
        // Memperbarui `terms[”players.details.derived.financial.title”]` menggunakan tuple yang membawa bagian 1: ”Analisis Performa Finansial”; bagian 2:
        // ”Financial Performance Analysis” dalam AddPlayers.
        terms["players.details.derived.financial.title"] = ("Analisis Performa Finansial", "Financial Performance Analysis");
        // Memperbarui `terms[”players.details.derived.financial.desc”]` menggunakan tuple yang membawa bagian 1: ”Perbandingan koin, pemerataan pendapatan,
        // persentase pengeluaran untuk bahan, d...; bagian 2: ”Coin comparison, income balance, ingredient spending share, and order profit ma... dalam
        // AddPlayers.
        terms["players.details.derived.financial.desc"] = ("Perbandingan koin, pemerataan pendapatan, persentase pengeluaran untuk bahan, dan persentase laba pesanan.", "Coin comparison, income balance, ingredient spending share, and order profit margin.");
        // Memperbarui `terms[”players.details.derived.strategy.title”]` menggunakan tuple yang membawa bagian 1: ”Analisis Keputusan Strategis”; bagian 2:
        // ”Strategic Decision Analysis” dalam AddPlayers.
        terms["players.details.derived.strategy.title"] = ("Analisis Keputusan Strategis", "Strategic Decision Analysis");
        // Memperbarui `terms[”players.details.derived.strategy.desc”]` menggunakan tuple yang membawa bagian 1: ”Risiko tanpa tindakan darurat, porsi sisa
        // pinjaman, dan progres pendanaan targe...; bagian 2: ”Risks without emergency action, outstanding loan share, and financial goal fund... dalam
        // AddPlayers.
        terms["players.details.derived.strategy.desc"] = ("Risiko tanpa tindakan darurat, porsi sisa pinjaman, dan progres pendanaan target finansial.", "Risks without emergency action, outstanding loan share, and financial goal funding progress.");
        // Memperbarui `terms[”players.details.derived.behavior.title”]` menggunakan tuple yang membawa bagian 1: ”Analisis Perilaku Pemain”; bagian 2:
        // ”Player Behavior Analysis” dalam AddPlayers.
        terms["players.details.derived.behavior.title"] = ("Analisis Perilaku Pemain", "Player Behavior Analysis");
        // Memperbarui `terms[”players.details.derived.behavior.desc”]` menggunakan tuple yang membawa bagian 1: ”Persentase aksi utama penghasil koin,
        // pemanfaatan bahan, dan aksi jangka panjan...; bagian 2: ”Income-earning main action share, ingredient utilization, and long-term actions... dalam
        // AddPlayers.
        terms["players.details.derived.behavior.desc"] = ("Penggunaan aksi untuk mendapatkan koin, pemanfaatan bahan, serta penggunaan aksi untuk tabungan, target, asuransi, dan pelunasan.", "Action actions spent to earn coins, ingredient use, and actions spent on savings, goals, insurance, and repayment.");
        // Memperbarui `terms[”players.details.derived.flourishing.title”]` menggunakan tuple yang membawa bagian 1: ”Analisis Kesejahteraan Pemain”; bagian
        // 2: ”Player Well-Being Analysis” dalam AddPlayers.
        terms["players.details.derived.flourishing.title"] = ("Analisis Kesejahteraan Pemain", "Player Well-Being Analysis");
        // Memperbarui `terms[”players.details.derived.flourishing.desc”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan kartu kebutuhan, skor
        // komitmen donasi, dan komposisi Poin Kebahagiaa...; bagian 2: ”Need card balance, donation commitment score, and Happiness Point composition.”
        // dalam AddPlayers.
        terms["players.details.derived.flourishing.desc"] = ("Pemerataan kartu kebutuhan, skor komitmen donasi, dan komposisi Poin Kebahagiaan.", "Need card balance, donation commitment score, and Happiness Point composition.");
        // Memperbarui `terms[”players.gameplay_metrics”]` menggunakan tuple yang membawa bagian 1: ”Data Permainan”; bagian 2: ”Gameplay Data” dalam
        // AddPlayers.
        terms["players.gameplay_metrics"] = ("Data Permainan", "Gameplay Data");
        // Memperbarui `terms[”players.gameplay_all_fields”]` menggunakan tuple yang membawa bagian 1: ”Tampilkan Semua Data”; bagian 2: ”Show All Data”
        // dalam AddPlayers.
        terms["players.gameplay_all_fields"] = ("Tampilkan Semua Data", "Show All Data");
        // Memperbarui `terms[”players.metrics_raw_desc”]` menggunakan tuple yang membawa bagian 1: ”Hanya catatan permainan yang membantu menjelaskan hasil
        // dan belum ditampilkan p...; bagian 2: ”Only gameplay records that help explain the result and are not already shown el... dalam AddPlayers.
        terms["players.metrics_raw_desc"] = ("Hanya catatan permainan yang membantu menjelaskan hasil dan belum ditampilkan pada bagian lain.", "Only gameplay records that help explain the result and are not already shown elsewhere.");
        // Memperbarui `terms[”players.metrics_derived_desc”]` menggunakan tuple yang membawa bagian 1: ”Hasil analisis utama dikelompokkan tanpa mengulang
        // data pembentuk yang sudah te...; bagian 2: ”Primary analysis results are grouped without repeating source data that is alre... dalam AddPlayers.
        terms["players.metrics_derived_desc"] = ("Hasil analisis utama dikelompokkan tanpa mengulang data pembentuk yang sudah tersedia.", "Primary analysis results are grouped without repeating source data that is already available.");
        // Memperbarui `terms[”players.metric_items_suffix”]` menggunakan tuple yang membawa bagian 1: ”item”; bagian 2: ”items” dalam AddPlayers.
        terms["players.metric_items_suffix"] = ("item", "items");
        // Memperbarui `terms[”players.group.summary”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Utama”; bagian 2: ”Main Summary” dalam
        // AddPlayers.
        terms["players.group.summary"] = ("Ringkasan Utama", "Main Summary");
        // Memperbarui `terms[”players.group.coins”]` menggunakan tuple yang membawa bagian 1: ”Koin dan Arus Kas”; bagian 2: ”Coins and Cashflow” dalam
        // AddPlayers.
        terms["players.group.coins"] = ("Koin dan Arus Kas", "Coins and Cashflow");
        // Memperbarui `terms[”players.group.ingredients”]` menggunakan tuple yang membawa bagian 1: ”Bahan”; bagian 2: ”Ingredients” dalam AddPlayers.
        terms["players.group.ingredients"] = ("Bahan", "Ingredients");
        // Memperbarui `terms[”players.group.meal_orders”]` menggunakan tuple yang membawa bagian 1: ”Pesanan Makanan”; bagian 2: ”Meal Orders” dalam
        // AddPlayers.
        terms["players.group.meal_orders"] = ("Pesanan Makanan", "Meal Orders");
        // Memperbarui `terms[”players.group.needs”]` menggunakan tuple yang membawa bagian 1: ”Kebutuhan”; bagian 2: ”Needs” dalam AddPlayers.
        terms["players.group.needs"] = ("Kebutuhan", "Needs");
        // Memperbarui `terms[”players.group.donations”]` menggunakan tuple yang membawa bagian 1: ”Donasi”; bagian 2: ”Donations” dalam AddPlayers.
        terms["players.group.donations"] = ("Donasi", "Donations");
        // Memperbarui `terms[”players.group.gold”]` menggunakan tuple yang membawa bagian 1: ”Investasi Emas”; bagian 2: ”Gold Investment” dalam
        // AddPlayers.
        terms["players.group.gold"] = ("Investasi Emas", "Gold Investment");
        // Memperbarui `terms[”players.group.pension”]` menggunakan tuple yang membawa bagian 1: ”Dana Pensiun”; bagian 2: ”Pension Fund” dalam AddPlayers.
        terms["players.group.pension"] = ("Dana Pensiun", "Pension Fund");
        // Memperbarui `terms[”players.group.life_risk”]` menggunakan tuple yang membawa bagian 1: ”Risiko Hidup”; bagian 2: ”Life Risk” dalam AddPlayers.
        terms["players.group.life_risk"] = ("Risiko Hidup", "Life Risk");
        // Memperbarui `terms[”players.group.financial_goals”]` menggunakan tuple yang membawa bagian 1: ”Target Finansial”; bagian 2: ”Financial Goals”
        // dalam AddPlayers.
        terms["players.group.financial_goals"] = ("Target Finansial", "Financial Goals");
        // Memperbarui `terms[”players.group.actions”]` menggunakan tuple yang membawa bagian 1: ”Aksi”; bagian 2: ”Actions” dalam AddPlayers.
        terms["players.group.actions"] = ("Aksi", "Actions");
        // Memperbarui `terms[”players.group.turns”]` menggunakan tuple yang membawa bagian 1: ”Progres Giliran”; bagian 2: ”Turn Progress” dalam
        // AddPlayers.
        terms["players.group.turns"] = ("Progres Giliran", "Turn Progress");
        // Memperbarui `terms[”players.group.notes”]` menggunakan tuple yang membawa bagian 1: ”Catatan Sistem”; bagian 2: ”System Notes” dalam AddPlayers.
        terms["players.group.notes"] = ("Catatan Sistem", "System Notes");
        // Memperbarui `terms[”players.group.expense_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Belanja Bahan”; bagian 2: ”Ingredient
        // Spending Details” dalam AddPlayers.
        terms["players.group.expense_components"] = ("Rincian Belanja Bahan", "Ingredient Spending Details");
        // Memperbarui `terms[”players.group.risk_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Risiko”; bagian 2: ”Risk Details” dalam
        // AddPlayers.
        terms["players.group.risk_components"] = ("Rincian Risiko", "Risk Details");
        // Memperbarui `terms[”players.group.happiness_portfolio”]` menggunakan tuple yang membawa bagian 1: ”Sumber Poin Kebahagiaan”; bagian 2: ”Happiness
        // Point Sources” dalam AddPlayers.
        terms["players.group.happiness_portfolio"] = ("Sumber Poin Kebahagiaan", "Happiness Point Sources");
        // Memperbarui `terms[”players.group.income_diversification_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Sumber Pendapatan”;
        // bagian 2: ”Income Source Details” dalam AddPlayers.
        terms["players.group.income_diversification_components"] = ("Rincian Sumber Pendapatan", "Income Source Details");
        // Memperbarui `terms[”players.group.expense_management_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Pengeluaran”; bagian 2:
        // ”Spending Details” dalam AddPlayers.
        terms["players.group.expense_management_components"] = ("Rincian Pengeluaran", "Spending Details");
        // Memperbarui `terms[”players.group.risk_appetite_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Risiko yang Diambil”; bagian 2:
        // ”Risk Taken Details” dalam AddPlayers.
        terms["players.group.risk_appetite_components"] = ("Rincian Risiko yang Diambil", "Risk Taken Details");
        // Memperbarui `terms[”players.group.goal_setting_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Target”; bagian 2: ”Goal Details”
        // dalam AddPlayers.
        terms["players.group.goal_setting_components"] = ("Rincian Target", "Goal Details");
        // Memperbarui `terms[”players.group.fulfillment_diversity_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Kebutuhan”; bagian 2:
        // ”Need Details” dalam AddPlayers.
        terms["players.group.fulfillment_diversity_components"] = ("Rincian Kebutuhan", "Need Details");
        // Memperbarui `terms[”players.group.donation_commitment_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Donasi”; bagian 2:
        // ”Donation Details” dalam AddPlayers.
        terms["players.group.donation_commitment_components"] = ("Rincian Donasi", "Donation Details");
        // Memperbarui `terms[”players.group.action_efficiency_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Penggunaan Aksi”; bagian 2:
        // ”Action Use Details” dalam AddPlayers.
        terms["players.group.action_efficiency_components"] = ("Rincian Penggunaan Aksi", "Action Use Details");
        // Memperbarui `terms[”players.group.planning_horizon_components”]` menggunakan tuple yang membawa bagian 1: ”Rincian Aksi Masa Depan”; bagian 2:
        // ”Future Action Details” dalam AddPlayers.
        terms["players.group.planning_horizon_components"] = ("Rincian Aksi untuk Tabungan, Target, Asuransi, dan Pelunasan", "Action Details for Savings, Goals, Insurance, and Repayment");
        // Memperbarui `terms[”players.metric.income_diversification”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan Sumber Pendapatan”; bagian 2:
        // ”Income Source Balance” dalam AddPlayers.
        terms["players.metric.income_diversification"] = ("Pemerataan Sumber Pendapatan", "Income Source Balance");
        // Memperbarui `terms[”players.metric.expense_efficiency”]` menggunakan tuple yang membawa bagian 1: ”Belanja untuk Bahan Terpakai”; bagian 2:
        // ”Spending on Used Ingredients” dalam AddPlayers.
        terms["players.metric.expense_efficiency"] = ("Belanja untuk Bahan Terpakai", "Spending on Used Ingredients");
        // Memperbarui `terms[”players.metric.business_profit_margin”]` menggunakan tuple yang membawa bagian 1: ”Persentase Laba Pesanan”; bagian 2: ”Order
        // Profit Margin” dalam AddPlayers.
        terms["players.metric.business_profit_margin"] = ("Persentase Laba Pesanan", "Order Profit Margin");
        // Memperbarui `terms[”players.metric.growth_pattern_ratio”]` menggunakan tuple yang membawa bagian 1: ”Rasio Pola Pertumbuhan”; bagian 2: ”Growth
        // Pattern Ratio” dalam AddPlayers.
        terms["players.metric.growth_pattern_ratio"] = ("Rasio Pola Pertumbuhan", "Growth Pattern Ratio");
        // Memperbarui `terms[”players.metric.risk_appetite”]` menggunakan tuple yang membawa bagian 1: ”Tingkat Risiko yang Diambil”; bagian 2: ”Risk Taken
        // Level” dalam AddPlayers.
        terms["players.metric.risk_appetite"] = ("Tingkat Risiko yang Diambil", "Risk Taken Level");
        // Memperbarui `terms[”players.metric.debt_leverage”]` menggunakan tuple yang membawa bagian 1: ”Beban Utang terhadap Kas”; bagian 2: ”Debt Burden
        // Compared with Cash” dalam AddPlayers.
        terms["players.metric.debt_leverage"] = ("Beban Utang terhadap Kas", "Debt Burden Compared with Cash");
        // Memperbarui `terms[”players.metric.meal_order_success”]` menggunakan tuple yang membawa bagian 1: ”Pesanan yang Berhasil”; bagian 2: ”Successful
        // Orders” dalam AddPlayers.
        terms["players.metric.meal_order_success"] = ("Pesanan yang Berhasil", "Successful Orders");
        // Memperbarui `terms[”players.metric.action_efficiency”]` menggunakan tuple yang membawa bagian 1: ”Persentase Aksi Utama Penghasil Koin”; bagian
        // 2: ”Income-Earning Main Action Share” dalam AddPlayers.
        terms["players.metric.action_efficiency"] = ("Persentase Aksi untuk Mendapatkan Koin", "Share of Actions Used to Earn Coins");
        // Memperbarui `terms[”players.metric.planning_horizon”]` menggunakan tuple yang membawa bagian 1: ”Persentase Aksi Utama untuk Jangka Panjang”;
        // bagian 2: ”Long-Term Main Action Share” dalam AddPlayers.
        terms["players.metric.planning_horizon"] = ("Persentase Aksi untuk Tabungan, Target, Asuransi, dan Pelunasan", "Share of Actions for Savings, Goals, Insurance, and Repayment");
        // Memperbarui `terms[”players.metric.donation_aggressiveness”]` menggunakan tuple yang membawa bagian 1: ”Agresivitas Donasi”; bagian 2: ”Donation
        // Aggressiveness” dalam AddPlayers.
        terms["players.metric.donation_aggressiveness"] = ("Agresivitas Donasi", "Donation Aggressiveness");
        // Memperbarui `terms[”players.metric.gold_roi”]` menggunakan tuple yang membawa bagian 1: ”ROI Emas”; bagian 2: ”Gold ROI” dalam AddPlayers.
        terms["players.metric.gold_roi"] = ("ROI Emas", "Gold ROI");
        // Memperbarui `terms[”players.metric.mission_achievement”]` menggunakan tuple yang membawa bagian 1: ”Pencapaian Misi”; bagian 2: ”Mission
        // Achievement” dalam AddPlayers.
        terms["players.metric.mission_achievement"] = ("Pencapaian Misi", "Mission Achievement");
        // Memperbarui `terms[”players.metric.fulfillment_diversity”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan Kartu Kebutuhan”; bagian 2:
        // ”Need Card Balance” dalam AddPlayers.
        terms["players.metric.fulfillment_diversity"] = ("Pemerataan Kartu Kebutuhan", "Need Card Balance");
        // Memperbarui `terms[”players.metric.donation_commitment”]` menggunakan tuple yang membawa bagian 1: ”Skor Komitmen Donasi”; bagian 2: ”Donation
        // Commitment Score” dalam AddPlayers.
        terms["players.metric.donation_commitment"] = ("Skor Komitmen Donasi", "Donation Commitment Score");
        // Memperbarui `terms[”players.metric.debt_ratio”]` menggunakan tuple yang membawa bagian 1: ”Rasio Utang”; bagian 2: ”Debt Ratio” dalam AddPlayers.
        terms["players.metric.debt_ratio"] = ("Rasio Utang", "Debt Ratio");
        // Memperbarui `terms[”players.metric.risk_exposure”]` menggunakan tuple yang membawa bagian 1: ”Eksposur Risiko”; bagian 2: ”Risk Exposure” dalam
        // AddPlayers.
        terms["players.metric.risk_exposure"] = ("Eksposur Risiko", "Risk Exposure");
        // Memperbarui `terms[”players.metric.risk_mitigation”]` menggunakan tuple yang membawa bagian 1: ”Mitigasi Risiko”; bagian 2: ”Risk Mitigation”
        // dalam AddPlayers.
        terms["players.metric.risk_mitigation"] = ("Mitigasi Risiko", "Risk Mitigation");
        // Memperbarui `terms[”players.metric.loan_repayment_discipline”]` menggunakan tuple yang membawa bagian 1: ”Disiplin Pelunasan Pinjaman”; bagian 2:
        // ”Loan Repayment Discipline” dalam AddPlayers.
        terms["players.metric.loan_repayment_discipline"] = ("Disiplin Pelunasan Pinjaman", "Loan Repayment Discipline");
        // Memperbarui `terms[”players.metric.basic_need_profile”]` menggunakan tuple yang membawa bagian 1: ”Memiliki Primer, Sekunder, dan Tersier”;
        // bagian 2: ”Has Primary, Secondary, and Tertiary” dalam AddPlayers.
        terms["players.metric.basic_need_profile"] = ("Memiliki Primer, Sekunder, dan Tersier", "Has Primary, Secondary, and Tertiary");
        // Memperbarui `terms[”players.metric.collector_need_profile”]` menggunakan tuple yang membawa bagian 1: ”Memiliki Minimal 4 Jenis Kebutuhan”;
        // bagian 2: ”Has at Least 4 Need Types” dalam AddPlayers.
        terms["players.metric.collector_need_profile"] = ("Memiliki Minimal 4 Jenis Kebutuhan", "Has at Least 4 Need Types");
        // Memperbarui `terms[”players.metric.specialist_need_profile”]` menggunakan tuple yang membawa bagian 1: ”Minimal 70% Berasal dari Satu Kategori”;
        // bagian 2: ”At Least 70% from One Category” dalam AddPlayers.
        terms["players.metric.specialist_need_profile"] = ("Minimal 70% Berasal dari Satu Kategori", "At Least 70% from One Category");
        // Memperbarui `terms[”players.raw.economy”]` menggunakan tuple yang membawa bagian 1: ”Ekonomi”; bagian 2: ”Economy” dalam AddPlayers.
        terms["players.raw.economy"] = ("Ekonomi", "Economy");
        // Memperbarui `terms[”players.raw.activity”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas”; bagian 2: ”Activity” dalam AddPlayers.
        terms["players.raw.activity"] = ("Aktivitas", "Activity");
        // Memperbarui `terms[”players.raw.commitment”]` menggunakan tuple yang membawa bagian 1: ”Komitmen”; bagian 2: ”Commitment” dalam AddPlayers.
        terms["players.raw.commitment"] = ("Komitmen", "Commitment");
        // Memperbarui `terms[”players.raw.starting_coins”]` menggunakan tuple yang membawa bagian 1: ”Koin Awal”; bagian 2: ”Starting Coins” dalam
        // AddPlayers.
        terms["players.raw.starting_coins"] = ("Koin Awal", "Starting Coins");
        // Memperbarui `terms[”players.raw.cash_in_total”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Masuk”; bagian 2: ”Total Incoming Coins”
        // dalam AddPlayers.
        terms["players.raw.cash_in_total"] = ("Total Koin Masuk", "Total Incoming Coins");
        // Memperbarui `terms[”players.raw.cash_out_total”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Keluar”; bagian 2: ”Total Outgoing Coins”
        // dalam AddPlayers.
        terms["players.raw.cash_out_total"] = ("Total Koin Keluar", "Total Outgoing Coins");
        // Memperbarui `terms[”players.raw.coins_earned_total”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Diperoleh”; bagian 2: ”Total Coins
        // Earned” dalam AddPlayers.
        terms["players.raw.coins_earned_total"] = ("Total Koin Diperoleh", "Total Coins Earned");
        // Memperbarui `terms[”players.raw.coins_spent_total”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Dikeluarkan”; bagian 2: ”Total Coins
        // Spent” dalam AddPlayers.
        terms["players.raw.coins_spent_total"] = ("Total Koin Dikeluarkan", "Total Coins Spent");
        // Memperbarui `terms[”players.raw.coins_held_current”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa”; bagian 2: ”Remaining Coins” dalam
        // AddPlayers.
        terms["players.raw.coins_held_current"] = ("Koin Tersisa", "Remaining Coins");
        // Memperbarui `terms[”players.raw.coins_saved”]` menggunakan tuple yang membawa bagian 1: ”Koin dalam Tabungan”; bagian 2: ”Coins in Savings” dalam
        // AddPlayers.
        terms["players.raw.coins_saved"] = ("Koin dalam Tabungan", "Coins in Savings");
        // Memperbarui `terms[”players.raw.coins_donated”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Donasi”; bagian 2: ”Total Donated Coins”
        // dalam AddPlayers.
        terms["players.raw.coins_donated"] = ("Total Koin Donasi", "Total Donated Coins");
        // Memperbarui `terms[”players.raw.coins_net_end”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa”; bagian 2: ”Remaining Coins” dalam
        // AddPlayers.
        terms["players.raw.coins_net_end"] = ("Koin Tersisa", "Remaining Coins");
        // Memperbarui `terms[”players.raw.coins_net_end_game”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa”; bagian 2: ”Remaining Coins” dalam
        // AddPlayers.
        terms["players.raw.coins_net_end_game"] = ("Koin Tersisa", "Remaining Coins");
        // Memperbarui `terms[”players.raw.meal_order_income_total”]` menggunakan tuple yang membawa bagian 1: ”Total Pendapatan Pesanan”; bagian 2: ”Total
        // Order Income” dalam AddPlayers.
        terms["players.raw.meal_order_income_total"] = ("Total Pendapatan Pesanan", "Total Order Income");
        // Memperbarui `terms[”players.raw.ingredients_collected”]` menggunakan tuple yang membawa bagian 1: ”Bahan Terkumpul”; bagian 2: ”Ingredients
        // Collected” dalam AddPlayers.
        terms["players.raw.ingredients_collected"] = ("Bahan Terkumpul", "Ingredients Collected");
        // Memperbarui `terms[”players.raw.ingredients_held_current”]` menggunakan tuple yang membawa bagian 1: ”Total Bahan Tersisa”; bagian 2: ”Total
        // Remaining Ingredients” dalam AddPlayers.
        terms["players.raw.ingredients_held_current"] = ("Total Bahan Tersisa", "Total Remaining Ingredients");
        // Memperbarui `terms[”players.raw.ingredients_wasted”]` menggunakan tuple yang membawa bagian 1: ”Bahan Dibuang tanpa Dipakai”; bagian 2:
        // ”Ingredients Discarded Unused” dalam AddPlayers.
        terms["players.raw.ingredients_wasted"] = ("Bahan Dibuang tanpa Dipakai", "Ingredients Discarded Unused");
        // Memperbarui `terms[”players.raw.meal_orders_claimed”]` menggunakan tuple yang membawa bagian 1: ”Pesanan Selesai”; bagian 2: ”Completed Orders”
        // dalam AddPlayers.
        terms["players.raw.meal_orders_claimed"] = ("Pesanan Selesai", "Completed Orders");
        // Memperbarui `terms[”players.raw.need_cards_purchased”]` menggunakan tuple yang membawa bagian 1: ”Kartu Kebutuhan Dibeli”; bagian 2: ”Need Cards
        // Purchased” dalam AddPlayers.
        terms["players.raw.need_cards_purchased"] = ("Kartu Kebutuhan Dibeli", "Need Cards Purchased");
        // Memperbarui `terms[”players.raw.needs_breakdown”]` menggunakan tuple yang membawa bagian 1: ”Primer / Sekunder / Tersier”; bagian 2: ”Primary /
        // Secondary / Tertiary Needs” dalam AddPlayers.
        terms["players.raw.needs_breakdown"] = ("Primer / Sekunder / Tersier", "Primary / Secondary / Tertiary Needs");
        // Memperbarui `terms[”players.raw.gold_cards_held_end”]` menggunakan tuple yang membawa bagian 1: ”Total Kartu Emas Tersisa”; bagian 2: ”Total
        // Remaining Gold Cards” dalam AddPlayers.
        terms["players.raw.gold_cards_held_end"] = ("Total Kartu Emas Tersisa", "Total Remaining Gold Cards");
        // Memperbarui `terms[”players.raw.loans_taken”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Pinjaman Diambil”; bagian 2: ”Loans Taken Count”
        // dalam AddPlayers.
        terms["players.raw.loans_taken"] = ("Jumlah Pinjaman Diambil", "Loans Taken Count");
        // Memperbarui `terms[”players.raw.loans_repaid”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Pinjaman Dilunasi”; bagian 2: ”Loans Repaid
        // Count” dalam AddPlayers.
        terms["players.raw.loans_repaid"] = ("Jumlah Pinjaman Dilunasi", "Loans Repaid Count");
        // Memperbarui `terms[”players.raw.loans_unpaid_end”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Pinjaman Belum Lunas”; bagian 2: ”Unpaid
        // Loan Count” dalam AddPlayers.
        terms["players.raw.loans_unpaid_end"] = ("Jumlah Pinjaman Belum Lunas", "Unpaid Loan Count");
        // Memperbarui `terms[”players.raw.collection_mission_complete”]` menggunakan tuple yang membawa bagian 1: ”Misi Koleksi Selesai”; bagian 2:
        // ”Collection Mission Complete” dalam AddPlayers.
        terms["players.raw.collection_mission_complete"] = ("Misi Koleksi Selesai", "Collection Mission Complete");
        // Memperbarui `terms[”metric.total_event”]` menggunakan tuple yang membawa bagian 1: ”Total Aktivitas Tercatat”; bagian 2: ”Total Recorded
        // Activities” dalam AddPlayers.
        terms["metric.total_event"] = ("Total Aktivitas Tercatat", "Total Recorded Activities");
        // Memperbarui `terms[”metric.cash_in”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Masuk”; bagian 2: ”Total Incoming Coins” dalam
        // AddPlayers.
        terms["metric.cash_in"] = ("Total Koin Masuk", "Total Incoming Coins");
        // Memperbarui `terms[”metric.cash_out”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Keluar”; bagian 2: ”Total Outgoing Coins” dalam
        // AddPlayers.
        terms["metric.cash_out"] = ("Total Koin Keluar", "Total Outgoing Coins");
        // Memperbarui `terms[”metric.cash_net”]` menggunakan tuple yang membawa bagian 1: ”Selisih Koin Masuk dan Keluar”; bagian 2: ”Incoming and Outgoing
        // Coin Difference” dalam AddPlayers.
        terms["metric.cash_net"] = ("Selisih Koin Masuk dan Keluar", "Incoming and Outgoing Coin Difference");
        // Memperbarui `terms[”metric.net_cashflow”]` menggunakan tuple yang membawa bagian 1: ”Selisih Koin Masuk dan Keluar”; bagian 2: ”Incoming and
        // Outgoing Coin Difference” dalam AddPlayers.
        terms["metric.net_cashflow"] = ("Selisih Koin Masuk dan Keluar", "Incoming and Outgoing Coin Difference");
        // Memperbarui `terms[”metric.donation”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Donasi”; bagian 2: ”Total Donated Coins” dalam
        // AddPlayers.
        terms["metric.donation"] = ("Total Koin Donasi", "Total Donated Coins");
        // Memperbarui `terms[”metric.gold_qty”]` menggunakan tuple yang membawa bagian 1: ”Emas yang Dimiliki”; bagian 2: ”Gold Owned” dalam AddPlayers.
        terms["metric.gold_qty"] = ("Emas yang Dimiliki", "Gold Owned");
        // Memperbarui `terms[”metric.happiness”]` menggunakan tuple yang membawa bagian 1: ”Total Poin Kebahagiaan”; bagian 2: ”Total Happiness Points”
        // dalam AddPlayers.
        terms["metric.happiness"] = ("Total Poin Kebahagiaan", "Total Happiness Points");
        // Memperbarui `terms[”metric.need_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Kartu Kebutuhan”; bagian 2: ”Need Card
        // Happiness Points” dalam AddPlayers.
        terms["metric.need_points"] = ("Poin Kebahagiaan Kartu Kebutuhan", "Need Card Happiness Points");
        // Memperbarui `terms[”metric.need_bonus”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Bonus Set Kebutuhan”; bagian 2: ”Need Set
        // Bonus Happiness Points” dalam AddPlayers.
        terms["metric.need_bonus"] = ("Poin Kebahagiaan Bonus Set Kebutuhan", "Need Set Bonus Happiness Points");
        // Memperbarui `terms[”metric.donation_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan dari Donasi”; bagian 2: ”Happiness
        // Points from Donations” dalam AddPlayers.
        terms["metric.donation_points"] = ("Poin Kebahagiaan dari Donasi", "Happiness Points from Donations");
        // Memperbarui `terms[”metric.gold_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Emas”; bagian 2: ”Gold Happiness Points”
        // dalam AddPlayers.
        terms["metric.gold_points"] = ("Poin Kebahagiaan Emas", "Gold Happiness Points");
        // Memperbarui `terms[”metric.pension_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Dana Pensiun”; bagian 2: ”Pension
        // Happiness Points” dalam AddPlayers.
        terms["metric.pension_points"] = ("Poin Kebahagiaan Dana Pensiun", "Pension Happiness Points");
        // Memperbarui `terms[”metric.saving_goal”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Target Finansial”; bagian 2: ”Financial Goal
        // Happiness Points” dalam AddPlayers.
        terms["metric.saving_goal"] = ("Poin Kebahagiaan Target Finansial", "Financial Goal Happiness Points");
        // Memperbarui `terms[”metric.loan_unpaid”]` menggunakan tuple yang membawa bagian 1: ”Status Pinjaman”; bagian 2: ”Loan Status” dalam AddPlayers.
        terms["metric.loan_unpaid"] = ("Status Pinjaman", "Loan Status");
        // Memperbarui `terms[”metric.mission_penalty”]` menggunakan tuple yang membawa bagian 1: ”Pengurangan Poin Kebahagiaan Misi”; bagian 2: ”Mission
        // Happiness Point Deduction” dalam AddPlayers.
        terms["metric.mission_penalty"] = ("Pengurangan Poin Kebahagiaan Misi", "Mission Happiness Point Deduction");
        // Memperbarui `terms[”metric.loan_penalty”]` menggunakan tuple yang membawa bagian 1: ”Pengurangan Poin Kebahagiaan Pinjaman”; bagian 2: ”Loan
        // Happiness Point Deduction” dalam AddPlayers.
        terms["metric.loan_penalty"] = ("Pengurangan Poin Kebahagiaan Pinjaman", "Loan Happiness Point Deduction");
        // Memperbarui `terms[”metric.orders_completed”]` menggunakan tuple yang membawa bagian 1: ”Pesanan Selesai”; bagian 2: ”Orders Completed” dalam
        // AddPlayers.
        terms["metric.orders_completed"] = ("Pesanan Selesai", "Orders Completed");
        // Memperbarui `terms[”metric.inventory_ingredient”]` menggunakan tuple yang membawa bagian 1: ”Bahan Tersisa”; bagian 2: ”Remaining Ingredients”
        // dalam AddPlayers.
        terms["metric.inventory_ingredient"] = ("Bahan Tersisa", "Remaining Ingredients");
        // Memperbarui `terms[”metric.fulfillment_diversity”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan Kartu Kebutuhan”; bagian 2: ”Need Card
        // Balance” dalam AddPlayers.
        terms["metric.fulfillment_diversity"] = ("Pemerataan Kartu Kebutuhan", "Need Card Balance");
        // Memperbarui `terms[”gameplay.raw”]` menggunakan tuple yang membawa bagian 1: ”Data Permainan”; bagian 2: ”Gameplay Data” dalam AddPlayers.
        terms["gameplay.raw"] = ("Data Permainan", "Gameplay Data");
        // Memperbarui `terms[”gameplay.derived”]` menggunakan tuple yang membawa bagian 1: ”Hasil Analisis”; bagian 2: ”Analysis Results” dalam AddPlayers.
        terms["gameplay.derived"] = ("Hasil Analisis", "Analysis Results");

        // Tab Helper Introductions
        // Memperbarui `terms[”players.stats.tab.summary.desc”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan semua uang masuk, semua uang keluar,
        // selisihnya, dan uang yang tersi...; bagian 2: ”Compare all money in, all money out, the difference, and the money left when th... dalam
        // AddPlayers.
        terms["players.stats.tab.summary.desc"] = ("Bandingkan semua uang masuk, semua uang keluar, selisihnya, dan uang yang tersisa saat permainan selesai.", "Compare all money in, all money out, the difference, and the money left when the game ends.");
        // Memperbarui `terms[”players.stats.tab.finance.desc”]` menggunakan tuple yang membawa bagian 1: ”Lihat perubahan saldo dari transaksi pertama
        // hingga terakhir untuk menemukan sa...; bagian 2: ”See balance changes from the first to the last transaction to find when the pla... dalam
        // AddPlayers.
        terms["players.stats.tab.finance.desc"] = ("Lihat perubahan saldo dari transaksi pertama hingga terakhir untuk menemukan saat uang pemain paling tinggi atau paling rendah.", "See balance changes from the first to the last transaction to find when the player's money was highest or lowest.");
        // Memperbarui `terms[”players.stats.tab.behavior.desc”]` menggunakan tuple yang membawa bagian 1: ”Lihat bagaimana pemain menggunakan aksi,
        // menyelesaikan pesanan, menyeimbangkan ...; bagian 2: ”See how the player used actions, completed orders, balanced need categories, an... dalam
        // AddPlayers.
        terms["players.stats.tab.behavior.desc"] = ("Lihat bagaimana pemain menggunakan aksi, menyelesaikan pesanan, menyeimbangkan kategori kebutuhan, dan mengikuti aturan.", "See how the player used actions, completed orders, balanced need categories, and followed the rules.");
        // Memperbarui `terms[”players.stats.tab.happiness.desc”]` menggunakan tuple yang membawa bagian 1: ”Lihat dari mana poin kebahagiaan akhir berasal
        // dan bagian mana yang menambah at...; bagian 2: ”See where the final score came from and what added to or reduced the player's h... dalam
        // AddPlayers.
        terms["players.stats.tab.happiness.desc"] = ("Lihat dari mana poin kebahagiaan akhir berasal dan bagian mana yang menambah atau mengurangi poin kebahagiaan pemain.", "See where the final score came from and what added to or reduced the player's happiness points.");
        // Memperbarui `terms[”players.stats.tab.gameplay_data.desc”]` menggunakan tuple yang membawa bagian 1: ”Pilih kelompok untuk melihat catatan asli
        // permainan, seperti koin, kartu, aksi,...; bagian 2: ”Choose a group to view original game records such as coins, cards, actions, ris... dalam
        // AddPlayers.
        terms["players.stats.tab.gameplay_data.desc"] = ("Pilih kelompok untuk melihat catatan asli permainan, seperti koin, kartu, aksi, risiko, dan perkembangan pemain.", "Choose a group to view original game records such as coins, cards, actions, risks, and player progress.");
        // Memperbarui `terms[”players.stats.tab.analysis_results.desc”]` menggunakan tuple yang membawa bagian 1: ”Pilih kelompok untuk melihat rasio dan
        // hasil perhitungan yang digunakan untuk m...; bagian 2: ”Choose a group to view ratios and calculated results used to understand player ... dalam
        // AddPlayers.
        terms["players.stats.tab.analysis_results.desc"] = ("Pilih kelompok untuk melihat rasio dan hasil perhitungan yang digunakan untuk membaca pola pemain.", "Choose a group to view ratios and calculated results used to understand player patterns.");

        // Raw variables translations
        // Memperbarui `terms[”players.raw.coins_spent_per_turn”]` menggunakan tuple yang membawa bagian 1: ”Koin Keluar per Kejadian”; bagian 2: ”Outgoing
        // Coins per Event” dalam AddPlayers.
        terms["players.raw.coins_spent_per_turn"] = ("Koin Keluar per Kejadian", "Outgoing Coins per Event");
        // Memperbarui `terms[”players.raw.coins_earned_per_turn”]` menggunakan tuple yang membawa bagian 1: ”Koin Masuk per Kejadian”; bagian 2: ”Incoming
        // Coins per Event” dalam AddPlayers.
        terms["players.raw.coins_earned_per_turn"] = ("Koin Masuk per Kejadian", "Incoming Coins per Event");
        // Memperbarui `terms[”players.raw.transaction_history”]` menggunakan tuple yang membawa bagian 1: ”Riwayat Transaksi”; bagian 2: ”Transaction
        // History” dalam AddPlayers.
        terms["players.raw.transaction_history"] = ("Riwayat Transaksi", "Transaction History");
        // Memperbarui `terms[”players.raw.coins_in_event”]` menggunakan tuple yang membawa bagian 1: ”Koin Masuk”; bagian 2: ”Coins In” dalam AddPlayers.
        terms["players.raw.coins_in_event"] = ("Koin Masuk", "Coins In");
        // Memperbarui `terms[”players.raw.coins_out_event”]` menggunakan tuple yang membawa bagian 1: ”Koin Keluar”; bagian 2: ”Coins Out” dalam
        // AddPlayers.
        terms["players.raw.coins_out_event"] = ("Koin Keluar", "Coins Out");
        // Memperbarui `terms[”players.raw.coin_change”]` menggunakan tuple yang membawa bagian 1: ”Perubahan Koin”; bagian 2: ”Coin Change” dalam
        // AddPlayers.
        terms["players.raw.coin_change"] = ("Perubahan Koin", "Coin Change");
        // Memperbarui `terms[”players.raw.coin_balance_after_event”]` menggunakan tuple yang membawa bagian 1: ”Saldo setelah Kejadian”; bagian 2: ”Balance
        // after Event” dalam AddPlayers.
        terms["players.raw.coin_balance_after_event"] = ("Saldo setelah Kejadian", "Balance after Event");
        // Memperbarui `terms[”players.raw.amount”]` menggunakan tuple yang membawa bagian 1: ”Jumlah”; bagian 2: ”Amount” dalam AddPlayers.
        terms["players.raw.amount"] = ("Jumlah", "Amount");
        // Memperbarui `terms[”players.raw.action_slot”]` menggunakan tuple yang membawa bagian 1: ”Konteks Aksi”; bagian 2: ”Action Context” dalam
        // AddPlayers.
        terms["players.raw.action_slot"] = ("Konteks Aksi", "Action Context");
        // Memperbarui `terms[”players.raw.sequence_number”]` menggunakan tuple yang membawa bagian 1: ”Urutan Kejadian”; bagian 2: ”Event Sequence” dalam
        // AddPlayers.
        terms["players.raw.sequence_number"] = ("Urutan Kejadian", "Event Sequence");
        // Memperbarui `terms[”players.raw.item_index”]` menggunakan tuple yang membawa bagian 1: ”No.”; bagian 2: ”No.” dalam AddPlayers.
        terms["players.raw.item_index"] = ("No.", "No.");
        // Memperbarui `terms[”players.raw.cashflow_category”]` menggunakan tuple yang membawa bagian 1: ”Sumber Perubahan Koin”; bagian 2: ”Coin Change
        // Source” dalam AddPlayers.
        terms["players.raw.cashflow_category"] = ("Sumber Perubahan Koin", "Coin Change Source");
        // Memperbarui `terms[”players.details.action_slot.free”]` menggunakan tuple yang membawa bagian 1: ”Di luar jatah aksi”; bagian 2: ”Outside the
        // action allowance” dalam AddPlayers.
        terms["players.details.action_slot.free"] = ("Di luar jatah aksi", "Outside the action allowance");
        terms["players.details.action_slot.setup"] = ("Pembagian awal", "Initial setup");
        terms["players.details.transaction.category.initial_ingredient"] = ("Pembayaran Bahan Awal", "Starting Ingredient Payment");
        // Memperbarui `terms[”players.details.action_slot.first”]` menggunakan tuple yang membawa bagian 1: ”Aksi pertama”; bagian 2: ”First action” dalam
        // AddPlayers.
        terms["players.details.action_slot.first"] = ("Aksi pertama", "First action");
        // Memperbarui `terms[”players.details.action_slot.second”]` menggunakan tuple yang membawa bagian 1: ”Aksi kedua”; bagian 2: ”Second action” dalam
        // AddPlayers.
        terms["players.details.action_slot.second"] = ("Aksi kedua", "Second action");
        // Memperbarui `terms[”players.details.action_slot.numbered”]` menggunakan tuple yang membawa bagian 1: ”Aksi {0}”; bagian 2: ”Action {0}” dalam
        // AddPlayers.
        terms["players.details.action_slot.numbered"] = ("Aksi {0}", "Action {0}");
        // Memperbarui `terms[”players.raw.action_index”]` menggunakan tuple yang membawa bagian 1: ”Indeks Aksi”; bagian 2: ”Action Index” dalam
        // AddPlayers.
        terms["players.raw.action_index"] = ("Indeks Aksi", "Action Index");
        // Memperbarui `terms[”players.raw.action_type”]` menggunakan tuple yang membawa bagian 1: ”Jenis Aksi”; bagian 2: ”Action Type” dalam AddPlayers.
        terms["players.raw.action_type"] = ("Jenis Aksi", "Action Type");
        // Memperbarui `terms[”players.raw.day_index”]` menggunakan tuple yang membawa bagian 1: ”Hari”; bagian 2: ”Day” dalam AddPlayers.
        terms["players.raw.day_index"] = ("Hari", "Day");
        // Memperbarui `terms[”players.details.day.setup”]` menggunakan tuple yang membawa bagian 1: ”Persiapan”; bagian 2: ”Setup” dalam AddPlayers.
        terms["players.details.day.setup"] = ("Persiapan", "Setup");
        // Memperbarui `terms[”players.raw.rank”]` menggunakan tuple yang membawa bagian 1: ”Peringkat”; bagian 2: ”Rank” dalam AddPlayers.
        terms["players.raw.rank"] = ("Peringkat", "Rank");
        // Memperbarui `terms[”players.raw.net”]` menggunakan tuple yang membawa bagian 1: ”Perubahan Koin”; bagian 2: ”Coin Change” dalam AddPlayers.
        terms["players.raw.net"] = ("Perubahan Koin", "Coin Change");
        // Memperbarui `terms[”players.raw.coins”]` menggunakan tuple yang membawa bagian 1: ”Koin”; bagian 2: ”Coins” dalam AddPlayers.
        terms["players.raw.coins"] = ("Koin", "Coins");
        // Memperbarui `terms[”players.raw.total_actions”]` menggunakan tuple yang membawa bagian 1: ”Total Aksi”; bagian 2: ”Total Actions” dalam
        // AddPlayers.
        terms["players.raw.total_actions"] = ("Aksi Digunakan", "Actions Used");
        // Memperbarui `terms[”players.raw.distinct_actions”]` menggunakan tuple yang membawa bagian 1: ”Aksi Unik”; bagian 2: ”Distinct Actions” dalam
        // AddPlayers.
        terms["players.raw.distinct_actions"] = ("Jenis Aksi Berbeda", "Different Action Types");
        terms["players.raw.action_pattern"] = ("Pola Aksi", "Action Pattern");
        terms["players.action_pattern.none"] = ("Belum memakai aksi", "No actions used");
        terms["players.action_pattern.single"] = ("Baru satu aksi", "Only one action used");
        terms["players.action_pattern.repeated"] = ("Mengulang jenis yang sama", "Repeats an action type");
        terms["players.action_pattern.different"] = ("Semua aksi berbeda", "All actions are different");
        // Memperbarui `terms[”players.raw.repeated_actions”]` menggunakan tuple yang membawa bagian 1: ”Aksi Berulang”; bagian 2: ”Repeated Actions” dalam
        // AddPlayers.
        terms["players.raw.repeated_actions"] = ("Aksi Berulang", "Repeated Actions");
        // Memperbarui `terms[”players.raw.diversity_score”]` menggunakan tuple yang membawa bagian 1: ”Skor Keberagaman”; bagian 2: ”Diversity Score” dalam
        // AddPlayers.
        terms["players.raw.diversity_score"] = ("Skor Keberagaman", "Diversity Score");
        // Memperbarui `terms[”players.raw.ingredient_types_held”]` menggunakan tuple yang membawa bagian 1: ”Rincian Bahan Tersisa per Jenis”; bagian 2:
        // ”Remaining Ingredients by Type” dalam AddPlayers.
        terms["players.raw.ingredient_types_held"] = ("Rincian Bahan Tersisa per Jenis", "Remaining Ingredients by Type");
        // Memperbarui `terms[”players.raw.ingredients_used_per_meal”]` menggunakan tuple yang membawa bagian 1: ”Bahan Digunakan per Pesanan”; bagian 2:
        // ”Ingredients Used per Order” dalam AddPlayers.
        terms["players.raw.ingredients_used_per_meal"] = ("Bahan Digunakan per Pesanan", "Ingredients Used per Order");
        // Memperbarui `terms[”players.raw.ingredients_used_total”]` menggunakan tuple yang membawa bagian 1: ”Total Bahan Digunakan”; bagian 2: ”Total
        // Ingredients Used” dalam AddPlayers.
        terms["players.raw.ingredients_used_total"] = ("Total Bahan Digunakan", "Total Ingredients Used");
        // Memperbarui `terms[”players.raw.ingredients_used_per_meal_average”]` menggunakan tuple yang membawa bagian 1: ”Rata-rata Bahan per Pesanan”;
        // bagian 2: ”Average Ingredients per Order” dalam AddPlayers.
        terms["players.raw.ingredients_used_per_meal_average"] = ("Rata-rata Bahan per Pesanan", "Average Ingredients per Order");
        // Memperbarui `terms[”players.raw.ingredient_investment_coins_total”]` menggunakan tuple yang membawa bagian 1: ”Total Biaya Pembelian Bahan”;
        // bagian 2: ”Total Ingredient Purchase Cost” dalam AddPlayers.
        terms["players.raw.ingredient_investment_coins_total"] = ("Total Biaya Pembelian Bahan", "Total Ingredient Purchase Cost");
        // Memperbarui `terms[”players.raw.meal_order_income_per_order”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan per Pesanan Makanan”; bagian
        // 2: ”Meal Order Income per Order” dalam AddPlayers.
        terms["players.raw.meal_order_income_per_order"] = ("Pendapatan per Pesanan Makanan", "Meal Order Income per Order");
        // Memperbarui `terms[”players.raw.meal_orders_per_turn_average”]` menggunakan tuple yang membawa bagian 1: ”Rata-rata Pesanan per Hari dengan Aksi
        // Utama”; bagian 2: ”Average Orders per Main-Action Day” dalam AddPlayers.
        terms["players.raw.meal_orders_per_turn_average"] = ("Rata-rata Pesanan per Hari Pemain Beraksi", "Average Orders per Day with Player Actions");
        // Memperbarui `terms[”players.raw.primary_needs_owned”]` menggunakan tuple yang membawa bagian 1: ”Kartu Kebutuhan Primer yang Dimiliki”; bagian 2:
        // ”Primary Need Cards Owned” dalam AddPlayers.
        terms["players.raw.primary_needs_owned"] = ("Kartu Kebutuhan Primer yang Dimiliki", "Primary Need Cards Owned");
        // Memperbarui `terms[”players.raw.secondary_needs_owned”]` menggunakan tuple yang membawa bagian 1: ”Kartu Kebutuhan Sekunder yang Dimiliki”;
        // bagian 2: ”Secondary Need Cards Owned” dalam AddPlayers.
        terms["players.raw.secondary_needs_owned"] = ("Kartu Kebutuhan Sekunder yang Dimiliki", "Secondary Need Cards Owned");
        // Memperbarui `terms[”players.raw.tertiary_needs_owned”]` menggunakan tuple yang membawa bagian 1: ”Kartu Kebutuhan Tersier yang Dimiliki”; bagian
        // 2: ”Tertiary Need Cards Owned” dalam AddPlayers.
        terms["players.raw.tertiary_needs_owned"] = ("Kartu Kebutuhan Tersier yang Dimiliki", "Tertiary Need Cards Owned");
        // Memperbarui `terms[”players.raw.need_cards_owned_current”]` menggunakan tuple yang membawa bagian 1: ”Kartu Kebutuhan yang Masih Dimiliki”;
        // bagian 2: ”Need Cards Currently Owned” dalam AddPlayers.
        terms["players.raw.need_cards_owned_current"] = ("Kartu Kebutuhan yang Masih Dimiliki", "Need Cards Currently Owned");
        // Memperbarui `terms[”players.raw.need_profile”]` menggunakan tuple yang membawa bagian 1: ”Pemeriksaan Komposisi Kebutuhan”; bagian 2: ”Need
        // Composition Checks” dalam AddPlayers.
        terms["players.raw.need_profile"] = ("Pemeriksaan Komposisi Kebutuhan", "Need Composition Checks");
        // Memperbarui `terms[”players.raw.basic_profile”]` menggunakan tuple yang membawa bagian 1: ”Memiliki Primer, Sekunder, dan Tersier”; bagian 2:
        // ”Has Primary, Secondary, and Tertiary” dalam AddPlayers.
        terms["players.raw.basic_profile"] = ("Memiliki Primer, Sekunder, dan Tersier", "Has Primary, Secondary, and Tertiary");
        // Memperbarui `terms[”players.raw.collector_profile”]` menggunakan tuple yang membawa bagian 1: ”Memiliki Minimal 4 Jenis Kebutuhan”; bagian 2:
        // ”Has at Least 4 Need Types” dalam AddPlayers.
        terms["players.raw.collector_profile"] = ("Memiliki Minimal 4 Jenis Kebutuhan", "Has at Least 4 Need Types");
        // Memperbarui `terms[”players.raw.specialist_profile”]` menggunakan tuple yang membawa bagian 1: ”Minimal 70% Berasal dari Satu Kategori”; bagian
        // 2: ”At Least 70% from One Category” dalam AddPlayers.
        terms["players.raw.specialist_profile"] = ("Minimal 70% Berasal dari Satu Kategori", "At Least 70% from One Category");
        // Memperbarui `terms[”players.raw.specific_tertiary_need”]` menggunakan tuple yang membawa bagian 1: ”Kebutuhan Tersier Target Misi Dimiliki”;
        // bagian 2: ”Mission Target Tertiary Need Owned” dalam AddPlayers.
        terms["players.raw.specific_tertiary_need"] = ("Kebutuhan Tersier Target Misi Pernah Dibeli", "Mission Target Tertiary Need Purchased");
        // Memperbarui `terms[”players.raw.need_cards_coins_spent”]` menggunakan tuple yang membawa bagian 1: ”Total Biaya Kartu Kebutuhan”; bagian 2:
        // ”Total Need Card Cost” dalam AddPlayers.
        terms["players.raw.need_cards_coins_spent"] = ("Total Biaya Kartu Kebutuhan", "Total Need Card Cost");
        // Memperbarui `terms[”players.raw.donation_amount_per_friday”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Donasi Setiap Jumat”; bagian 2:
        // ”Donation Amount Each Friday” dalam AddPlayers.
        terms["players.raw.donation_amount_per_friday"] = ("Jumlah Donasi Setiap Jumat", "Donation Amount Each Friday");
        // Memperbarui `terms[”players.raw.donation_history”]` menggunakan tuple yang membawa bagian 1: ”Donasi Setiap Jumat”; bagian 2: ”Donations Each
        // Friday” dalam AddPlayers.
        terms["players.raw.donation_history"] = ("Donasi Setiap Jumat", "Donations Each Friday");
        // Memperbarui `terms[”players.details.donations.amount”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Donasi”; bagian 2: ”Donation Amount”
        // dalam AddPlayers.
        terms["players.details.donations.amount"] = ("Jumlah Donasi", "Donation Amount");
        // Memperbarui `terms[”players.raw.donation_rank_per_friday”]` menggunakan tuple yang membawa bagian 1: ”Peringkat Donasi per Jumat”; bagian 2:
        // ”Donation Rank by Friday” dalam AddPlayers.
        terms["players.raw.donation_rank_per_friday"] = ("Peringkat Donasi per Jumat", "Donation Rank by Friday");
        // Memperbarui `terms[”players.raw.donation_total_coins”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Donasi”; bagian 2: ”Total Donated
        // Coins” dalam AddPlayers.
        terms["players.raw.donation_total_coins"] = ("Total Koin Donasi", "Total Donated Coins");
        // Memperbarui `terms[”players.raw.donation_champion_cards_earned”]` menggunakan tuple yang membawa bagian 1: ”Kartu Juara Donasi Diperoleh”; bagian
        // 2: ”Donation Champion Cards Earned” dalam AddPlayers.
        terms["players.raw.donation_champion_cards_earned"] = ("Kartu Juara Donasi Diperoleh", "Donation Champion Cards Earned");
        // Memperbarui `terms[”players.raw.donation_happiness_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan dari Donasi”; bagian 2:
        // ”Happiness Points from Donations” dalam AddPlayers.
        terms["players.raw.donation_happiness_points"] = ("Poin Kebahagiaan dari Donasi", "Happiness Points from Donations");
        // Memperbarui `terms[”players.raw.donation_events”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Hari Berdonasi”; bagian 2: ”Donation Days”
        // dalam AddPlayers.
        terms["players.raw.donation_events"] = ("Jumlah Hari Berdonasi", "Donation Days");
        // Memperbarui `terms[”players.raw.gold_cards_initial”]` menggunakan tuple yang membawa bagian 1: ”Kartu Emas Awal”; bagian 2: ”Starting Gold Cards”
        // dalam AddPlayers.
        terms["players.raw.gold_cards_initial"] = ("Kartu Emas Awal", "Starting Gold Cards");
        // Memperbarui `terms[”players.raw.gold_cards_purchased”]` menggunakan tuple yang membawa bagian 1: ”Kartu Emas Dibeli selama Permainan”; bagian 2:
        // ”Gold Cards Purchased during Gameplay” dalam AddPlayers.
        terms["players.raw.gold_cards_purchased"] = ("Kartu Emas Dibeli selama Permainan", "Gold Cards Purchased during Gameplay");
        // Memperbarui `terms[”players.raw.gold_cards_sold”]` menggunakan tuple yang membawa bagian 1: ”Kartu Emas Dijual”; bagian 2: ”Gold Cards Sold”
        // dalam AddPlayers.
        terms["players.raw.gold_cards_sold"] = ("Kartu Emas Dijual", "Gold Cards Sold");
        // Memperbarui `terms[”players.raw.gold_prices_per_purchase”]` menggunakan tuple yang membawa bagian 1: ”Harga Emas Saat Beli”; bagian 2: ”Gold
        // Purchase Prices” dalam AddPlayers.
        terms["players.raw.gold_prices_per_purchase"] = ("Harga Beli per Kartu Emas", "Purchase Price per Gold Card");
        // Memperbarui `terms[”players.raw.gold_price_per_sale”]` menggunakan tuple yang membawa bagian 1: ”Harga Emas Saat Jual”; bagian 2: ”Gold Sale
        // Prices” dalam AddPlayers.
        terms["players.raw.gold_price_per_sale"] = ("Harga Jual per Kartu Emas", "Sale Price per Gold Card");
        // Memperbarui `terms[”players.raw.gold_investment_coins_spent”]` menggunakan tuple yang membawa bagian 1: ”Total Biaya Pembelian Emas”; bagian 2:
        // ”Total Gold Purchase Cost” dalam AddPlayers.
        terms["players.raw.gold_investment_coins_spent"] = ("Total Biaya Pembelian Emas", "Total Gold Purchase Cost");
        // Memperbarui `terms[”players.raw.gold_investment_coins_earned”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan Penjualan Emas”; bagian 2:
        // ”Gold Sale Income” dalam AddPlayers.
        terms["players.raw.gold_investment_coins_earned"] = ("Pendapatan Penjualan Emas", "Gold Sale Income");
        // Memperbarui `terms[”players.raw.gold_investment_net”]` menggunakan tuple yang membawa bagian 1: ”Selisih Arus Kas Transaksi Emas”; bagian 2:
        // ”Gold Transaction Cashflow Difference” dalam AddPlayers.
        terms["players.raw.gold_investment_net"] = ("Selisih Arus Kas Transaksi Emas", "Gold Transaction Cashflow Difference");
        // Memperbarui `terms[”players.raw.leftover_coins_end_game”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa”; bagian 2: ”Remaining Coins”
        // dalam AddPlayers.
        terms["players.raw.leftover_coins_end_game"] = ("Koin Tersisa", "Remaining Coins");
        // Memperbarui `terms[”players.raw.ingredient_cards_value_end”]` menggunakan tuple yang membawa bagian 1: ”Nilai Kartu Bahan Tersisa”; bagian 2:
        // ”Remaining Ingredient Card Value” dalam AddPlayers.
        terms["players.raw.ingredient_cards_value_end"] = ("Nilai Kartu Bahan Tersisa", "Remaining Ingredient Card Value");
        // Memperbarui `terms[”players.raw.coins_in_savings_goal”]` menggunakan tuple yang membawa bagian 1: ”Koin dalam Tabungan”; bagian 2: ”Coins in
        // Savings” dalam AddPlayers.
        terms["players.raw.coins_in_savings_goal"] = ("Koin dalam Tabungan", "Coins in Savings");
        // Memperbarui `terms[”players.raw.pension_fund_total”]` menggunakan tuple yang membawa bagian 1: ”Total Dana Pensiun”; bagian 2: ”Total Pension
        // Fund” dalam AddPlayers.
        terms["players.raw.pension_fund_total"] = ("Total Dana Pensiun", "Total Pension Fund");
        // Memperbarui `terms[”players.raw.pension_fund_rank_per_game”]` menggunakan tuple yang membawa bagian 1: ”Peringkat Dana Pensiun”; bagian 2:
        // ”Pension Fund Rank” dalam AddPlayers.
        terms["players.raw.pension_fund_rank_per_game"] = ("Peringkat Dana Pensiun", "Pension Fund Rank");
        // Memperbarui `terms[”players.raw.pension_fund_happiness_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Dana Pensiun”; bagian
        // 2: ”Pension Happiness Points” dalam AddPlayers.
        terms["players.raw.pension_fund_happiness_points"] = ("Poin Kebahagiaan Dana Pensiun", "Pension Happiness Points");
        // Memperbarui `terms[”players.raw.life_risk_cards_drawn”]` menggunakan tuple yang membawa bagian 1: ”Kartu Risiko Kehidupan yang Muncul”; bagian 2:
        // ”Life Risk Cards Drawn” dalam AddPlayers.
        terms["players.raw.life_risk_cards_drawn"] = ("Kartu Risiko Kehidupan yang Muncul", "Life Risk Cards Drawn");
        // Memperbarui `terms[”players.raw.life_risk_costs_per_card”]` menggunakan tuple yang membawa bagian 1: ”Nominal Dampak Koin per Kartu Risiko”;
        // bagian 2: ”Nominal Coin Impact per Risk Card” dalam AddPlayers.
        terms["players.raw.life_risk_costs_per_card"] = ("Nominal Dampak Koin per Kartu Risiko", "Nominal Coin Impact per Risk Card");
        // Memperbarui `terms[”players.raw.life_risk_costs_total”]` menggunakan tuple yang membawa bagian 1: ”Total Nominal Dampak Koin Kartu Risiko”;
        // bagian 2: ”Total Nominal Risk-Card Coin Impact” dalam AddPlayers.
        terms["players.raw.life_risk_costs_total"] = ("Total Nominal Dampak Koin Kartu Risiko", "Total Nominal Risk-Card Coin Impact");
        // Memperbarui `terms[”players.raw.life_risk_mitigated_with_insurance”]` menggunakan tuple yang membawa bagian 1: ”Risiko yang Ditanggung Asuransi”;
        // bagian 2: ”Risks Covered by Insurance” dalam AddPlayers.
        terms["players.raw.life_risk_mitigated_with_insurance"] = ("Risiko yang Ditanggung Asuransi", "Risks Covered by Insurance");
        // Memperbarui `terms[”players.raw.insurance_payments_made”]` menggunakan tuple yang membawa bagian 1: ”Total Premi Asuransi Dibayar”; bagian 2:
        // ”Total Insurance Premiums Paid” dalam AddPlayers.
        terms["players.raw.insurance_payments_made"] = ("Total Premi Asuransi Dibayar", "Total Insurance Premiums Paid");
        // Memperbarui `terms[”players.raw.emergency_options_used”]` menggunakan tuple yang membawa bagian 1: ”Penggunaan Tindakan Darurat”; bagian 2:
        // ”Emergency Actions Used” dalam AddPlayers.
        terms["players.raw.emergency_options_used"] = ("Penggunaan Tindakan Darurat", "Emergency Actions Used");
        // Memperbarui `terms[”players.raw.financial_goals_attempted”]` menggunakan tuple yang membawa bagian 1: ”Target Finansial yang Dicoba”; bagian 2:
        // ”Financial Goals Attempted” dalam AddPlayers.
        terms["players.raw.financial_goals_attempted"] = ("Target Finansial yang Dicoba", "Financial Goals Attempted");
        // Memperbarui `terms[”players.raw.financial_goals_completed”]` menggunakan tuple yang membawa bagian 1: ”Target Finansial yang Selesai”; bagian 2:
        // ”Financial Goals Completed” dalam AddPlayers.
        terms["players.raw.financial_goals_completed"] = ("Target Finansial Berhasil Dibeli", "Financial Goals Purchased");
        terms["players.raw.financial_goals_purchase_cost_total"] = ("Total Biaya Pembelian Target Finansial", "Total Financial Goal Purchase Cost");
        // Memperbarui `terms[”players.raw.financial_goals_coins_per_goal”]` menggunakan tuple yang membawa bagian 1: ”Biaya per Target Finansial”; bagian
        // 2: ”Cost per Financial Goal” dalam AddPlayers.
        terms["players.raw.financial_goals_coins_per_goal"] = ("Biaya per Target Finansial", "Cost per Financial Goal");
        // Memperbarui `terms[”players.raw.financial_goals_balance_per_goal”]` menggunakan tuple yang membawa bagian 1: ”Saldo per Target Finansial”; bagian
        // 2: ”Balance per Financial Goal” dalam AddPlayers.
        terms["players.raw.financial_goals_balance_per_goal"] = ("Saldo per Target Finansial", "Balance per Financial Goal");
        // Memperbarui `terms[”players.raw.financial_goals_coins_total_invested”]` menggunakan tuple yang membawa bagian 1: ”Dana yang Diperhitungkan untuk
        // Target”; bagian 2: ”Funds Counted Toward Goals” dalam AddPlayers.
        terms["players.raw.financial_goals_coins_total_invested"] = ("Dana yang Diperhitungkan untuk Target", "Funds Counted Toward Goals");
        // Memperbarui `terms[”players.raw.financial_goals_incomplete_coins_wasted”]` menggunakan tuple yang membawa bagian 1: ”Koin pada Target Finansial
        // Belum Selesai”; bagian 2: ”Coins in Unfinished Financial Goals” dalam AddPlayers.
        terms["players.raw.financial_goals_incomplete_coins_wasted"] = ("Tabungan untuk Target Belum Dibeli", "Savings for Unpurchased Goals");
        // Memperbarui `terms[”players.raw.sharia_loans_taken”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Pinjaman Diambil”; bagian 2: ”Loans Taken
        // Count” dalam AddPlayers.
        terms["players.raw.sharia_loans_taken"] = ("Jumlah Pinjaman Diambil", "Loans Taken Count");
        // Memperbarui `terms[”players.raw.sharia_loans_repaid”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Pinjaman Dilunasi”; bagian 2: ”Loans
        // Repaid Count” dalam AddPlayers.
        terms["players.raw.sharia_loans_repaid"] = ("Jumlah Pinjaman Dilunasi", "Loans Repaid Count");
        // Memperbarui `terms[”players.raw.sharia_loans_unpaid_end”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Pinjaman Belum Lunas”; bagian 2:
        // ”Unpaid Loan Count” dalam AddPlayers.
        terms["players.raw.sharia_loans_unpaid_end"] = ("Jumlah Pinjaman Belum Lunas", "Unpaid Loan Count");
        // Memperbarui `terms[”players.raw.sharia_loans_outstanding_coins”]` menggunakan tuple yang membawa bagian 1: ”Sisa Pinjaman”; bagian 2:
        // ”Outstanding Loan” dalam AddPlayers.
        terms["players.raw.sharia_loans_outstanding_coins"] = ("Sisa Pinjaman", "Outstanding Loan");
        // Memperbarui `terms[”players.raw.loan_penalty_if_unpaid”]` menggunakan tuple yang membawa bagian 1: ”Pengurangan Poin Kebahagiaan Pinjaman”;
        // bagian 2: ”Loan Happiness Point Deduction” dalam AddPlayers.
        terms["players.raw.loan_penalty_if_unpaid"] = ("Pengurangan Poin Kebahagiaan Pinjaman", "Loan Happiness Point Deduction");
        // Memperbarui `terms[”players.raw.actions_per_turn”]` menggunakan tuple yang membawa bagian 1: ”Jatah Aksi per Hari”; bagian 2: ”Actions Available
        // per Day” dalam AddPlayers.
        terms["players.raw.actions_per_turn"] = ("Jatah Aksi per Hari", "Actions Available per Day");
        // Memperbarui `terms[”players.raw.action_repetitions_per_turn”]` menggunakan tuple yang membawa bagian 1: ”Pola Aksi per Hari”; bagian 2: ”Daily
        // Action Pattern” dalam AddPlayers.
        terms["players.raw.action_repetitions_per_turn"] = ("Pola Aksi per Hari", "Daily Action Pattern");
        // Memperbarui `terms[”players.raw.action_sequence”]` menggunakan tuple yang membawa bagian 1: ”Urutan Aksi”; bagian 2: ”Action Sequence” dalam
        // AddPlayers.
        terms["players.raw.action_sequence"] = ("Urutan Aksi", "Action Sequence");
        // Memperbarui `terms[”players.raw.action_usage_history”]` menggunakan tuple yang membawa bagian 1: ”Riwayat Penggunaan Aksi per Hari”; bagian 2:
        // ”Daily Action Use History” dalam AddPlayers.
        terms["players.raw.action_usage_history"] = ("Riwayat Penggunaan Aksi per Hari", "Daily Action Use History");
        // Memperbarui `terms[”players.raw.coins_per_turn_progression”]` menggunakan tuple yang membawa bagian 1: ”Saldo Koin setelah Setiap Kejadian”;
        // bagian 2: ”Coin Balance after Each Event” dalam AddPlayers.
        terms["players.raw.coins_per_turn_progression"] = ("Saldo Koin setelah Setiap Kejadian", "Coin Balance after Each Event");
        // Memperbarui `terms[”players.raw.net_income_per_turn”]` menggunakan tuple yang membawa bagian 1: ”Perubahan Koin per Kejadian”; bagian 2: ”Coin
        // Change per Event” dalam AddPlayers.
        terms["players.raw.net_income_per_turn"] = ("Perubahan Koin per Kejadian", "Coin Change per Event");
        // Memperbarui `terms[”players.support.meaning.coin_change_event”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan perubahan koin pada
        // satu kejadian. Nilai positif menambah koin,...; bagian 2: ”{0} shows the coin change from one event. A positive value adds coins, while a ...
        // dalam AddPlayers.
        terms["players.support.meaning.coin_change_event"] = ("{0} menunjukkan perubahan koin pada satu kejadian. Nilai positif menambah koin, sedangkan nilai negatif mengurangi koin.", "{0} shows the coin change from one event. A positive value adds coins, while a negative value subtracts coins.");
        // Memperbarui `terms[”players.support.meaning.transaction_history”]` menggunakan tuple yang membawa bagian 1: ”{0} memperlihatkan koin masuk, koin
        // keluar, perubahan bersih, dan saldo setelah...; bagian 2: ”{0} shows coins in, coins out, net change, and the balance after every event in...
        // dalam AddPlayers.
        terms["players.support.meaning.transaction_history"] = ("{0} memperlihatkan koin masuk, koin keluar, perubahan bersih, dan saldo setelah setiap kejadian. Pembayaran Bahan Awal adalah biaya kartu bahan yang dibagikan saat persiapan, sebelum jatah aksi harian dimulai.", "{0} shows coins in, coins out, net change, and the balance after every event. Starting Ingredient Payment is the cost of the ingredient card dealt during setup, before the daily action allowance begins.");
        // Memperbarui `terms[”players.raw.action_slot_when_debt_introduced”]` menggunakan tuple yang membawa bagian 1: ”Urutan Aksi saat Pinjaman Pertama”;
        // bagian 2: ”Action Slot of First Loan” dalam AddPlayers.
        terms["players.raw.action_slot_when_debt_introduced"] = ("Urutan Aksi saat Pinjaman Pertama", "Action Slot of First Loan");
        // Memperbarui `terms[”players.raw.action_slot_when_first_risk_hit”]` menggunakan tuple yang membawa bagian 1: ”Urutan Aksi saat Risiko Pertama”;
        // bagian 2: ”Action Slot of First Risk” dalam AddPlayers.
        terms["players.raw.action_slot_when_first_risk_hit"] = ("Urutan Aksi saat Risiko Pertama", "Action Slot of First Risk");
        // Memperbarui `terms[”players.raw.action_slot_game_completion”]` menggunakan tuple yang membawa bagian 1: ”Urutan Aksi Terakhir”; bagian 2: ”Final
        // Action Slot” dalam AddPlayers.
        terms["players.raw.action_slot_game_completion"] = ("Urutan Aksi Terakhir", "Final Action Slot");
        // Memperbarui `terms[”players.raw.day_when_debt_introduced”]` menggunakan tuple yang membawa bagian 1: ”Pinjaman Pertama Tercatat”; bagian 2:
        // ”First Recorded Loan” dalam AddPlayers.
        terms["players.raw.day_when_debt_introduced"] = ("Pinjaman Pertama Tercatat", "First Recorded Loan");
        // Memperbarui `terms[”players.raw.day_when_first_risk_hit”]` menggunakan tuple yang membawa bagian 1: ”Risiko Pertama Tercatat”; bagian 2: ”First
        // Recorded Risk” dalam AddPlayers.
        terms["players.raw.day_when_first_risk_hit"] = ("Risiko Pertama Tercatat", "First Recorded Risk");
        // Memperbarui `terms[”players.raw.day_game_completion”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas Terakhir Tercatat”; bagian 2: ”Latest
        // Recorded Activity” dalam AddPlayers.
        terms["players.raw.day_game_completion"] = ("Aktivitas Terakhir Tercatat", "Latest Recorded Activity");
        // Memperbarui `terms[”players.raw.turn_number_game_completion”]` menggunakan tuple yang membawa bagian 1: ”Giliran Penyelesaian Permainan”; bagian
        // 2: ”Game Completion Turn” dalam AddPlayers.
        terms["players.raw.turn_number_game_completion"] = ("Giliran Penyelesaian Permainan", "Game Completion Turn");
        // Memperbarui `terms[”players.raw.event_count”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Kejadian Tercatat”; bagian 2: ”Recorded Event
        // Count” dalam AddPlayers.
        terms["players.raw.event_count"] = ("Jumlah Kejadian Tercatat", "Recorded Event Count");
        // Memperbarui `terms[”players.raw.latest_day_index”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas Terakhir Tercatat”; bagian 2: ”Latest
        // Recorded Activity” dalam AddPlayers.
        terms["players.raw.latest_day_index"] = ("Aktivitas Terakhir Tercatat", "Latest Recorded Activity");
        // Memperbarui `terms[”players.support.meaning.first_loan_day”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan kapan pinjaman pertama
        // tercatat. Pada mode Mahir, pinjaman awal...; bagian 2: ”{0} shows when the first loan was recorded. In Advanced mode, the setup loan is... dalam
        // AddPlayers.
        terms["players.support.meaning.first_loan_day"] = ("{0} menunjukkan kapan pinjaman pertama tercatat. Pada mode Mahir, pinjaman awal diberikan saat persiapan dan bukan aksi yang dipilih pemain saat permainan.", "{0} shows when the first loan was recorded. In Advanced mode, the setup loan is granted during setup rather than chosen as a gameplay action.");
        // Memperbarui `terms[”players.support.meaning.first_risk_day”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan hari pertama kartu Risiko
        // Kehidupan dicatat untuk pemain.”; bagian 2: ”{0} shows the first day a Life Risk card was recorded for the player.” dalam AddPlayers.
        terms["players.support.meaning.first_risk_day"] = ("{0} menunjukkan hari pertama kartu Risiko Kehidupan dicatat untuk pemain.", "{0} shows the first day a Life Risk card was recorded for the player.");
        // Memperbarui `terms[”players.support.meaning.last_activity_day”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan hari dari aktivitas
        // terakhir pemain yang tercatat. Nilai ini bu...; bagian 2: ”{0} shows the day of the player's latest recorded activity. It is not elapsed p...
        // dalam AddPlayers.
        terms["players.support.meaning.last_activity_day"] = ("{0} menunjukkan hari dari aktivitas terakhir pemain yang tercatat. Nilai ini bukan lama bermain atau penanda bahwa sesi sudah selesai.", "{0} shows the day of the player's latest recorded activity. It is not elapsed play time or proof that the session has ended.");
        // Memperbarui `terms[”players.support.meaning.emergency_actions”]` menggunakan tuple yang membawa bagian 1: ”{0} menghitung berapa kali pemain
        // menyelesaikan biaya Risiko Kehidupan dengan m...; bagian 2: ”{0} counts how often the player covered a Life Risk cost by selling a need card...
        // dalam AddPlayers.
        terms["players.support.meaning.emergency_actions"] = ("{0} menghitung berapa kali pemain menyelesaikan biaya Risiko Kehidupan dengan menjual kartu kebutuhan, menjual emas, atau mengambil pinjaman Syariah karena koin tidak mencukupi.", "{0} counts how often the player covered a Life Risk cost by selling a need card, selling gold, or taking a Sharia loan because available coins were insufficient.");
        // Memperbarui `terms[”players.raw.transaction_count”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Transaksi”; bagian 2: ”Transaction Count”
        // dalam AddPlayers.
        terms["players.raw.transaction_count"] = ("Jumlah Transaksi", "Transaction Count");
        // Memperbarui `terms[”players.raw.seed_source”]` menggunakan tuple yang membawa bagian 1: ”Sumber Data Simulasi”; bagian 2: ”Simulation Data
        // Source” dalam AddPlayers.
        terms["players.raw.seed_source"] = ("Sumber Data Simulasi", "Simulation Data Source");
        // Memperbarui `terms[”players.raw.total_happiness_points”]` menggunakan tuple yang membawa bagian 1: ”Total Poin Kebahagiaan”; bagian 2: ”Total
        // Happiness Points” dalam AddPlayers.
        terms["players.raw.total_happiness_points"] = ("Total Poin Kebahagiaan", "Total Happiness Points");
        // Memperbarui `terms[”players.raw.final_rank”]` menggunakan tuple yang membawa bagian 1: ”Peringkat Akhir”; bagian 2: ”Final Rank” dalam
        // AddPlayers.
        terms["players.raw.final_rank"] = ("Peringkat Akhir", "Final Rank");
        // Memperbarui `terms[”players.raw.winner_flag”]` menggunakan tuple yang membawa bagian 1: ”Status Pemenang”; bagian 2: ”Winner Status” dalam
        // AddPlayers.
        terms["players.raw.winner_flag"] = ("Status Pemenang", "Winner Status");
        // Memperbarui `terms[”players.raw.finish_line_reached”]` menggunakan tuple yang membawa bagian 1: ”Mencapai Garis Finis”; bagian 2: ”Finish Line
        // Reached” dalam AddPlayers.
        terms["players.raw.finish_line_reached"] = ("Mencapai Garis Finis", "Finish Line Reached");
        // Memperbarui `terms[”players.raw.dnf_flag”]` menggunakan tuple yang membawa bagian 1: ”Status Tidak Finis (DNF)”; bagian 2: ”DNF Status” dalam
        // AddPlayers.
        terms["players.raw.dnf_flag"] = ("Status Tidak Finis (DNF)", "DNF Status");

        // Ingredients and Need Cards translations
        // Memperbarui `terms[”players.raw.sayur”]` menggunakan tuple yang membawa bagian 1: ”Sayur”; bagian 2: ”Veggie” dalam AddPlayers.
        terms["players.raw.sayur"] = ("Sayur", "Veggie");
        // Memperbarui `terms[”players.raw.telur”]` menggunakan tuple yang membawa bagian 1: ”Telur”; bagian 2: ”Egg” dalam AddPlayers.
        terms["players.raw.telur"] = ("Telur", "Egg");
        // Memperbarui `terms[”players.raw.daging”]` menggunakan tuple yang membawa bagian 1: ”Daging”; bagian 2: ”Meat” dalam AddPlayers.
        terms["players.raw.daging"] = ("Daging", "Meat");
        // Memperbarui `terms[”players.raw.tahu_tempe”]` menggunakan tuple yang membawa bagian 1: ”Tahu Tempe”; bagian 2: ”Tofu Tempeh” dalam AddPlayers.
        terms["players.raw.tahu_tempe"] = ("Tahu Tempe", "Tofu Tempeh");
        // Memperbarui `terms[”players.raw.nasi_putih”]` menggunakan tuple yang membawa bagian 1: ”Nasi Putih”; bagian 2: ”White Rice” dalam AddPlayers.
        terms["players.raw.nasi_putih"] = ("Nasi Putih", "White Rice");
        // Memperbarui `terms[”players.raw.ayam”]` menggunakan tuple yang membawa bagian 1: ”Ayam”; bagian 2: ”Chicken” dalam AddPlayers.
        terms["players.raw.ayam"] = ("Ayam", "Chicken");
        // Memperbarui `terms[”players.raw.buku”]` menggunakan tuple yang membawa bagian 1: ”Buku”; bagian 2: ”Book” dalam AddPlayers.
        terms["players.raw.buku"] = ("Buku", "Book");
        // Memperbarui `terms[”players.raw.sepatu”]` menggunakan tuple yang membawa bagian 1: ”Sepatu”; bagian 2: ”Shoes” dalam AddPlayers.
        terms["players.raw.sepatu"] = ("Sepatu", "Shoes");
        // Memperbarui `terms[”players.raw.boneka”]` menggunakan tuple yang membawa bagian 1: ”Boneka”; bagian 2: ”Doll” dalam AddPlayers.
        terms["players.raw.boneka"] = ("Boneka", "Doll");
        // Memperbarui `terms[”players.raw.gameboy”]` menggunakan tuple yang membawa bagian 1: ”Gameboy”; bagian 2: ”Gameboy” dalam AddPlayers.
        terms["players.raw.gameboy"] = ("Gameboy", "Gameboy");
        // Memperbarui `terms[”players.raw.hiburan”]` menggunakan tuple yang membawa bagian 1: ”Hiburan”; bagian 2: ”Entertainment” dalam AddPlayers.
        terms["players.raw.hiburan"] = ("Hiburan", "Entertainment");
        // Memperbarui `terms[”players.raw.veggie”]` menggunakan tuple yang membawa bagian 1: ”Sayur”; bagian 2: ”Veggie” dalam AddPlayers.
        terms["players.raw.veggie"] = ("Sayur", "Veggie");
        // Memperbarui `terms[”players.raw.egg”]` menggunakan tuple yang membawa bagian 1: ”Telur”; bagian 2: ”Egg” dalam AddPlayers.
        terms["players.raw.egg"] = ("Telur", "Egg");
        // Memperbarui `terms[”players.raw.meat”]` menggunakan tuple yang membawa bagian 1: ”Daging”; bagian 2: ”Meat” dalam AddPlayers.
        terms["players.raw.meat"] = ("Daging", "Meat");
        // Memperbarui `terms[”players.raw.tofu_tempeh”]` menggunakan tuple yang membawa bagian 1: ”Tahu Tempe”; bagian 2: ”Tofu Tempeh” dalam AddPlayers.
        terms["players.raw.tofu_tempeh"] = ("Tahu Tempe", "Tofu Tempeh");
        // Memperbarui `terms[”players.raw.white_rice”]` menggunakan tuple yang membawa bagian 1: ”Nasi Putih”; bagian 2: ”White Rice” dalam AddPlayers.
        terms["players.raw.white_rice"] = ("Nasi Putih", "White Rice");
        // Memperbarui `terms[”players.raw.chicken”]` menggunakan tuple yang membawa bagian 1: ”Ayam”; bagian 2: ”Chicken” dalam AddPlayers.
        terms["players.raw.chicken"] = ("Ayam", "Chicken");
        // Memperbarui `terms[”players.raw.book”]` menggunakan tuple yang membawa bagian 1: ”Buku”; bagian 2: ”Book” dalam AddPlayers.
        terms["players.raw.book"] = ("Buku", "Book");
        // Memperbarui `terms[”players.raw.shoes”]` menggunakan tuple yang membawa bagian 1: ”Sepatu”; bagian 2: ”Shoes” dalam AddPlayers.
        terms["players.raw.shoes"] = ("Sepatu", "Shoes");
        // Memperbarui `terms[”players.raw.doll”]` menggunakan tuple yang membawa bagian 1: ”Boneka”; bagian 2: ”Doll” dalam AddPlayers.
        terms["players.raw.doll"] = ("Boneka", "Doll");
        // Memperbarui `terms[”players.raw.entertainment”]` menggunakan tuple yang membawa bagian 1: ”Hiburan”; bagian 2: ”Entertainment” dalam AddPlayers.
        terms["players.raw.entertainment"] = ("Hiburan", "Entertainment");

        // Derived metrics translations
        // Memperbarui `terms[”players.metric.net_worth_index”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa dibanding Koin Awal”; bagian 2:
        // ”Remaining Coins Compared with Starting Coins” dalam AddPlayers.
        terms["players.metric.net_worth_index"] = ("Perubahan Koin dari Awal", "Coin Change from Start");
        // Memperbarui `terms[”players.metric.income_diversification_index”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan Sumber Pendapatan”;
        // bagian 2: ”Income Source Balance” dalam AddPlayers.
        terms["players.metric.income_diversification_index"] = ("Pemerataan Sumber Pendapatan", "Income Source Balance");
        // Memperbarui `terms[”players.metric.cash_growth_percent”]` menggunakan tuple yang membawa bagian 1: ”Koin Tersisa dibanding Koin Awal”; bagian 2:
        // ”Remaining Coins Compared with Starting Coins” dalam AddPlayers.
        terms["players.metric.cash_growth_percent"] = ("Perubahan Koin dari Awal", "Coin Change from Start");
        // Memperbarui `terms[”players.metric.business_expense_share_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Pengeluaran untuk
        // Membeli Bahan”; bagian 2: ”Ingredient Purchase Share of Spending” dalam AddPlayers.
        terms["players.metric.business_expense_share_percent"] = ("Persentase Pengeluaran untuk Membeli Bahan", "Ingredient Purchase Share of Spending");
        // Memperbarui `terms[”players.metric.meal_order_profit_margin_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Laba Pesanan”; bagian
        // 2: ”Order Profit Margin” dalam AddPlayers.
        terms["players.metric.meal_order_profit_margin_percent"] = ("Persentase Laba Pesanan", "Order Profit Margin");
        // Memperbarui `terms[”players.metric.risk_readiness_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Risiko Selesai tanpa Tindakan
        // Darurat”; bagian 2: ”Share of Risks Resolved without Emergency Action” dalam AddPlayers.
        terms["players.metric.risk_readiness_percent"] = ("Persentase Risiko Selesai tanpa Tindakan Darurat", "Share of Risks Resolved without Emergency Action");
        // Memperbarui `terms[”players.metric.loan_burden_percent”]` menggunakan tuple yang membawa bagian 1: ”Porsi Sisa Pinjaman”; bagian 2: ”Outstanding
        // Loan Share” dalam AddPlayers.
        terms["players.metric.loan_burden_percent"] = ("Porsi Sisa Pinjaman", "Outstanding Loan Share");
        // Memperbarui `terms[”players.metric.financial_goal_progress_percent”]` menggunakan tuple yang membawa bagian 1: ”Progres Pendanaan Target
        // Finansial”; bagian 2: ”Financial Goal Funding Progress” dalam AddPlayers.
        terms["players.metric.financial_goal_progress_percent"] = ("Progres Pendanaan Target Finansial", "Financial Goal Funding Progress");
        // Memperbarui `terms[”players.metric.income_action_focus_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Aksi Utama Penghasil
        // Koin”; bagian 2: ”Income-Earning Main Action Share” dalam AddPlayers.
        terms["players.metric.income_action_focus_percent"] = ("Persentase Aksi untuk Mendapatkan Koin", "Share of Actions Used to Earn Coins");
        // Memperbarui `terms[”players.metric.ingredient_utilization_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Bahan yang Terpakai”;
        // bagian 2: ”Ingredient Utilization” dalam AddPlayers.
        terms["players.metric.ingredient_utilization_percent"] = ("Persentase Bahan yang Terpakai", "Ingredient Utilization");
        // Memperbarui `terms[”players.metric.long_term_action_share_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Aksi Utama untuk Jangka
        // Panjang”; bagian 2: ”Long-Term Main Action Share” dalam AddPlayers.
        terms["players.metric.long_term_action_share_percent"] = ("Persentase Aksi untuk Tabungan, Target, Asuransi, dan Pelunasan", "Share of Actions for Savings, Goals, Insurance, and Repayment");
        // Memperbarui `terms[”players.metric.need_fulfillment_diversity_percent”]` menggunakan tuple yang membawa bagian 1: ”Pemerataan Kartu Kebutuhan”;
        // bagian 2: ”Need Card Balance” dalam AddPlayers.
        terms["players.metric.need_fulfillment_diversity_percent"] = ("Pemerataan Kartu Kebutuhan", "Need Card Balance");
        // Memperbarui `terms[”players.metric.active_income_source_count”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Sumber Pendapatan Aktif”;
        // bagian 2: ”Active Income Source Count” dalam AddPlayers.
        terms["players.metric.active_income_source_count"] = ("Jumlah Sumber Pendapatan Aktif", "Active Income Source Count");
        // Memperbarui `terms[”players.metric.income_shares”]` menggunakan tuple yang membawa bagian 1: ”Persentase Pendapatan per Sumber”; bagian 2:
        // ”Income Share by Source” dalam AddPlayers.
        terms["players.metric.income_shares"] = ("Persentase Pendapatan per Sumber", "Income Share by Source");
        // Memperbarui `terms[”players.metric.meal_order_income”]` menggunakan tuple yang membawa bagian 1: ”Total Pendapatan Pesanan”; bagian 2: ”Total
        // Order Income” dalam AddPlayers.
        terms["players.metric.meal_order_income"] = ("Total Pendapatan Pesanan", "Total Order Income");
        // Memperbarui `terms[”players.metric.gold_sale_income”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan Penjualan Emas”; bagian 2: ”Gold Sale
        // Income” dalam AddPlayers.
        terms["players.metric.gold_sale_income"] = ("Pendapatan Penjualan Emas", "Gold Sale Income");
        // Memperbarui `terms[”players.metric.total_cash_out”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Keluar”; bagian 2: ”Total Outgoing
        // Coins” dalam AddPlayers.
        terms["players.metric.total_cash_out"] = ("Total Koin Keluar", "Total Outgoing Coins");
        // Memperbarui `terms[”players.metric.ingredient_cost_used”]` menggunakan tuple yang membawa bagian 1: ”Biaya Bahan Terpakai”; bagian 2: ”Used
        // Ingredient Cost” dalam AddPlayers.
        terms["players.metric.ingredient_cost_used"] = ("Biaya Bahan Terpakai", "Used Ingredient Cost");
        // Memperbarui `terms[”players.metric.risks_resolved_without_emergency”]` menggunakan tuple yang membawa bagian 1: ”Risiko Selesai tanpa Tindakan
        // Darurat”; bagian 2: ”Risks Resolved without Emergency Action” dalam AddPlayers.
        terms["players.metric.risks_resolved_without_emergency"] = ("Risiko Selesai tanpa Tindakan Darurat", "Risks Resolved without Emergency Action");
        // Memperbarui `terms[”players.metric.outstanding_loan”]` menggunakan tuple yang membawa bagian 1: ”Sisa Pinjaman”; bagian 2: ”Outstanding Loan”
        // dalam AddPlayers.
        terms["players.metric.outstanding_loan"] = ("Sisa Pinjaman", "Outstanding Loan");
        // Memperbarui `terms[”players.metric.liquid_assets”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Tersisa dan Tabungan”; bagian 2:
        // ”Remaining Coins and Savings Total” dalam AddPlayers.
        terms["players.metric.liquid_assets"] = ("Total Koin Tersisa dan Tabungan", "Remaining Coins and Savings Total");
        // Memperbarui `terms[”players.metric.coins_committed_to_goals”]` menggunakan tuple yang membawa bagian 1: ”Dana yang Diperhitungkan untuk Target”;
        // bagian 2: ”Funds Counted Toward Goals” dalam AddPlayers.
        terms["players.metric.coins_committed_to_goals"] = ("Dana yang Diperhitungkan untuk Target", "Funds Counted Toward Goals");
        // Memperbarui `terms[”players.metric.attempted_goal_target_total”]` menggunakan tuple yang membawa bagian 1: ”Total Biaya Target Finansial yang
        // Dicoba”; bagian 2: ”Total Cost of Attempted Financial Goals” dalam AddPlayers.
        terms["players.metric.attempted_goal_target_total"] = ("Total Biaya Target Finansial yang Dicoba", "Total Cost of Attempted Financial Goals");
        // Memperbarui `terms[”players.metric.income_main_actions”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Utama Penghasil Koin”; bagian 2:
        // ”Income-Earning Main Action Count” dalam AddPlayers.
        terms["players.metric.income_main_actions"] = ("Aksi untuk Mendapatkan Koin", "Actions Used to Earn Coins");
        // Memperbarui `terms[”players.metric.total_main_actions”]` menggunakan tuple yang membawa bagian 1: ”Total Aksi Utama”; bagian 2: ”Total Main
        // Actions” dalam AddPlayers.
        terms["players.metric.total_main_actions"] = ("Total Aksi yang Digunakan", "Total Actions Used");
        // Memperbarui `terms[”players.metric.ingredients_used_in_completed_orders”]` menggunakan tuple yang membawa bagian 1: ”Total Bahan Digunakan”;
        // bagian 2: ”Total Ingredients Used” dalam AddPlayers.
        terms["players.metric.ingredients_used_in_completed_orders"] = ("Total Bahan Digunakan", "Total Ingredients Used");
        // Memperbarui `terms[”players.metric.saving_actions”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Menabung”; bagian 2: ”Savings Action
        // Count” dalam AddPlayers.
        terms["players.metric.saving_actions"] = ("Aksi untuk Menabung", "Actions for Savings");
        terms["players.metric.saving_and_goal_actions"] = ("Aksi untuk Tabungan dan Pembelian Target", "Actions for Savings and Goal Purchases");
        // Memperbarui `terms[”players.metric.insurance_actions”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Asuransi”; bagian 2: ”Insurance
        // Action Count” dalam AddPlayers.
        terms["players.metric.insurance_actions"] = ("Aksi untuk Mengaktifkan Asuransi", "Actions for Insurance Activation");
        // Memperbarui `terms[”players.metric.loan_repayment_actions”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Pelunasan Pinjaman”; bagian 2:
        // ”Loan Repayment Action Count” dalam AddPlayers.
        terms["players.metric.loan_repayment_actions"] = ("Aksi untuk Pelunasan Pinjaman", "Actions for Loan Repayment");
        // Memperbarui `terms[”players.metric.primary_need_share”]` menggunakan tuple yang membawa bagian 1: ”Persentase Kartu Kebutuhan Primer”; bagian 2:
        // ”Primary Need Card Share” dalam AddPlayers.
        terms["players.metric.primary_need_share"] = ("Persentase Kartu Kebutuhan Primer", "Primary Need Card Share");
        // Memperbarui `terms[”players.metric.secondary_need_share”]` menggunakan tuple yang membawa bagian 1: ”Persentase Kartu Kebutuhan Sekunder”; bagian
        // 2: ”Secondary Need Card Share” dalam AddPlayers.
        terms["players.metric.secondary_need_share"] = ("Persentase Kartu Kebutuhan Sekunder", "Secondary Need Card Share");
        // Memperbarui `terms[”players.metric.tertiary_need_share”]` menggunakan tuple yang membawa bagian 1: ”Persentase Kartu Kebutuhan Tersier”; bagian
        // 2: ”Tertiary Need Card Share” dalam AddPlayers.
        terms["players.metric.tertiary_need_share"] = ("Persentase Kartu Kebutuhan Tersier", "Tertiary Need Card Share");
        // Memperbarui `terms[”players.metric.donated_resource_share”]` menggunakan tuple yang membawa bagian 1: ”Porsi Donasi dari Koin Tersisa dan
        // Donasi”; bagian 2: ”Donation Share of Remaining and Donated Coins” dalam AddPlayers.
        terms["players.metric.donated_resource_share"] = ("Porsi Donasi dari Koin Tersisa dan Donasi", "Donation Share of Remaining and Donated Coins");
        // Memperbarui `terms[”players.metric.total_happiness_points”]` menggunakan tuple yang membawa bagian 1: ”Total Poin Kebahagiaan”; bagian 2: ”Total
        // Happiness Points” dalam AddPlayers.
        terms["players.metric.total_happiness_points"] = ("Total Poin Kebahagiaan", "Total Happiness Points");
        // Memperbarui `terms[”players.metric.need_card_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Kartu Kebutuhan”; bagian 2:
        // ”Need Card Happiness Points” dalam AddPlayers.
        terms["players.metric.need_card_points"] = ("Poin Kebahagiaan Kartu Kebutuhan", "Need Card Happiness Points");
        // Memperbarui `terms[”players.metric.need_set_bonus_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Bonus Set Kebutuhan”;
        // bagian 2: ”Need Set Bonus Happiness Points” dalam AddPlayers.
        terms["players.metric.need_set_bonus_points"] = ("Poin Kebahagiaan Bonus Set Kebutuhan", "Need Set Bonus Happiness Points");
        // Memperbarui `terms[”players.metric.donation_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan dari Donasi”; bagian 2:
        // ”Happiness Points from Donations” dalam AddPlayers.
        terms["players.metric.donation_points"] = ("Poin Kebahagiaan dari Donasi", "Happiness Points from Donations");
        // Memperbarui `terms[”players.metric.gold_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Emas”; bagian 2: ”Gold Happiness
        // Points” dalam AddPlayers.
        terms["players.metric.gold_points"] = ("Poin Kebahagiaan Emas", "Gold Happiness Points");
        // Memperbarui `terms[”players.metric.pension_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Dana Pensiun”; bagian 2: ”Pension
        // Happiness Points” dalam AddPlayers.
        terms["players.metric.pension_points"] = ("Poin Kebahagiaan Dana Pensiun", "Pension Happiness Points");
        // Memperbarui `terms[”players.metric.financial_goal_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Target Finansial”; bagian
        // 2: ”Financial Goal Happiness Points” dalam AddPlayers.
        terms["players.metric.financial_goal_points"] = ("Poin Kebahagiaan Target Finansial", "Financial Goal Happiness Points");
        // Memperbarui `terms[”players.metric.mission_penalty_points”]` menggunakan tuple yang membawa bagian 1: ”Pengurangan Poin Kebahagiaan Misi”; bagian
        // 2: ”Mission Happiness Point Deduction” dalam AddPlayers.
        terms["players.metric.mission_penalty_points"] = ("Pengurangan Poin Kebahagiaan Misi", "Mission Happiness Point Deduction");
        // Memperbarui `terms[”players.metric.loan_penalty_points”]` menggunakan tuple yang membawa bagian 1: ”Pengurangan Poin Kebahagiaan Pinjaman”;
        // bagian 2: ”Loan Happiness Point Deduction” dalam AddPlayers.
        terms["players.metric.loan_penalty_points"] = ("Pengurangan Poin Kebahagiaan Pinjaman", "Loan Happiness Point Deduction");
        // Memperbarui `terms[”players.group.cash_growth_components”]` menggunakan tuple yang membawa bagian 1: ”Data Perbandingan Koin Tersisa dan Koin
        // Awal”; bagian 2: ”Remaining and Starting Coin Inputs” dalam AddPlayers.
        terms["players.group.cash_growth_components"] = ("Data Perbandingan Koin Tersisa dan Koin Awal", "Remaining and Starting Coin Inputs");
        // Memperbarui `terms[”players.group.business_expense_share_components”]` menggunakan tuple yang membawa bagian 1: ”Data Pengeluaran untuk Membeli
        // Bahan”; bagian 2: ”Ingredient Purchase Spending Inputs” dalam AddPlayers.
        terms["players.group.business_expense_share_components"] = ("Data Pengeluaran untuk Membeli Bahan", "Ingredient Purchase Spending Inputs");
        // Memperbarui `terms[”players.group.meal_order_profit_margin_components”]` menggunakan tuple yang membawa bagian 1: ”Data Persentase Laba Pesanan”;
        // bagian 2: ”Order Profit Margin Inputs” dalam AddPlayers.
        terms["players.group.meal_order_profit_margin_components"] = ("Data Persentase Laba Pesanan", "Order Profit Margin Inputs");
        // Memperbarui `terms[”players.group.risk_readiness_components”]` menggunakan tuple yang membawa bagian 1: ”Data Risiko dan Tindakan Darurat”;
        // bagian 2: ”Risk and Emergency Action Inputs” dalam AddPlayers.
        terms["players.group.risk_readiness_components"] = ("Data Risiko dan Tindakan Darurat", "Risk and Emergency Action Inputs");
        // Memperbarui `terms[”players.group.loan_burden_components”]` menggunakan tuple yang membawa bagian 1: ”Data Porsi Sisa Pinjaman”; bagian 2:
        // ”Outstanding Loan Share Inputs” dalam AddPlayers.
        terms["players.group.loan_burden_components"] = ("Data Porsi Sisa Pinjaman", "Outstanding Loan Share Inputs");
        // Memperbarui `terms[”players.group.financial_goal_progress_components”]` menggunakan tuple yang membawa bagian 1: ”Data Pendanaan Target
        // Finansial”; bagian 2: ”Financial Goal Funding Inputs” dalam AddPlayers.
        terms["players.group.financial_goal_progress_components"] = ("Data Pendanaan Target Finansial", "Financial Goal Funding Inputs");
        // Memperbarui `terms[”players.group.income_action_focus_components”]` menggunakan tuple yang membawa bagian 1: ”Data Aksi Utama Penghasil Koin”;
        // bagian 2: ”Income-Earning Main Action Inputs” dalam AddPlayers.
        terms["players.group.income_action_focus_components"] = ("Data Aksi untuk Mendapatkan Koin", "Action Inputs for Earning Coins");
        // Memperbarui `terms[”players.group.ingredient_utilization_components”]` menggunakan tuple yang membawa bagian 1: ”Data Bahan Terkumpul dan
        // Digunakan”; bagian 2: ”Collected and Used Ingredient Inputs” dalam AddPlayers.
        terms["players.group.ingredient_utilization_components"] = ("Data Bahan Terkumpul dan Digunakan", "Collected and Used Ingredient Inputs");
        // Memperbarui `terms[”players.group.long_term_action_share_components”]` menggunakan tuple yang membawa bagian 1: ”Data Aksi Utama untuk Jangka
        // Panjang”; bagian 2: ”Long-Term Main Action Inputs” dalam AddPlayers.
        terms["players.group.long_term_action_share_components"] = ("Data Aksi untuk Tabungan, Target, Asuransi, dan Pelunasan", "Action Inputs for Savings, Goals, Insurance, and Repayment");
        // Memperbarui `terms[”players.group.need_fulfillment_diversity_components”]` menggunakan tuple yang membawa bagian 1: ”Data Pemerataan Kartu
        // Kebutuhan”; bagian 2: ”Need Card Balance Inputs” dalam AddPlayers.
        terms["players.group.need_fulfillment_diversity_components"] = ("Data Pemerataan Kartu Kebutuhan", "Need Card Balance Inputs");
        // Memperbarui `terms[”players.group.happiness_points_composition”]` menggunakan tuple yang membawa bagian 1: ”Komposisi Poin Kebahagiaan”; bagian
        // 2: ”Happiness Points Composition” dalam AddPlayers.
        terms["players.group.happiness_points_composition"] = ("Komposisi Poin Kebahagiaan", "Happiness Points Composition");
        // Memperbarui `terms[”players.metric.income_diversification_ratio”]` menggunakan tuple yang membawa bagian 1: ”Rasio Diversifikasi Pendapatan”;
        // bagian 2: ”Income Diversification Ratio” dalam AddPlayers.
        terms["players.metric.income_diversification_ratio"] = ("Rasio Diversifikasi Pendapatan", "Income Diversification Ratio");
        // Memperbarui `terms[”players.metric.freelance_income”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan Kerja Lepas”; bagian 2: ”Freelance
        // Income” dalam AddPlayers.
        terms["players.metric.freelance_income"] = ("Pendapatan Kerja Lepas", "Freelance Income");
        // Memperbarui `terms[”players.metric.meal_income”]` menggunakan tuple yang membawa bagian 1: ”Total Pendapatan Pesanan”; bagian 2: ”Total Order
        // Income” dalam AddPlayers.
        terms["players.metric.meal_income"] = ("Total Pendapatan Pesanan", "Total Order Income");
        // Memperbarui `terms[”players.metric.gold_income”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan Penjualan Emas”; bagian 2: ”Gold Sale
        // Income” dalam AddPlayers.
        terms["players.metric.gold_income"] = ("Pendapatan Penjualan Emas", "Gold Sale Income");
        // Memperbarui `terms[”players.metric.donations_received”]` menggunakan tuple yang membawa bagian 1: ”Donasi Diterima”; bagian 2: ”Donations
        // Received” dalam AddPlayers.
        terms["players.metric.donations_received"] = ("Donasi Diterima", "Donations Received");
        // Memperbarui `terms[”players.metric.other_income”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan Lain”; bagian 2: ”Other Income” dalam
        // AddPlayers.
        terms["players.metric.other_income"] = ("Pendapatan Lain", "Other Income");
        // Memperbarui `terms[”players.metric.total_income”]` menggunakan tuple yang membawa bagian 1: ”Total Pendapatan”; bagian 2: ”Total Income” dalam
        // AddPlayers.
        terms["players.metric.total_income"] = ("Total Pendapatan", "Total Income");
        // Memperbarui `terms[”players.metric.n_active_income_sources”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Sumber Pendapatan Aktif”; bagian
        // 2: ”Active Income Source Count” dalam AddPlayers.
        terms["players.metric.n_active_income_sources"] = ("Jumlah Sumber Pendapatan Aktif", "Active Income Source Count");
        // Memperbarui `terms[”players.metric.income_share_i”]` menggunakan tuple yang membawa bagian 1: ”Persentase Pendapatan per Sumber”; bagian 2:
        // ”Income Share by Source” dalam AddPlayers.
        terms["players.metric.income_share_i"] = ("Persentase Pendapatan per Sumber", "Income Share by Source");
        // Memperbarui `terms[”players.metric.expense_management_efficiency”]` menggunakan tuple yang membawa bagian 1: ”Belanja untuk Bahan Terpakai”;
        // bagian 2: ”Spending on Used Ingredients” dalam AddPlayers.
        terms["players.metric.expense_management_efficiency"] = ("Belanja untuk Bahan Terpakai", "Spending on Used Ingredients");
        // Memperbarui `terms[”players.metric.essential_expenses”]` menggunakan tuple yang membawa bagian 1: ”Biaya Bahan Terpakai”; bagian 2: ”Used
        // Ingredient Cost” dalam AddPlayers.
        terms["players.metric.essential_expenses"] = ("Biaya Bahan Terpakai", "Used Ingredient Cost");
        // Memperbarui `terms[”players.metric.total_expenses”]` menggunakan tuple yang membawa bagian 1: ”Total Koin Keluar”; bagian 2: ”Total Outgoing
        // Coins” dalam AddPlayers.
        terms["players.metric.total_expenses"] = ("Total Koin Keluar", "Total Outgoing Coins");
        // Memperbarui `terms[”players.metric.business_efficiency_ratio”]` menggunakan tuple yang membawa bagian 1: ”Rasio Efisiensi Bisnis”; bagian 2:
        // ”Business Efficiency Ratio” dalam AddPlayers.
        terms["players.metric.business_efficiency_ratio"] = ("Rasio Efisiensi Bisnis", "Business Efficiency Ratio");
        // Memperbarui `terms[”players.metric.gold_roi_percentage”]` menggunakan tuple yang membawa bagian 1: ”Persentase ROI Emas”; bagian 2: ”Gold ROI %”
        // dalam AddPlayers.
        terms["players.metric.gold_roi_percentage"] = ("Persentase ROI Emas", "Gold ROI %");
        // Memperbarui `terms[”players.metric.risk_exposure_percentage”]` menggunakan tuple yang membawa bagian 1: ”Persentase Eksposur Risiko”; bagian 2:
        // ”Risk Exposure %” dalam AddPlayers.
        terms["players.metric.risk_exposure_percentage"] = ("Persentase Eksposur Risiko", "Risk Exposure %");
        // Memperbarui `terms[”players.metric.risk_mitigation_effectiveness”]` menggunakan tuple yang membawa bagian 1: ”Efektivitas Mitigasi Risiko”;
        // bagian 2: ”Risk Mitigation Effectiveness” dalam AddPlayers.
        terms["players.metric.risk_mitigation_effectiveness"] = ("Efektivitas Mitigasi Risiko", "Risk Mitigation Effectiveness");
        // Memperbarui `terms[”players.metric.risk_appetite_score”]` menggunakan tuple yang membawa bagian 1: ”Tingkat Risiko yang Diambil”; bagian 2: ”Risk
        // Taken Level” dalam AddPlayers.
        terms["players.metric.risk_appetite_score"] = ("Tingkat Risiko yang Diambil", "Risk Taken Level");
        // Memperbarui `terms[”players.metric.risk_acceptance_rate”]` menggunakan tuple yang membawa bagian 1: ”Tingkat Penerimaan Risiko”; bagian 2: ”Risk
        // Acceptance Rate” dalam AddPlayers.
        terms["players.metric.risk_acceptance_rate"] = ("Tingkat Penerimaan Risiko", "Risk Acceptance Rate");
        // Memperbarui `terms[”players.metric.average_risk_cost”]` menggunakan tuple yang membawa bagian 1: ”Rata-rata Biaya Risiko”; bagian 2: ”Average
        // Risk Cost” dalam AddPlayers.
        terms["players.metric.average_risk_cost"] = ("Rata-rata Biaya Risiko", "Average Risk Cost");
        // Memperbarui `terms[”players.metric.insurance_activation_rate”]` menggunakan tuple yang membawa bagian 1: ”Tingkat Aktivasi Asuransi”; bagian 2:
        // ”Insurance Activation Rate” dalam AddPlayers.
        terms["players.metric.insurance_activation_rate"] = ("Tingkat Aktivasi Asuransi", "Insurance Activation Rate");
        // Memperbarui `terms[”players.metric.insurance_coverage_rate”]` menggunakan tuple yang membawa bagian 1: ”Rasio Perlindungan Asuransi”; bagian 2:
        // ”Insurance Coverage Rate” dalam AddPlayers.
        terms["players.metric.insurance_coverage_rate"] = ("Rasio Perlindungan Asuransi", "Insurance Coverage Rate");
        // Memperbarui `terms[”players.metric.risk_cost_intensity”]` menggunakan tuple yang membawa bagian 1: ”Beban Biaya Risiko”; bagian 2: ”Risk Cost
        // Burden” dalam AddPlayers.
        terms["players.metric.risk_cost_intensity"] = ("Beban Biaya Risiko", "Risk Cost Burden");
        // Memperbarui `terms[”players.metric.debt_leverage_ratio”]` menggunakan tuple yang membawa bagian 1: ”Beban Utang terhadap Kas”; bagian 2: ”Debt
        // Burden Compared with Cash” dalam AddPlayers.
        terms["players.metric.debt_leverage_ratio"] = ("Beban Utang terhadap Kas", "Debt Burden Compared with Cash");
        // Memperbarui `terms[”players.metric.goal_ambition”]` menggunakan tuple yang membawa bagian 1: ”Ambisi Target Finansial”; bagian 2: ”Goal Ambition”
        // dalam AddPlayers.
        terms["players.metric.goal_ambition"] = ("Ambisi Target Finansial", "Goal Ambition");
        // Memperbarui `terms[”players.metric.goal_ambition_index”]` menggunakan tuple yang membawa bagian 1: ”Keseriusan Mengejar Target”; bagian 2: ”Goal
        // Pursuit” dalam AddPlayers.
        terms["players.metric.goal_ambition_index"] = ("Keseriusan Mengejar Target", "Goal Pursuit");
        // Memperbarui `terms[”players.metric.goal_setting_ambition”]` menggunakan tuple yang membawa bagian 1: ”Skor Ambisi Target (Rumus Dokumen)”; bagian
        // 2: ”Goal Ambition Score (Document Formula)” dalam AddPlayers.
        terms["players.metric.goal_setting_ambition"] = ("Skor Ambisi Target (Rumus Dokumen)", "Goal Ambition Score (Document Formula)");
        // Memperbarui `terms[”players.metric.goal_attempt_rate”]` menggunakan tuple yang membawa bagian 1: ”Persentase Target yang Dicoba”; bagian 2:
        // ”Goals Attempted Percentage” dalam AddPlayers.
        terms["players.metric.goal_attempt_rate"] = ("Persentase Target yang Dicoba", "Goals Attempted Percentage");
        // Memperbarui `terms[”players.metric.goal_investment_rate”]` menggunakan tuple yang membawa bagian 1: ”Bagian Dana untuk Target”; bagian 2: ”Share
        // of Funds for Goals” dalam AddPlayers.
        terms["players.metric.goal_investment_rate"] = ("Bagian Dana untuk Target", "Share of Funds for Goals");
        // Memperbarui `terms[”players.metric.action_efficiency_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Aksi Utama Penghasil Koin”;
        // bagian 2: ”Income-Earning Main Action Share” dalam AddPlayers.
        terms["players.metric.action_efficiency_percent"] = ("Persentase Aksi untuk Mendapatkan Koin", "Share of Actions Used to Earn Coins");
        // Memperbarui `terms[”players.metric.action_diversity_score_avg”]` menggunakan tuple yang membawa bagian 1: ”Rata-rata Keberagaman Aksi”; bagian 2:
        // ”Average Action Diversity” dalam AddPlayers.
        terms["players.metric.action_diversity_score_avg"] = ("Rata-rata Keberagaman Aksi", "Average Action Diversity");
        // Memperbarui `terms[”players.metric.income_producing_actions”]` menggunakan tuple yang membawa bagian 1: ”Aksi Penghasil Pemasukan”; bagian 2:
        // ”Income-Producing Actions” dalam AddPlayers.
        terms["players.metric.income_producing_actions"] = ("Aksi untuk Mendapatkan Koin", "Actions Used to Earn Coins");
        // Memperbarui `terms[”players.metric.all_player_actions”]` menggunakan tuple yang membawa bagian 1: ”Seluruh Aksi Pemain”; bagian 2: ”All Player
        // Actions” dalam AddPlayers.
        terms["players.metric.all_player_actions"] = ("Total Aksi yang Digunakan", "Total Actions Used");
        // Memperbarui `terms[”players.metric.meal_order_success_rate”]` menggunakan tuple yang membawa bagian 1: ”Pesanan yang Berhasil”; bagian 2:
        // ”Successful Orders” dalam AddPlayers.
        terms["players.metric.meal_order_success_rate"] = ("Pesanan yang Berhasil", "Successful Orders");
        // Memperbarui `terms[”players.metric.planning_horizon_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Aksi Utama untuk Jangka
        // Panjang”; bagian 2: ”Long-Term Main Action Share” dalam AddPlayers.
        terms["players.metric.planning_horizon_percent"] = ("Persentase Aksi untuk Tabungan, Target, Asuransi, dan Pelunasan", "Share of Actions for Savings, Goals, Insurance, and Repayment");
        // Memperbarui `terms[”players.metric.savings_actions”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Menabung”; bagian 2: ”Savings Action
        // Count” dalam AddPlayers.
        terms["players.metric.savings_actions"] = ("Aksi untuk Menabung", "Actions for Savings");
        // Memperbarui `terms[”players.metric.financial_goal_actions”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Target Finansial”; bagian 2:
        // ”Financial Goal Action Count” dalam AddPlayers.
        terms["players.metric.financial_goal_actions"] = ("Aksi untuk Target Finansial", "Actions for Financial Goals");
        // Memperbarui `terms[”players.metric.insurance_premium_actions”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi Asuransi”; bagian 2:
        // ”Insurance Action Count” dalam AddPlayers.
        terms["players.metric.insurance_premium_actions"] = ("Aksi untuk Mengaktifkan Asuransi", "Actions for Insurance Activation");
        // Memperbarui `terms[”players.metric.p_primary”]` menggunakan tuple yang membawa bagian 1: ”Persentase Kartu Kebutuhan Primer”; bagian 2: ”Primary
        // Need Card Share” dalam AddPlayers.
        terms["players.metric.p_primary"] = ("Persentase Kartu Kebutuhan Primer", "Primary Need Card Share");
        // Memperbarui `terms[”players.metric.p_secondary”]` menggunakan tuple yang membawa bagian 1: ”Persentase Kartu Kebutuhan Sekunder”; bagian 2:
        // ”Secondary Need Card Share” dalam AddPlayers.
        terms["players.metric.p_secondary"] = ("Persentase Kartu Kebutuhan Sekunder", "Secondary Need Card Share");
        // Memperbarui `terms[”players.metric.p_tertiary”]` menggunakan tuple yang membawa bagian 1: ”Persentase Kartu Kebutuhan Tersier”; bagian 2:
        // ”Tertiary Need Card Share” dalam AddPlayers.
        terms["players.metric.p_tertiary"] = ("Persentase Kartu Kebutuhan Tersier", "Tertiary Need Card Share");
        // Memperbarui `terms[”players.metric.fulfillment_diversity_document_formula”]` menggunakan tuple yang membawa bagian 1: ”Konsentrasi Kebutuhan
        // (Rumus Dokumen)”; bagian 2: ”Need Concentration (Document Formula)” dalam AddPlayers.
        terms["players.metric.fulfillment_diversity_document_formula"] = ("Konsentrasi Kebutuhan (Rumus Dokumen)", "Need Concentration (Document Formula)");
        // Memperbarui `terms[”players.metric.donation_aggressiveness_percent”]` menggunakan tuple yang membawa bagian 1: ”Persentase Agresivitas Donasi”;
        // bagian 2: ”Donation Aggressiveness %” dalam AddPlayers.
        terms["players.metric.donation_aggressiveness_percent"] = ("Persentase Agresivitas Donasi", "Donation Aggressiveness %");
        // Memperbarui `terms[”players.metric.donation_stability_std_deviation”]` menggunakan tuple yang membawa bagian 1: ”Variasi Nominal Donasi”; bagian
        // 2: ”Donation Amount Variation” dalam AddPlayers.
        terms["players.metric.donation_stability_std_deviation"] = ("Variasi Nominal Donasi", "Donation Amount Variation");
        // Memperbarui `terms[”players.metric.donation_ratio”]` menggunakan tuple yang membawa bagian 1: ”Bagian Kas untuk Donasi”; bagian 2: ”Share of Cash
        // Donated” dalam AddPlayers.
        terms["players.metric.donation_ratio"] = ("Bagian Kas untuk Donasi", "Share of Cash Donated");
        // Memperbarui `terms[”players.metric.friday_participation_rate”]` menggunakan tuple yang membawa bagian 1: ”Persentase Jumat dengan Donasi”; bagian
        // 2: ”Share of Fridays with Donations” dalam AddPlayers.
        terms["players.metric.friday_participation_rate"] = ("Persentase Jumat dengan Donasi", "Share of Fridays with Donations");
        // Memperbarui `terms[”players.metric.donation_commitment_score”]` menggunakan tuple yang membawa bagian 1: ”Skor Komitmen Donasi”; bagian 2:
        // ”Donation Commitment Score” dalam AddPlayers.
        terms["players.metric.donation_commitment_score"] = ("Skor Komitmen Donasi", "Donation Commitment Score");
        // Memperbarui `terms[”players.metric.donation_stability”]` menggunakan tuple yang membawa bagian 1: ”Stabilitas Donasi”; bagian 2: ”Donation
        // Stability” dalam AddPlayers.
        terms["players.metric.donation_stability"] = ("Stabilitas Donasi", "Donation Stability");
        // Memperbarui `terms[”players.metric.donation_stability_index”]` menggunakan tuple yang membawa bagian 1: ”Keteraturan Jumlah Donasi”; bagian 2:
        // ”Donation Amount Regularity” dalam AddPlayers.
        terms["players.metric.donation_stability_index"] = ("Keteraturan Jumlah Donasi", "Donation Amount Regularity");
        // Memperbarui `terms[”players.metric.risk_appetite_score_normalized”]` menggunakan tuple yang membawa bagian 1: ”Tingkat Risiko yang Diambil
        // (0–100)”; bagian 2: ”Risk Taken Level (0–100)” dalam AddPlayers.
        terms["players.metric.risk_appetite_score_normalized"] = ("Tingkat Risiko yang Diambil (0–100)", "Risk Taken Level (0–100)");
        // Memperbarui `terms[”players.metric.sharia_loans_outstanding_coins”]` menggunakan tuple yang membawa bagian 1: ”Sisa Pinjaman”; bagian 2:
        // ”Outstanding Loan” dalam AddPlayers.
        terms["players.metric.sharia_loans_outstanding_coins"] = ("Sisa Pinjaman", "Outstanding Loan");
        // Memperbarui `terms[”players.metric.total_happiness_pts”]` menggunakan tuple yang membawa bagian 1: ”Total Poin Kebahagiaan”; bagian 2: ”Total
        // Happiness Points” dalam AddPlayers.
        terms["players.metric.total_happiness_pts"] = ("Total Poin Kebahagiaan", "Total Happiness Points");
        // Memperbarui `terms[”players.metric.need_cards_pts”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Kartu Kebutuhan”; bagian 2: ”Need
        // Card Happiness Points” dalam AddPlayers.
        terms["players.metric.need_cards_pts"] = ("Poin Kebahagiaan Kartu Kebutuhan", "Need Card Happiness Points");
        // Memperbarui `terms[”players.metric.need_set_bonus_pts”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Bonus Set Kebutuhan”; bagian
        // 2: ”Need Set Bonus Happiness Points” dalam AddPlayers.
        terms["players.metric.need_set_bonus_pts"] = ("Poin Kebahagiaan Bonus Set Kebutuhan", "Need Set Bonus Happiness Points");
        // Memperbarui `terms[”players.metric.donations_pts”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan dari Donasi”; bagian 2: ”Happiness
        // Points from Donations” dalam AddPlayers.
        terms["players.metric.donations_pts"] = ("Poin Kebahagiaan dari Donasi", "Happiness Points from Donations");
        // Memperbarui `terms[”players.metric.gold_pts”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Emas”; bagian 2: ”Gold Happiness
        // Points” dalam AddPlayers.
        terms["players.metric.gold_pts"] = ("Poin Kebahagiaan Emas", "Gold Happiness Points");
        // Memperbarui `terms[”players.metric.pension_pts”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Dana Pensiun”; bagian 2: ”Pension
        // Happiness Points” dalam AddPlayers.
        terms["players.metric.pension_pts"] = ("Poin Kebahagiaan Dana Pensiun", "Pension Happiness Points");
        // Memperbarui `terms[”players.metric.financial_goals_pts”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Target Finansial”; bagian 2:
        // ”Financial Goal Happiness Points” dalam AddPlayers.
        terms["players.metric.financial_goals_pts"] = ("Poin Kebahagiaan Target Finansial", "Financial Goal Happiness Points");
        // Memperbarui `terms[”players.metric.mission_bonus_pts”]` menggunakan tuple yang membawa bagian 1: ”Pengurangan Poin Kebahagiaan Misi”; bagian 2:
        // ”Mission Happiness Point Deduction” dalam AddPlayers.
        terms["players.metric.mission_bonus_pts"] = ("Pengurangan Poin Kebahagiaan Misi", "Mission Happiness Point Deduction");
        // Memperbarui `terms[”players.metric.loan_penalty_pts”]` menggunakan tuple yang membawa bagian 1: ”Pengurangan Poin Kebahagiaan Pinjaman”; bagian
        // 2: ”Loan Happiness Point Deduction” dalam AddPlayers.
        terms["players.metric.loan_penalty_pts"] = ("Pengurangan Poin Kebahagiaan Pinjaman", "Loan Happiness Point Deduction");
        // Memperbarui `terms[”players.support.meaning.risk_readiness”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan persentase risiko yang
        // selesai tanpa Tindakan Darurat, bukan ha...; bagian 2: ”{0} shows the percentage of risks resolved without Emergency Actions, not just ... dalam
        // AddPlayers.
        terms["players.support.meaning.risk_readiness"] = ("{0} menunjukkan persentase risiko yang selesai tanpa Tindakan Darurat, bukan hanya risiko yang ditanggung asuransi.", "{0} shows the percentage of risks resolved without Emergency Actions, not just those covered by insurance.");
        // Memperbarui `terms[”players.support.meaning.loan_burden”]` menggunakan tuple yang membawa bagian 1: ”{0} membandingkan Sisa Pinjaman dengan Sisa
        // Pinjaman ditambah Total Koin Tersis...; bagian 2: ”{0} compares Outstanding Loan with Outstanding Loan plus Remaining Coins and Sa... dalam
        // AddPlayers.
        terms["players.support.meaning.loan_burden"] = ("{0} membandingkan Sisa Pinjaman dengan Sisa Pinjaman ditambah Total Koin Tersisa dan Tabungan.", "{0} compares Outstanding Loan with Outstanding Loan plus Remaining Coins and Savings Total.");
        // Memperbarui `terms[”players.support.meaning.ingredient_utilization”]` menggunakan tuple yang membawa bagian 1: ”{0} menunjukkan bagian Bahan
        // Terkumpul yang sudah menjadi Total Bahan Digunakan...; bagian 2: ”{0} shows the share of Ingredients Collected counted as Total Ingredients Used
        // ... dalam AddPlayers.
        terms["players.support.meaning.ingredient_utilization"] = ("{0} menunjukkan bagian Bahan Terkumpul yang sudah menjadi Total Bahan Digunakan pada pesanan selesai.", "{0} shows the share of Ingredients Collected counted as Total Ingredients Used in completed orders.");
        // Memperbarui `terms[”players.support.meaning.ingredients_remaining”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah jumlah kartu bahan yang
        // masih dimiliki. Bahan yang sudah digunakan u...; bagian 2: ”{0} is the number of ingredient cards still owned. Cards already used for order...
        // dalam AddPlayers.
        terms["players.support.meaning.ingredients_remaining"] = ("{0} adalah jumlah kartu bahan yang masih dimiliki. Bahan yang sudah digunakan untuk pesanan atau dibuang tidak termasuk.", "{0} is the number of ingredient cards still owned. Cards already used for orders or discarded are not included.");
        // Memperbarui `terms[”players.support.meaning.ingredients_per_order”]` menggunakan tuple yang membawa bagian 1: ”Setiap baris menunjukkan pesanan
        // yang selesai, sesuai urutan penyelesaiannya, d...; bagian 2: ”Each row shows a completed order in completion order and the number of ingredie...
        // dalam AddPlayers.
        terms["players.support.meaning.ingredients_per_order"] = ("Setiap baris menunjukkan pesanan yang selesai, sesuai urutan penyelesaiannya, dan jumlah kartu bahan yang dipakai untuk pesanan tersebut. Angka pesanan bukan nomor pada kartu pesanan.", "Each row shows a completed order in completion order and the number of ingredient cards used for it. The order number is not the ID printed on the order card.");
        // Memperbarui `terms[”players.support.meaning.orders_per_active_day”]` menggunakan tuple yang membawa bagian 1: ”{0} membagi jumlah Pesanan Selesai
        // dengan jumlah hari ketika pemain menjalankan...; bagian 2: ”{0} divides Completed Orders by the number of days when the player performed at...
        // dalam AddPlayers.
        terms["players.support.meaning.orders_per_active_day"] = ("{0} membagi jumlah Pesanan Selesai dengan jumlah hari ketika pemain melakukan minimal satu aksi. Hari tanpa aksi tidak menjadi pembagi.", "{0} divides Completed Orders by the number of days when the player took at least one action. Days without actions are excluded from the denominator.");
        // Memperbarui `terms[”players.support.meaning.income_per_order”]` menggunakan tuple yang membawa bagian 1: ”Menampilkan koin yang diterima dari
        // setiap pesanan yang selesai, sebelum dikura...; bagian 2: ”Shows the coins received from each completed order before deducting ingredient ...
        // dalam AddPlayers.
        terms["players.support.meaning.income_per_order"] = ("Menampilkan koin yang diterima dari setiap pesanan yang selesai, sebelum dikurangi biaya bahan. Pesanan ke-1 berarti pesanan pertama yang diselesaikan pemain, bukan nomor kartu pesanan.", "Shows the coins received from each completed order before deducting ingredient costs. Order No. 1 means the first order completed by the player, not the ID printed on the order card.");
        // Memperbarui `terms[”players.support.meaning.ingredient_types_remaining”]` menggunakan tuple yang membawa bagian 1: ”{0} memecah Total Bahan
        // Tersisa berdasarkan nama bahan agar isi persediaan terl...; bagian 2: ”{0} breaks Total Remaining Ingredients down by ingredient name so the
        // inventory... dalam AddPlayers.
        terms["players.support.meaning.ingredient_types_remaining"] = ("{0} memecah Total Bahan Tersisa berdasarkan nama bahan agar isi persediaan terlihat jelas.", "{0} breaks Total Remaining Ingredients down by ingredient name so the inventory is clear.");
        // Memperbarui `terms[”players.support.meaning.ingredients_discarded”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah kartu bahan yang
        // dilepas tanpa digunakan untuk menyelesaikan pesanan...; bagian 2: ”{0} is ingredient cards removed without completing an order; it does not
        // includ... dalam AddPlayers.
        terms["players.support.meaning.ingredients_discarded"] = ("{0} adalah kartu bahan yang dilepas tanpa digunakan untuk menyelesaikan pesanan; nilainya tidak termasuk bahan yang masih dimiliki.", "{0} is ingredient cards removed without completing an order; it does not include ingredients still owned.");
        // Memperbarui `terms[”players.support.meaning.gold_purchased”]` menggunakan tuple yang membawa bagian 1: ”{0} hanya menghitung kartu emas yang
        // dibeli setelah permainan dimulai; Kartu Em...; bagian 2: ”{0} counts only gold cards bought after gameplay began; Starting Gold Cards are...
        // dalam AddPlayers.
        terms["players.support.meaning.gold_purchased"] = ("{0} hanya menghitung kartu emas yang dibeli setelah permainan dimulai; Kartu Emas Awal dihitung terpisah.", "{0} counts only gold cards bought after gameplay began; Starting Gold Cards are counted separately.");
        // Memperbarui `terms[”players.support.meaning.gold_remaining”]` menggunakan tuple yang membawa bagian 1: ”{0} dihitung sebagai Kartu Emas Awal +
        // Kartu Emas Dibeli selama Permainan − Kar...; bagian 2: ”{0} equals Starting Gold Cards + Gold Cards Purchased during Gameplay − Gold Ca... dalam
        // AddPlayers.
        terms["players.support.meaning.gold_remaining"] = ("{0} dihitung sebagai Kartu Emas Awal + Kartu Emas Dibeli selama Permainan − Kartu Emas Dijual.", "{0} equals Starting Gold Cards + Gold Cards Purchased during Gameplay − Gold Cards Sold.");
        // Memperbarui `terms[”players.support.meaning.gold_cashflow”]` menggunakan tuple yang membawa bagian 1: ”{0} = Pendapatan Penjualan Emas − Total
        // Biaya Pembelian Emas. Nilai ini hanya m...; bagian 2: ”{0} = Gold Sale Income − Total Gold Purchase Cost. It shows cashflow only and d... dalam
        // AddPlayers.
        terms["players.support.meaning.gold_cashflow"] = ("{0} = Pendapatan Penjualan Emas − Total Biaya Pembelian Emas. Nilai ini hanya menunjukkan arus kas dan tidak menambahkan nilai kartu emas yang masih dimiliki.", "{0} = Gold Sale Income − Total Gold Purchase Cost. It shows cashflow only and does not add the value of gold cards still held.");
        // Memperbarui `terms[”players.support.meaning.pension_ingredient_value”]` menggunakan tuple yang membawa bagian 1: ”{0} menilai setiap kartu bahan
        // yang masih dimiliki sebesar 1 koin untuk perhitu...; bagian 2: ”{0} values each remaining ingredient card at 1 coin for the Pension Fund
        // calcul... dalam AddPlayers.
        terms["players.support.meaning.pension_ingredient_value"] = ("{0} menilai setiap kartu bahan yang masih dimiliki sebesar 1 koin untuk perhitungan Dana Pensiun; ini bukan harga beli asli kartu bahan.", "{0} values each remaining ingredient card at 1 coin for the Pension Fund calculation; it is not the card's original purchase price.");
        // Memperbarui `terms[”players.support.meaning.risk_nominal_cost”]` menggunakan tuple yang membawa bagian 1: ”{0} menjumlahkan nominal dampak koin
        // yang tercetak pada kartu risiko. Nilai ini...; bagian 2: ”{0} totals the nominal coin impact printed on risk cards. It still includes car...
        // dalam AddPlayers.
        terms["players.support.meaning.risk_nominal_cost"] = ("{0} menjumlahkan nominal dampak koin yang tercetak pada kartu risiko. Nilai ini tetap menampilkan dampak kartu yang diselesaikan dengan asuransi atau Tindakan Darurat; koin yang benar-benar masuk atau keluar dilihat pada Riwayat Transaksi.", "{0} totals the nominal coin impact printed on risk cards. It still includes card effects resolved by insurance or an Emergency Action; actual incoming or outgoing coins are shown in Transaction History.");
        // Memperbarui `terms[”players.support.meaning.action_usage_history”]` menggunakan tuple yang membawa bagian 1: ”Setiap baris menunjukkan aksi utama
        // pada satu hari. Aksi Berulang bernilai 1 ji...; bagian 2: ”Each row shows one day's main actions. Repeated Actions equals 1 when both acti...
        // dalam AddPlayers.
        terms["players.support.meaning.action_usage_history"] = ("Contoh: membeli bahan lalu menjual masakan dihitung sebagai dua aksi dengan dua jenis kegiatan. Membeli bahan dua kali juga dihitung sebagai dua aksi, tetapi hanya satu jenis kegiatan. Kegiatan di luar jatah aksi tidak masuk tabel.", "For example, buying ingredients then selling a meal counts as two actions with two activity types. Buying ingredients twice also counts as two actions, but only one activity type. Activities outside the action allowance are excluded from this table.");
        terms["players.metric.happiness_source_diversity_percent"] = ("Pemerataan Sumber Poin Kebahagiaan", "Happiness Point Source Balance");
        terms["players.support.meaning.happiness_diversity"] = ("{0} mengukur seberapa merata poin kebahagiaan positif tersebar pada sumber yang tersedia dalam mode permainan. Penalti dikecualikan.", "{0} measures how evenly positive Happiness points are spread across the sources available in this game mode. Penalties are excluded.");
        terms["players.support.guide.happiness_diverse"] = ("67–100%: poin kebahagiaan positif tersebar cukup merata. Semakin dekat ke 100%, semakin seimbang kontribusi tiap sumber.", "67–100%: positive Happiness points are fairly evenly spread. Closer to 100% means more balanced contributions.");
        terms["players.support.guide.happiness_mixed"] = ("34–67%: poin kebahagiaan positif berasal dari beberapa sumber, tetapi kontribusinya belum merata.", "34–67%: positive Happiness points come from several sources, but their contributions are uneven.");
        terms["players.support.guide.happiness_concentrated"] = ("Di bawah 34%: poin kebahagiaan positif terkonsentrasi pada sedikit sumber. 0% berarti hanya satu sumber yang menyumbang poin kebahagiaan.", "Below 34%: positive Happiness points are concentrated in a few sources. 0% means only one source contributes Happiness points.");
        terms["players.support.recommendation.happiness_diversity"] = ("Bandingkan kontribusi sumber poin kebahagiaan dan pertimbangkan sumber yang belum dimanfaatkan sesuai strategi pemain.", "Compare Happiness point-source contributions and consider unused sources that suit the player's strategy.");
        // Memperbarui `terms[”players.support.meaning.ingredients_collected”]` menggunakan tuple yang membawa bagian 1: ”Jumlah seluruh kartu bahan yang
        // diperoleh, termasuk bahan awal dan pembelian se...; bagian 2: ”Total ingredient cards obtained, including starting ingredients and purchases d...
        // dalam AddPlayers.
        terms["players.support.meaning.ingredients_collected"] = ("Jumlah seluruh kartu bahan yang diperoleh, termasuk bahan awal dan pembelian selama permainan.", "Total ingredient cards obtained, including starting ingredients and purchases during gameplay.");
        // Memperbarui `terms[”players.support.meaning.ingredients_used_total”]` menggunakan tuple yang membawa bagian 1: ”Jumlah kartu bahan yang
        // benar-benar diserahkan untuk menyelesaikan seluruh pesa...; bagian 2: ”Ingredient cards actually submitted to complete all meal orders.” dalam
        // AddPlayers.
        terms["players.support.meaning.ingredients_used_total"] = ("Jumlah kartu bahan yang benar-benar diserahkan untuk menyelesaikan seluruh pesanan makanan.", "Ingredient cards actually submitted to complete all meal orders.");
        // Memperbarui `terms[”players.support.meaning.ingredients_used_average”]` menggunakan tuple yang membawa bagian 1: ”Total Bahan Digunakan dibagi
        // jumlah Pesanan Selesai.”; bagian 2: ”Total Ingredients Used divided by Completed Orders.” dalam AddPlayers.
        terms["players.support.meaning.ingredients_used_average"] = ("Total Bahan Digunakan dibagi jumlah Pesanan Selesai.", "Total Ingredients Used divided by Completed Orders.");
        // Memperbarui `terms[”players.support.meaning.ingredient_purchase_cost”]` menggunakan tuple yang membawa bagian 1: ”Jumlah koin yang dibayar untuk
        // seluruh pembelian bahan, termasuk bahan yang mas...; bagian 2: ”Coins paid for all ingredient purchases, including ingredients still
        // remaining.... dalam AddPlayers.
        terms["players.support.meaning.ingredient_purchase_cost"] = ("Jumlah koin yang dibayar untuk seluruh pembelian bahan, termasuk bahan yang masih tersisa.", "Coins paid for all ingredient purchases, including ingredients still remaining.");
        // Memperbarui `terms[”players.support.meaning.orders_completed”]` menggunakan tuple yang membawa bagian 1: ”Jumlah pesanan makanan yang berhasil
        // diselesaikan dengan menyerahkan bahan yang...; bagian 2: ”Meal orders completed by submitting the required ingredients.” dalam AddPlayers.
        terms["players.support.meaning.orders_completed"] = ("Jumlah pesanan makanan yang berhasil diselesaikan dengan menyerahkan bahan yang diminta.", "Meal orders completed by submitting the required ingredients.");
        // Memperbarui `terms[”players.support.meaning.order_income_total”]` menggunakan tuple yang membawa bagian 1: ”Jumlah seluruh koin yang diterima
        // dari Pesanan Selesai, sebelum dikurangi biaya...; bagian 2: ”Total coins received from Completed Orders before ingredient costs are deducted...
        // dalam AddPlayers.
        terms["players.support.meaning.order_income_total"] = ("Jumlah seluruh koin yang diterima dari Pesanan Selesai, sebelum dikurangi biaya bahan.", "Total coins received from Completed Orders before ingredient costs are deducted.");
        // Memperbarui `terms[”players.support.meaning.gold_initial”]` menggunakan tuple yang membawa bagian 1: ”Kartu emas yang diterima saat persiapan;
        // tidak termasuk pembelian selama permai...; bagian 2: ”Gold cards received during setup; gameplay purchases are excluded.” dalam AddPlayers.
        terms["players.support.meaning.gold_initial"] = ("Kartu emas yang diterima saat persiapan; tidak termasuk pembelian selama permainan.", "Gold cards received during setup; gameplay purchases are excluded.");
        // Memperbarui `terms[”players.support.meaning.gold_sold”]` menggunakan tuple yang membawa bagian 1: ”Jumlah kartu emas yang dijual selama
        // permainan.”; bagian 2: ”Gold cards sold during gameplay.” dalam AddPlayers.
        terms["players.support.meaning.gold_sold"] = ("Jumlah kartu emas yang dijual selama permainan.", "Gold cards sold during gameplay.");
        // Memperbarui `terms[”players.support.meaning.gold_purchase_prices”]` menggunakan tuple yang membawa bagian 1: ”Setiap baris menunjukkan urutan
        // pembelian emas dan harga yang dibayar pada pemb...; bagian 2: ”Each row shows a gold purchase in order and the price paid for it.” dalam
        // AddPlayers.
        terms["players.support.meaning.gold_purchase_prices"] = ("Setiap baris menunjukkan harga satu kartu emas pada transaksi pembelian. Total pembayaran = harga per kartu × jumlah kartu yang dibeli.", "Each row shows the price of one gold card in a purchase. Total payment = price per card × number of cards purchased.");
        // Memperbarui `terms[”players.support.meaning.gold_sale_prices”]` menggunakan tuple yang membawa bagian 1: ”Setiap baris menunjukkan urutan
        // penjualan emas dan koin yang diterima pada penj...; bagian 2: ”Each row shows a gold sale in order and the coins received from it.” dalam
        // AddPlayers.
        terms["players.support.meaning.gold_sale_prices"] = ("Setiap baris menunjukkan harga satu kartu emas pada transaksi penjualan. Total koin diterima = harga per kartu × jumlah kartu yang dijual.", "Each row shows the price of one gold card in a sale. Total coins received = price per card × number of cards sold.");
        // Memperbarui `terms[”players.support.meaning.gold_purchase_cost”]` menggunakan tuple yang membawa bagian 1: ”Jumlah koin yang dibayar untuk
        // seluruh kartu emas yang dibeli selama permainan....; bagian 2: ”Coins paid for all gold cards bought during gameplay.” dalam AddPlayers.
        terms["players.support.meaning.gold_purchase_cost"] = ("Jumlah koin yang dibayar untuk seluruh kartu emas yang dibeli selama permainan.", "Coins paid for all gold cards bought during gameplay.");
        // Memperbarui `terms[”players.support.meaning.gold_sale_income”]` menggunakan tuple yang membawa bagian 1: ”Jumlah koin yang diterima dari seluruh
        // penjualan kartu emas.”; bagian 2: ”Coins received from all gold-card sales.” dalam AddPlayers.
        terms["players.support.meaning.gold_sale_income"] = ("Jumlah koin yang diterima dari seluruh penjualan kartu emas.", "Coins received from all gold-card sales.");
        // Memperbarui `terms[”players.support.meaning.pension_total”]` menggunakan tuple yang membawa bagian 1: ”Total Dana Pensiun = Koin Tersisa + Nilai
        // Kartu Bahan Tersisa + Koin dalam Tabu...; bagian 2: ”Total Pension Fund = Remaining Coins + Remaining Ingredient Card Value + Coins ... dalam
        // AddPlayers.
        terms["players.support.meaning.pension_total"] = ("Total Dana Pensiun = Koin Tersisa + Nilai Kartu Bahan Tersisa + Koin dalam Tabungan.", "Total Pension Fund = Remaining Coins + Remaining Ingredient Card Value + Coins in Savings.");
        // Memperbarui `terms[”players.support.meaning.pension_rank”]` menggunakan tuple yang membawa bagian 1: ”Posisi dana pensiun pemain dibanding pemain
        // lain; peringkat ke-1 adalah terting...; bagian 2: ”The player's pension-fund position compared with other players; rank 1 is highe... dalam
        // AddPlayers.
        terms["players.support.meaning.pension_rank"] = ("Posisi dana pensiun pemain dibanding pemain lain; peringkat ke-1 adalah tertinggi.", "The player's pension-fund position compared with other players; rank 1 is highest.");
        // Memperbarui `terms[”players.support.meaning.pension_happiness”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan yang diberikan
        // berdasarkan Peringkat Dana Pensiun sesuai set a...; bagian 2: ”Happiness Points awarded from the Pension Fund Rank according to the ruleset.”
        // dalam AddPlayers.
        terms["players.support.meaning.pension_happiness"] = ("Poin Kebahagiaan yang diberikan berdasarkan Peringkat Dana Pensiun sesuai set aturan.", "Happiness Points awarded from the Pension Fund Rank according to the ruleset.");
        // Memperbarui `terms[”players.support.meaning.risk_cards_drawn”]` menggunakan tuple yang membawa bagian 1: ”Jumlah kartu Risiko Kehidupan yang
        // muncul setelah pemain menyelesaikan pesanan ...; bagian 2: ”Life Risk Cards drawn after the player completed orders in Advanced mode.” dalam
        // AddPlayers.
        terms["players.support.meaning.risk_cards_drawn"] = ("Jumlah kartu Risiko Kehidupan yang muncul setelah pemain menyelesaikan pesanan pada mode Mahir.", "Life Risk Cards drawn after the player completed orders in Advanced mode.");
        // Memperbarui `terms[”players.support.meaning.risk_insured”]` menggunakan tuple yang membawa bagian 1: ”Jumlah kejadian risiko yang dampaknya
        // ditanggung oleh asuransi aktif.”; bagian 2: ”Risk events whose effects were covered by active insurance.” dalam AddPlayers.
        terms["players.support.meaning.risk_insured"] = ("Jumlah kejadian risiko yang dampaknya ditanggung oleh asuransi aktif.", "Risk events whose effects were covered by active insurance.");
        // Memperbarui `terms[”players.support.meaning.insurance_premium”]` menggunakan tuple yang membawa bagian 1: ”Jumlah koin premi yang benar-benar
        // dibayar untuk mengaktifkan asuransi.”; bagian 2: ”Premium coins actually paid to activate insurance.” dalam AddPlayers.
        terms["players.support.meaning.insurance_premium"] = ("Jumlah koin premi yang dibayar untuk mengaktifkan kembali asuransi. Saat persiapan, pemain mendapat perlindungan gratis satu kali; karena itu klaim asuransi dapat terjadi meskipun premi yang dibayar nol.", "Premium coins paid to reactivate insurance. Players receive one free coverage during setup, so an insurance claim can occur even when paid premiums are zero.");
        // Memperbarui `terms[”players.support.meaning.donation_rank”]` menggunakan tuple yang membawa bagian 1: ”{0} menampilkan posisi pemain pada setiap
        // Jumat berdasarkan jumlah donasi; nomi...; bagian 2: ”{0} shows the player's position each Friday by donation amount; ties use the hi... dalam
        // AddPlayers.
        terms["players.support.meaning.donation_rank"] = ("{0} menampilkan posisi pemain pada setiap Jumat berdasarkan jumlah donasi; nominal sama diurutkan dengan angka Tie Breaker terbesar.", "{0} shows the player's position each Friday by donation amount; ties use the highest Tie Breaker number.");
        // Memperbarui `terms[”players.support.meaning.donation_history”]` menggunakan tuple yang membawa bagian 1: ”{0} menampilkan jumlah koin yang
        // didonasikan dan peringkat pemain pada hari yan...; bagian 2: ”{0} shows coins donated and the player's rank on the same day. Ranks follow the...
        // dalam AddPlayers.
        terms["players.support.meaning.donation_history"] = ("{0} menampilkan jumlah koin yang didonasikan dan peringkat pemain pada hari yang sama. Peringkat diurutkan dari jumlah donasi terbesar; jika sama, angka Tie Breaker terbesar didahulukan. Tanda pisah berarti data belum tersedia.", "{0} shows coins donated and the player's rank on the same day. Ranks follow the highest donation amount; ties use the highest Tie Breaker number. A dash means data is not available.");
        // Memperbarui `terms[”players.support.meaning.donation_happiness”]` menggunakan tuple yang membawa bagian 1: ”{0} adalah Poin Kebahagiaan yang
        // diperoleh dari kartu juara donasi, bukan jumla...; bagian 2: ”{0} is Happiness Points earned from donation champion cards, not the number of ...
        // dalam AddPlayers.
        terms["players.support.meaning.donation_happiness"] = ("{0} adalah Poin Kebahagiaan yang diperoleh dari kartu juara donasi, bukan jumlah koin yang didonasikan.", "{0} is Happiness Points earned from donation champion cards, not the number of coins donated.");
        // Memperbarui `terms[”players.support.guide.ingredient_use_high”]` menggunakan tuple yang membawa bagian 1: ”80% atau lebih: sebagian besar bahan
        // yang diperoleh sudah digunakan pada pesana...; bagian 2: ”80% or more: most collected ingredients were used in completed orders.” dalam
        // AddPlayers.
        terms["players.support.guide.ingredient_use_high"] = ("80% atau lebih: sebagian besar bahan yang diperoleh sudah digunakan pada pesanan selesai.", "80% or more: most collected ingredients were used in completed orders.");
        // Memperbarui `terms[”players.support.guide.ingredient_use_low”]` menggunakan tuple yang membawa bagian 1: ”Di bawah 60%: sebagian besar bahan yang
        // diperoleh belum digunakan pada pesanan ...; bagian 2: ”Below 60%: most collected ingredients were not used in completed orders. Check ... dalam
        // AddPlayers.
        terms["players.support.guide.ingredient_use_low"] = ("Di bawah 60%: sebagian besar bahan yang diperoleh belum digunakan pada pesanan selesai. Periksa bahan tersisa dan terbuang.", "Below 60%: most collected ingredients were not used in completed orders. Check remaining and wasted ingredients.");
        // Memperbarui `terms[”players.support.guide.ingredient_use_moderate”]` menggunakan tuple yang membawa bagian 1: ”60–80%: lebih dari separuh bahan
        // yang diperoleh sudah digunakan pada pesanan se...; bagian 2: ”60–80%: over half of collected ingredients were used in completed orders.” dalam
        // AddPlayers.
        terms["players.support.guide.ingredient_use_moderate"] = ("60–80%: lebih dari separuh bahan yang diperoleh sudah digunakan pada pesanan selesai.", "60–80%: over half of collected ingredients were used in completed orders.");
    // Menutup scope metode AddPlayers; bagian berikut berada di luar batas blok tersebut dalam AddPlayers.
    }
// Menutup scope tipe UiTextLexicon; bagian berikut berada di luar batas blok tersebut.
}
