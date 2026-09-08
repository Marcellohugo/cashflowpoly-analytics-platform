// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui UiTextLexicon.Sessions.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `UiTextLexicon`.
internal static partial class UiTextLexicon
// Membuka scope tipe UiTextLexicon; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `AddSessions` dengan hasil bertipe `void`; operasi ini menangani add sessions. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddSessions(Dictionary<string, (string Id, string En)> terms)
    // Membuka scope metode AddSessions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddSessions.
    {
        // Memperbarui `terms[”sessions.management”]` menggunakan tuple yang membawa bagian 1: ”Analitika Sesi”; bagian 2: ”Session Analytics” dalam
        // AddSessions.
        terms["sessions.management"] = ("Analitika Sesi", "Session Analytics");
        // Memperbarui `terms[”sessions.title”]` menggunakan tuple yang membawa bagian 1: ”Daftar Sesi Permainan”; bagian 2: ”Game Session List” dalam
        // AddSessions.
        terms["sessions.title"] = ("Daftar Sesi Permainan", "Game Session List");
        // Memperbarui `terms[”sessions.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Pantau status sesi dan buka rincian analitika tiap sesi.”;
        // bagian 2: ”Monitor session status and open analytics details for each session.” dalam AddSessions.
        terms["sessions.subtitle"] = ("Pantau status sesi dan buka rincian analitika tiap sesi.", "Monitor session status and open analytics details for each session.");
        // Memperbarui `terms[”sessions.player_management”]` menggunakan tuple yang membawa bagian 1: ”Sesi Pemain”; bagian 2: ”Player Sessions” dalam
        // AddSessions.
        terms["sessions.player_management"] = ("Sesi Pemain", "Player Sessions");
        // Memperbarui `terms[”sessions.player_title”]` menggunakan tuple yang membawa bagian 1: ”Daftar sesi saya”; bagian 2: ”My session list” dalam
        // AddSessions.
        terms["sessions.player_title"] = ("Daftar sesi saya", "My session list");
        // Memperbarui `terms[”sessions.player_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Lihat sesi yang Anda ikuti dan buka analitika progres
        // permainan Anda.”; bagian 2: ”View sessions you joined and open your gameplay progress analytics.” dalam AddSessions.
        terms["sessions.player_subtitle"] = ("Lihat sesi yang Anda ikuti dan buka analitika progres permainan Anda.", "View sessions you joined and open your gameplay progress analytics.");
        // Memperbarui `terms[”sessions.readonly”]` menggunakan tuple yang membawa bagian 1: ”Hanya Baca”; bagian 2: ”Read-only” dalam AddSessions.
        terms["sessions.readonly"] = ("Hanya Baca", "Read-only");
        // Memperbarui `terms[”sessions.total_sessions”]` menggunakan tuple yang membawa bagian 1: ”Total Sesi”; bagian 2: ”Total Sessions” dalam
        // AddSessions.
        terms["sessions.total_sessions"] = ("Total Sesi", "Total Sessions");
        // Memperbarui `terms[”sessions.my_sessions”]` menggunakan tuple yang membawa bagian 1: ”Sesi saya”; bagian 2: ”My sessions” dalam AddSessions.
        terms["sessions.my_sessions"] = ("Sesi saya", "My sessions");
        // Memperbarui `terms[”sessions.started”]` menggunakan tuple yang membawa bagian 1: ”Sedang Berjalan”; bagian 2: ”In Progress” dalam AddSessions.
        terms["sessions.started"] = ("Sedang Berjalan", "In Progress");
        // Memperbarui `terms[”sessions.created”]` menggunakan tuple yang membawa bagian 1: ”Siap Dimulai”; bagian 2: ”Ready to Start” dalam AddSessions.
        terms["sessions.created"] = ("Siap Dimulai", "Ready to Start");
        // Memperbarui `terms[”sessions.ended”]` menggunakan tuple yang membawa bagian 1: ”Sesi Selesai”; bagian 2: ”Completed Sessions” dalam AddSessions.
        terms["sessions.ended"] = ("Sesi Selesai", "Completed Sessions");
        // Memperbarui `terms[”sessions.index.summary_title”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan status sesi”; bagian 2: ”Session status
        // summary” dalam AddSessions.
        terms["sessions.index.summary_title"] = ("Ringkasan status sesi", "Session status summary");
        // Memperbarui `terms[”sessions.index.summary_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Pantau distribusi status untuk melihat sesi yang
        // sedang berjalan dan sesi yang ...; bagian 2: ”Monitor status distribution to see which sessions are in progress and which hav... dalam
        // AddSessions.
        terms["sessions.index.summary_subtitle"] = ("Pantau distribusi status untuk melihat sesi yang sedang berjalan dan sesi yang sudah selesai.", "Monitor status distribution to see which sessions are in progress and which have finished.");
        // Memperbarui `terms[”sessions.index.list_title.instructor”]` menggunakan tuple yang membawa bagian 1: ”Daftar sesi yang Anda kelola”; bagian 2:
        // ”Sessions you manage” dalam AddSessions.
        terms["sessions.index.list_title.instructor"] = ("Daftar sesi yang Anda kelola", "Sessions you manage");
        // Memperbarui `terms[”sessions.index.list_subtitle.instructor”]` menggunakan tuple yang membawa bagian 1: ”Pilih sesi untuk membuka rincian
        // analitika, performa pemain, dan linimasa event...; bagian 2: ”Select a session to open analytics details, player performance, and the event t...
        // dalam AddSessions.
        terms["sessions.index.list_subtitle.instructor"] = ("Pilih sesi untuk membuka rincian analitika, performa pemain, dan linimasa event.", "Select a session to open analytics details, player performance, and the event timeline.");
        // Memperbarui `terms[”sessions.index.list_title.player”]` menggunakan tuple yang membawa bagian 1: ”Daftar sesi yang Anda ikuti”; bagian 2:
        // ”Sessions you joined” dalam AddSessions.
        terms["sessions.index.list_title.player"] = ("Daftar sesi yang Anda ikuti", "Sessions you joined");
        // Memperbarui `terms[”sessions.index.list_subtitle.player”]` menggunakan tuple yang membawa bagian 1: ”Pilih sesi untuk melihat ringkasan performa
        // dan progres pribadi Anda.”; bagian 2: ”Select a session to view your performance summary and personal progress.” dalam AddSessions.
        terms["sessions.index.list_subtitle.player"] = ("Pilih sesi untuk melihat ringkasan performa dan progres pribadi Anda.", "Select a session to view your performance summary and personal progress.");
        // Memperbarui `terms[”sessions.tips.instructor.title”]` menggunakan tuple yang membawa bagian 1: ”Tips penggunaan menu Sesi”; bagian 2: ”Sessions
        // menu tips” dalam AddSessions.
        terms["sessions.tips.instructor.title"] = ("Tips penggunaan menu Sesi", "Sessions menu tips");
        // Memperbarui `terms[”sessions.tips.player.title”]` menggunakan tuple yang membawa bagian 1: ”Tips membaca sesi untuk pemain”; bagian 2: ”Player
        // session tips” dalam AddSessions.
        terms["sessions.tips.player.title"] = ("Tips membaca sesi untuk pemain", "Player session tips");
        // Memperbarui `terms[”sessions.step”]` menggunakan tuple yang membawa bagian 1: ”Langkah”; bagian 2: ”Step” dalam AddSessions.
        terms["sessions.step"] = ("Langkah", "Step");
        // Memperbarui `terms[”sessions.note”]` menggunakan tuple yang membawa bagian 1: ”Catatan”; bagian 2: ”Note” dalam AddSessions.
        terms["sessions.note"] = ("Catatan", "Note");
        // Memperbarui `terms[”sessions.tips.instructor.step1”]` menggunakan tuple yang membawa bagian 1: ”Gunakan status sesi untuk membedakan sesi yang
        // siap dimulai, sedang berjalan, d...; bagian 2: ”Use session status to distinguish sessions that are ready, in progress, or comp... dalam
        // AddSessions.
        terms["sessions.tips.instructor.step1"] = ("Gunakan status sesi untuk membedakan sesi yang siap dimulai, sedang berjalan, dan sudah selesai.", "Use session status to distinguish sessions that are ready, in progress, or completed.");
        // Memperbarui `terms[”sessions.tips.instructor.step2”]` menggunakan tuple yang membawa bagian 1: ”Buka Analitika Sesi untuk memeriksa Set Aturan
        // aktif, hasil umum, dan urutan ak...; bagian 2: ”Open Session Analytics to review the active ruleset, overall results, and activ... dalam
        // AddSessions.
        terms["sessions.tips.instructor.step2"] = ("Buka Analitika Sesi untuk memeriksa Set Aturan aktif, hasil umum, dan urutan aktivitas pada sesi tersebut.", "Open Session Analytics to review the active ruleset, overall results, and activity order for that session.");
        // Memperbarui `terms[”sessions.tips.instructor.step3”]` menggunakan tuple yang membawa bagian 1: ”Pilih Lihat Analitika pada tabel pemain saat
        // suatu hasil perlu ditelusuri sampa...; bagian 2: ”Select View Analytics in the player table when a result needs to be traced to i... dalam
        // AddSessions.
        terms["sessions.tips.instructor.step3"] = ("Pilih Lihat Analitika pada tabel pemain saat suatu hasil perlu ditelusuri sampai ke keputusan individunya.", "Select View Analytics in the player table when a result needs to be traced to individual decisions.");
        // Memperbarui `terms[”sessions.tips.player.step1”]` menggunakan tuple yang membawa bagian 1: ”Pilih sesi berdasarkan nama dan status agar hasil
        // yang dibandingkan berasal dar...; bagian 2: ”Choose a session by name and status so the compared results come from the corre... dalam
        // AddSessions.
        terms["sessions.tips.player.step1"] = ("Pilih sesi berdasarkan nama dan status agar hasil yang dibandingkan berasal dari permainan yang tepat.", "Choose a session by name and status so the compared results come from the correct game.");
        // Memperbarui `terms[”sessions.tips.player.step2”]` menggunakan tuple yang membawa bagian 1: ”Buka Analitika Sesi untuk melihat poin kebahagiaan,
        // arus kas, posisi permainan,...; bagian 2: ”Open Session Analytics to review your scores, cashflow, game position, and acti... dalam AddSessions.
        terms["sessions.tips.player.step2"] = ("Buka Analitika Sesi untuk melihat poin kebahagiaan, arus kas, posisi permainan, dan aktivitas Anda.", "Open Session Analytics to review your scores, cashflow, game position, and activity.");
        // Memperbarui `terms[”sessions.tips.player.step3”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan sesi yang sudah selesai untuk melihat
        // keputusan mana yang memperkuat...; bagian 2: ”Compare completed sessions to see which decisions strengthened or weakened the ... dalam
        // AddSessions.
        terms["sessions.tips.player.step3"] = ("Bandingkan sesi yang sudah selesai untuk melihat keputusan mana yang memperkuat atau melemahkan hasil akhir.", "Compare completed sessions to see which decisions strengthened or weakened the final result.");
        // Memperbarui `terms[”sessions.view_progress”]` menggunakan tuple yang membawa bagian 1: ”Lihat Rincian Sesi”; bagian 2: ”View Session Details”
        // dalam AddSessions.
        terms["sessions.view_progress"] = ("Lihat Rincian Sesi", "View Session Details");
        // Memperbarui `terms[”sessions.view_analytics”]` menggunakan tuple yang membawa bagian 1: ”Lihat Analitika”; bagian 2: ”View Analytics” dalam
        // AddSessions.
        terms["sessions.view_analytics"] = ("Lihat Analitika", "View Analytics");
        // Memperbarui `terms[”sessions.view_ruleset”]` menggunakan tuple yang membawa bagian 1: ”Lihat Ruleset”; bagian 2: ”View Ruleset” dalam
        // AddSessions.
        terms["sessions.view_ruleset"] = ("Lihat Ruleset", "View Ruleset");
        // Memperbarui `terms[”sessions.back_to_list”]` menggunakan tuple yang membawa bagian 1: ”Kembali ke daftar sesi”; bagian 2: ”Back to session list”
        // dalam AddSessions.
        terms["sessions.back_to_list"] = ("Kembali ke daftar sesi", "Back to session list");
        // Memperbarui `terms[”sessions.live_log”]` menggunakan tuple yang membawa bagian 1: ”Log Sesi Real-time”; bagian 2: ”Real-time Session Log” dalam
        // AddSessions.
        terms["sessions.live_log"] = ("Log Sesi Real-time", "Real-time Session Log");
        // Memperbarui `terms[”sessions.detail_title”]` menggunakan tuple yang membawa bagian 1: ”Rincian dan Analitika Sesi”; bagian 2: ”Session Details
        // and Analytics” dalam AddSessions.
        terms["sessions.detail_title"] = ("Rincian dan Analitika Sesi", "Session Details and Analytics");
        // Memperbarui `terms[”sessions.detail.ruleset_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Bagian ini menunjukkan aturan yang sedang
        // dipakai sebagai dasar validasi dan sk...; bagian 2: ”This section shows the ruleset currently used as the basis for session validati... dalam
        // AddSessions.
        terms["sessions.detail.ruleset_subtitle"] = ("Bagian ini menunjukkan aturan yang sedang dipakai sebagai dasar validasi dan skor sesi.", "This section shows the ruleset currently used as the basis for session validation and scoring.");
        // Memperbarui `terms[”sessions.detail.summary_title”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Kinerja Sesi”; bagian 2: ”Session
        // Performance Summary” dalam AddSessions.
        terms["sessions.detail.summary_title"] = ("Ringkasan Kinerja Sesi", "Session Performance Summary");
        // Memperbarui `terms[”sessions.detail.summary_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan ini membantu Anda membaca kondisi
        // sesi sebelum masuk ke level pemain....; bagian 2: ”This summary helps you understand session conditions before moving to player-le... dalam
        // AddSessions.
        terms["sessions.detail.summary_subtitle"] = ("Ringkasan ini membantu Anda membaca kondisi sesi sebelum masuk ke level pemain.", "This summary helps you understand session conditions before moving to player-level analysis.");
        // Memperbarui `terms[”sessions.detail.player_summary_title”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Hasil Saya”; bagian 2: ”My
        // Results Summary” dalam AddSessions.
        terms["sessions.detail.player_summary_title"] = ("Ringkasan Hasil Saya", "My Results Summary");
        // Memperbarui `terms[”sessions.detail.player_summary_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Seluruh angka di bawah berasal dari
        // hasil Anda sendiri, bukan gabungan seluruh ...; bagian 2: ”Every number below comes from your own results, not the combined session totals...
        // dalam AddSessions.
        terms["sessions.detail.player_summary_subtitle"] = ("Seluruh angka di bawah berasal dari hasil Anda sendiri, bukan gabungan seluruh pemain.", "Every number below comes from your own results, not the combined session totals.");
        // Memperbarui `terms[”sessions.detail.players_title”]` menggunakan tuple yang membawa bagian 1: ”Daftar Pemain pada Sesi”; bagian 2: ”Players in
        // This Session” dalam AddSessions.
        terms["sessions.detail.players_title"] = ("Daftar Pemain pada Sesi", "Players in This Session");
        // Memperbarui `terms[”sessions.detail.players_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan delapan komponen dan total Poin
        // Kebahagiaan setiap pemain dalam satu...; bagian 2: ”Compare all eight components and each player's total Happiness Score in one tab... dalam
        // AddSessions.
        terms["sessions.detail.players_subtitle"] = ("Bandingkan delapan komponen dan total Poin Kebahagiaan setiap pemain dalam satu tabel.", "Compare all eight components and each player's total Happiness Score in one table.");
        // Memperbarui `terms[”sessions.detail.player_scores_title”]` menggunakan tuple yang membawa bagian 1: ”Pembentuk Poin Kebahagiaan Saya”; bagian 2:
        // ”My Happiness Score Components” dalam AddSessions.
        terms["sessions.detail.player_scores_title"] = ("Pembentuk Poin Kebahagiaan Saya", "My Happiness Score Components");
        // Memperbarui `terms[”sessions.detail.player_scores_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Lihat komponen yang menambah atau
        // mengurangi Poin Kebahagiaan Anda pada sesi in...; bagian 2: ”See which components added to or reduced your Happiness Score in this session.”
        // dalam AddSessions.
        terms["sessions.detail.player_scores_subtitle"] = ("Lihat komponen yang menambah atau mengurangi Poin Kebahagiaan Anda pada sesi ini.", "See which components added to or reduced your Happiness Score in this session.");
        // Memperbarui `terms[”sessions.score.elements_title”]` menggunakan tuple yang membawa bagian 1: ”8 Elemen Poin Kebahagiaan”; bagian 2: ”8 Happiness
        // Score Components” dalam AddSessions.
        terms["sessions.score.elements_title"] = ("8 Elemen Poin Kebahagiaan", "8 Happiness Score Components");
        // Memperbarui `terms[”sessions.score.need_points”]` menggunakan tuple yang membawa bagian 1: ”Total Poin Kebahagiaan Kartu Aneka Kebutuhan”; bagian
        // 2: ”Total Needs Card Happiness Points” dalam AddSessions.
        terms["sessions.score.need_points"] = ("Total Poin Kebahagiaan Kartu Aneka Kebutuhan", "Total Needs Card Happiness Points");
        // Memperbarui `terms[”sessions.score.need_bonus”]` menggunakan tuple yang membawa bagian 1: ”Bonus Set Kartu Aneka Kebutuhan”; bagian 2: ”Needs
        // Card Set Bonus” dalam AddSessions.
        terms["sessions.score.need_bonus"] = ("Bonus Set Kartu Aneka Kebutuhan", "Needs Card Set Bonus");
        // Memperbarui `terms[”sessions.score.mission_penalty”]` menggunakan tuple yang membawa bagian 1: ”Pengurang karena Misi Koleksi Gagal”; bagian 2:
        // ”Deduction for Failed Collection Mission” dalam AddSessions.
        terms["sessions.score.mission_penalty"] = ("Pengurang karena Misi Koleksi Gagal", "Deduction for Failed Collection Mission");
        // Memperbarui `terms[”sessions.score.donation_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Kartu Juara Peduli Donasi”;
        // bagian 2: ”Donation Champion Card Happiness Points” dalam AddSessions.
        terms["sessions.score.donation_points"] = ("Poin Kebahagiaan Kartu Juara Peduli Donasi", "Donation Champion Card Happiness Points");
        // Memperbarui `terms[”sessions.score.pension_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Kartu Juara Dana Pensiun”; bagian
        // 2: ”Pension Champion Card Happiness Points” dalam AddSessions.
        terms["sessions.score.pension_points"] = ("Poin Kebahagiaan Kartu Juara Dana Pensiun", "Pension Champion Card Happiness Points");
        // Memperbarui `terms[”sessions.score.financial_goal”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Kartu Tujuan Finansial”; bagian
        // 2: ”Financial Goal Card Happiness Points” dalam AddSessions.
        terms["sessions.score.financial_goal"] = ("Poin Kebahagiaan Kartu Tujuan Finansial", "Financial Goal Card Happiness Points");
        // Memperbarui `terms[”sessions.score.loan_penalty”]` menggunakan tuple yang membawa bagian 1: ”Pengurang karena Pinjaman Syariah Belum Lunas”;
        // bagian 2: ”Deduction for Unpaid Sharia Loan” dalam AddSessions.
        terms["sessions.score.loan_penalty"] = ("Pengurang karena Pinjaman Syariah Belum Lunas", "Deduction for Unpaid Sharia Loan");
        // Memperbarui `terms[”sessions.score.gold_points”]` menggunakan tuple yang membawa bagian 1: ”Poin Kebahagiaan Investasi Emas”; bagian 2: ”Gold
        // Investment Happiness Points” dalam AddSessions.
        terms["sessions.score.gold_points"] = ("Poin Kebahagiaan Investasi Emas", "Gold Investment Happiness Points");
        // Memperbarui `terms[”sessions.score.total”]` menggunakan tuple yang membawa bagian 1: ”Total Poin Kebahagiaan”; bagian 2: ”Total Happiness Score”
        // dalam AddSessions.
        terms["sessions.score.total"] = ("Total Poin Kebahagiaan", "Total Happiness Score");
        // Memperbarui `terms[”sessions.detail.empty_analytics”]` menggunakan tuple yang membawa bagian 1: ”Analitika sesi belum tersedia. Pastikan
        // aktivitas permainan sudah terkirim, lal...; bagian 2: ”Session analytics are not available yet. Ensure gameplay events have been submi... dalam
        // AddSessions.
        terms["sessions.detail.empty_analytics"] = ("Analitika sesi belum tersedia. Pastikan aktivitas permainan sudah terkirim, lalu muat ulang halaman ini.", "Session analytics are not available yet. Ensure gameplay events have been submitted, then refresh this page.");
        // Memperbarui `terms[”sessions.analytics_title”]` menggunakan tuple yang membawa bagian 1: ”Analitika Sesi”; bagian 2: ”Session Analytics” dalam
        // AddSessions.
        terms["sessions.analytics_title"] = ("Analitika Sesi", "Session Analytics");
        // Memperbarui `terms[”sessions.winner.title”]` menggunakan tuple yang membawa bagian 1: ”Pemenang Sesi”; bagian 2: ”Session Winner” dalam
        // AddSessions.
        terms["sessions.winner.title"] = ("Pemenang Sesi", "Session Winner");
        // Memperbarui `terms[”sessions.winner.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Performa terbaik pada sesi ini”; bagian 2: ”Best
        // performance in this session” dalam AddSessions.
        terms["sessions.winner.subtitle"] = ("Performa terbaik pada sesi ini", "Best performance in this session");
        // Memperbarui `terms[”sessions.winner.net”]` menggunakan tuple yang membawa bagian 1: ”Bersih Arus Kas”; bagian 2: ”Net cashflow” dalam
        // AddSessions.
        terms["sessions.winner.net"] = ("Bersih Arus Kas", "Net cashflow");
        // Memperbarui `terms[”sessions.winner.happiness”]` menggunakan tuple yang membawa bagian 1: ”Kebahagiaan”; bagian 2: ”Happiness” dalam AddSessions.
        terms["sessions.winner.happiness"] = ("Kebahagiaan", "Happiness");
        // Memperbarui `terms[”sessions.winner.cash_in”]` menggunakan tuple yang membawa bagian 1: ”Kas Masuk”; bagian 2: ”Cash in” dalam AddSessions.
        terms["sessions.winner.cash_in"] = ("Kas Masuk", "Cash in");
        // Memperbarui `terms[”sessions.active_ruleset.title”]` menggunakan tuple yang membawa bagian 1: ”Set Aturan Aktif Sesi”; bagian 2: ”Session Active
        // Ruleset” dalam AddSessions.
        terms["sessions.active_ruleset.title"] = ("Set Aturan Aktif Sesi", "Session Active Ruleset");
        // Memperbarui `terms[”sessions.active_ruleset.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Set Aturan ini menjadi acuan validasi aturan
        // dan penilaian pada sesi ini.”; bagian 2: ”This ruleset is the source of rule validation and scoring in this session.” dalam AddSessions.
        terms["sessions.active_ruleset.subtitle"] = ("Set Aturan ini menjadi acuan validasi aturan dan penilaian pada sesi ini.", "This ruleset is the source of rule validation and scoring in this session.");
        // Memperbarui `terms[”sessions.active_ruleset.empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada Set Aturan aktif pada sesi ini.
        // Instruktur dapat memilih ruleset akti...; bagian 2: ”No active ruleset for this session yet. Instructors can choose the active rules... dalam
        // AddSessions.
        terms["sessions.active_ruleset.empty"] = ("Belum ada Set Aturan aktif pada sesi ini. Instruktur dapat memilih ruleset aktif dari halaman ini.", "No active ruleset for this session yet. Instructors can choose the active ruleset from this page.");
        // Memperbarui `terms[”sessions.ruleset_modal.title”]` menggunakan tuple yang membawa bagian 1: ”Rincian Set Aturan”; bagian 2: ”Ruleset Details”
        // dalam AddSessions.
        terms["sessions.ruleset_modal.title"] = ("Rincian Set Aturan", "Ruleset Details");
        // Memperbarui `terms[”sessions.activate_ruleset”]` menggunakan tuple yang membawa bagian 1: ”Ganti/Atur Set Aturan Aktif”; bagian 2: ”Set or Change
        // Active Ruleset” dalam AddSessions.
        terms["sessions.activate_ruleset"] = ("Ganti/Atur Set Aturan Aktif", "Set or Change Active Ruleset");
        // Memperbarui `terms[”sessions.ruleset_title”]` menggunakan tuple yang membawa bagian 1: ”Pilih Set Aturan untuk Sesi Ini”; bagian 2: ”Select
        // Ruleset for This Session” dalam AddSessions.
        terms["sessions.ruleset_title"] = ("Pilih Set Aturan untuk Sesi Ini", "Select Ruleset for This Session");
        // Memperbarui `terms[”sessions.ruleset_hint”]` menggunakan tuple yang membawa bagian 1: ”Masukkan versi aturan (contoh: 1 atau versi terbaru).”;
        // bagian 2: ”Enter ruleset version (for example: 1 or latest).” dalam AddSessions.
        terms["sessions.ruleset_hint"] = ("Masukkan versi aturan (contoh: 1 atau versi terbaru).", "Enter ruleset version (for example: 1 or latest).");
        // Memperbarui `terms[”sessions.ruleset_form_title”]` menggunakan tuple yang membawa bagian 1: ”Form Aktivasi Set Aturan”; bagian 2: ”Ruleset
        // Activation Form” dalam AddSessions.
        terms["sessions.ruleset_form_title"] = ("Form Aktivasi Set Aturan", "Ruleset Activation Form");
        // Memperbarui `terms[”sessions.ruleset_form_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Pilih set aturan dan versi yang akan menjadi
        // dasar validasi sesi.”; bagian 2: ”Choose the ruleset and version that will drive session validation.” dalam AddSessions.
        terms["sessions.ruleset_form_subtitle"] = ("Pilih set aturan dan versi yang akan menjadi dasar validasi sesi.", "Choose the ruleset and version that will drive session validation.");
        // Memperbarui `terms[”sessions.activate”]` menggunakan tuple yang membawa bagian 1: ”Aktifkan Aturan”; bagian 2: ”Activate Ruleset” dalam
        // AddSessions.
        terms["sessions.activate"] = ("Aktifkan Aturan", "Activate Ruleset");
        // Memperbarui `terms[”sessions.ruleset_guide”]` menggunakan tuple yang membawa bagian 1: ”Panduan Aktivasi Set Aturan”; bagian 2: ”Ruleset
        // Activation Guide” dalam AddSessions.
        terms["sessions.ruleset_guide"] = ("Panduan Aktivasi Set Aturan", "Ruleset Activation Guide");
        // Memperbarui `terms[”sessions.ruleset_step1”]` menggunakan tuple yang membawa bagian 1: ”Pilih set aturan yang akan dipakai.”; bagian 2: ”Choose
        // the ruleset to use.” dalam AddSessions.
        terms["sessions.ruleset_step1"] = ("Pilih set aturan yang akan dipakai.", "Choose the ruleset to use.");
        // Memperbarui `terms[”sessions.ruleset_step2”]` menggunakan tuple yang membawa bagian 1: ”Pilih versi aturan yang ingin diaktifkan.”; bagian 2:
        // ”Choose the ruleset version to activate.” dalam AddSessions.
        terms["sessions.ruleset_step2"] = ("Pilih versi aturan yang ingin diaktifkan.", "Choose the ruleset version to activate.");
        // Memperbarui `terms[”sessions.ruleset_note”]` menggunakan tuple yang membawa bagian 1: ”Pastikan versi berstatus AKTIF sebelum menyimpan.”; bagian
        // 2: ”Ensure the version is ACTIVE before saving.” dalam AddSessions.
        terms["sessions.ruleset_note"] = ("Pastikan versi berstatus AKTIF sebelum menyimpan.", "Ensure the version is ACTIVE before saving.");
        // Memperbarui `terms[”sessions.timeline”]` menggunakan tuple yang membawa bagian 1: ”Linimasa”; bagian 2: ”Timeline” dalam AddSessions.
        terms["sessions.timeline"] = ("Linimasa", "Timeline");
        // Memperbarui `terms[”sessions.timeline_title”]` menggunakan tuple yang membawa bagian 1: ”Linimasa Aktivitas Sesi”; bagian 2: ”Session Activity
        // Timeline” dalam AddSessions.
        terms["sessions.timeline_title"] = ("Linimasa Aktivitas Sesi", "Session Activity Timeline");
        // Memperbarui `terms[”sessions.timeline_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Urutan aktivitas berdasarkan waktu dan nomor urut
        // aktivitas.”; bagian 2: ”Activity order based on timestamp and event sequence.” dalam AddSessions.
        terms["sessions.timeline_subtitle"] = ("Urutan aktivitas berdasarkan waktu dan nomor urut aktivitas.", "Activity order based on timestamp and event sequence.");
        // Memperbarui `terms[”sessions.timeline_player_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Tampilan ini hanya menyorot aktivitas yang
        // dipicu pemain agar evaluasi strategi...; bagian 2: ”This view highlights only player-triggered activity so strategy review stays fo... dalam
        // AddSessions.
        terms["sessions.timeline_player_subtitle"] = ("Tampilan ini hanya menyorot aktivitas yang dipicu pemain agar evaluasi strategi lebih fokus.", "This view highlights only player-triggered activity so strategy review stays focused.");
        // Memperbarui `terms[”sessions.timeline_mixed_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Tampilan ini menggabungkan aksi pemain dan
        // event sistem penting agar alur perma...; bagian 2: ”This view combines player actions and key system events so the game flow is eas... dalam
        // AddSessions.
        terms["sessions.timeline_mixed_subtitle"] = ("Tampilan ini menggabungkan aksi pemain dan event sistem penting agar alur permainan lebih mudah diikuti.", "This view combines player actions and key system events so the game flow is easier to follow.");
        // Memperbarui `terms[”sessions.timeline_filter_label”]` menggunakan tuple yang membawa bagian 1: ”Pilih jenis aktivitas”; bagian 2: ”Choose
        // activity type” dalam AddSessions.
        terms["sessions.timeline_filter_label"] = ("Pilih jenis aktivitas", "Choose activity type");
        // Memperbarui `terms[”sessions.timeline_filter_all”]` menggunakan tuple yang membawa bagian 1: ”Semua aktivitas”; bagian 2: ”All activity” dalam
        // AddSessions.
        terms["sessions.timeline_filter_all"] = ("Semua aktivitas", "All activity");
        // Memperbarui `terms[”sessions.timeline_filter_players”]` menggunakan tuple yang membawa bagian 1: ”Pemain”; bagian 2: ”Players” dalam AddSessions.
        terms["sessions.timeline_filter_players"] = ("Pemain", "Players");
        // Memperbarui `terms[”sessions.timeline_filter_system”]` menggunakan tuple yang membawa bagian 1: ”Sistem”; bagian 2: ”System” dalam AddSessions.
        terms["sessions.timeline_filter_system"] = ("Sistem", "System");
        // Memperbarui `terms[”sessions.timeline_active_players”]` menggunakan tuple yang membawa bagian 1: ”Pemain Aktif”; bagian 2: ”Active Players” dalam
        // AddSessions.
        terms["sessions.timeline_active_players"] = ("Pemain Aktif", "Active Players");
        // Memperbarui `terms[”sessions.timeline_latest_player”]` menggunakan tuple yang membawa bagian 1: ”Pemain Terakhir Aktif”; bagian 2: ”Latest Active
        // Player” dalam AddSessions.
        terms["sessions.timeline_latest_player"] = ("Pemain Terakhir Aktif", "Latest Active Player");
        // Memperbarui `terms[”sessions.timeline_player_empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada aktivitas pemain pada sesi ini.”;
        // bagian 2: ”No player activity is available in this session yet.” dalam AddSessions.
        terms["sessions.timeline_player_empty"] = ("Belum ada aktivitas pemain pada sesi ini.", "No player activity is available in this session yet.");
        // Memperbarui `terms[”sessions.timeline_filtered_empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada aktivitas untuk pemain yang
        // dipilih.”; bagian 2: ”No activity is available for the selected player.” dalam AddSessions.
        terms["sessions.timeline_filtered_empty"] = ("Belum ada aktivitas untuk pemain yang dipilih.", "No activity is available for the selected player.");
        // Memperbarui `terms[”sessions.timeline_chart_progress”]` menggunakan tuple yang membawa bagian 1: ”Diagram Aktivitas dari Awal hingga Selesai”;
        // bagian 2: ”Event Timeline from Start to Finish” dalam AddSessions.
        terms["sessions.timeline_chart_progress"] = ("Diagram Aktivitas dari Awal hingga Selesai", "Event Timeline from Start to Finish");
        // Memperbarui `terms[”sessions.timeline_chart_actor”]` menggunakan tuple yang membawa bagian 1: ”Sebaran Aktivitas per Aktor terhadap Waktu”;
        // bagian 2: ”Event Distribution by Actor Over Time” dalam AddSessions.
        terms["sessions.timeline_chart_actor"] = ("Sebaran Aktivitas per Aktor terhadap Waktu", "Event Distribution by Actor Over Time");
        // Memperbarui `terms[”sessions.timeline_day_turn”]` menggunakan tuple yang membawa bagian 1: ”Hari dan Aktivitas Terakhir”; bagian 2: ”Latest Day
        // and Activity” dalam AddSessions.
        terms["sessions.timeline_day_turn"] = ("Hari dan Aktivitas Terakhir", "Latest Day and Activity");
        // Memperbarui `terms[”sessions.day_label”]` menggunakan tuple yang membawa bagian 1: ”Hari”; bagian 2: ”Day” dalam AddSessions.
        terms["sessions.day_label"] = ("Hari", "Day");
        // Memperbarui `terms[”sessions.turn_label”]` menggunakan tuple yang membawa bagian 1: ”Aksi”; bagian 2: ”Action” dalam AddSessions.
        terms["sessions.turn_label"] = ("Aksi", "Action");
        // Memperbarui `terms[”sessions.activity_default”]` menggunakan tuple yang membawa bagian 1: ”Detail aktivitas belum tersedia”; bagian 2: ”Activity
        // details not available” dalam AddSessions.
        terms["sessions.activity_default"] = ("Detail aktivitas belum tersedia", "Activity details not available");
        // Memperbarui `terms[”sessions.timeline_actor”]` menggunakan tuple yang membawa bagian 1: ”Aktor Aktivitas Terakhir”; bagian 2: ”Latest Activity
        // Actor” dalam AddSessions.
        terms["sessions.timeline_actor"] = ("Aktor Aktivitas Terakhir", "Latest Activity Actor");
        // Memperbarui `terms[”sessions.timeline_flow”]` menggunakan tuple yang membawa bagian 1: ”Alur”; bagian 2: ”Flow” dalam AddSessions.
        terms["sessions.timeline_flow"] = ("Alur", "Flow");
        // Memperbarui `terms[”sessions.timeline_description”]` menggunakan tuple yang membawa bagian 1: ”Penjelasan”; bagian 2: ”Description” dalam
        // AddSessions.
        terms["sessions.timeline_description"] = ("Penjelasan", "Description");
        // Memperbarui `terms[”sessions.timeline_empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada aktivitas linimasa pada sesi ini.”; bagian 2:
        // ”No timeline events for this session yet.” dalam AddSessions.
        terms["sessions.timeline_empty"] = ("Belum ada aktivitas linimasa pada sesi ini.", "No timeline events for this session yet.");
        // Memperbarui `terms[”sessions.timeline_chart_cumulative_events”]` menggunakan tuple yang membawa bagian 1: ”Kumulatif Aktivitas”; bagian 2:
        // ”Cumulative Events” dalam AddSessions.
        terms["sessions.timeline_chart_cumulative_events"] = ("Kumulatif Aktivitas", "Cumulative Events");
        // Memperbarui `terms[”sessions.timeline_chart_event_sequence”]` menggunakan tuple yang membawa bagian 1: ”Urutan aktivitas”; bagian 2: ”Event
        // sequence order” dalam AddSessions.
        terms["sessions.timeline_chart_event_sequence"] = ("Urutan aktivitas", "Event sequence order");
        // Memperbarui `terms[”sessions.timeline_chart_minutes_since_first”]` menggunakan tuple yang membawa bagian 1: ”Menit sejak aktivitas pertama”;
        // bagian 2: ”Minutes since first event” dalam AddSessions.
        terms["sessions.timeline_chart_minutes_since_first"] = ("Menit sejak aktivitas pertama", "Minutes since first event");
        // Memperbarui `terms[”sessions.timeline_chart_event_count”]` menggunakan tuple yang membawa bagian 1: ”Jumlah aktivitas (kumulatif)”; bagian 2:
        // ”Event count (cumulative)” dalam AddSessions.
        terms["sessions.timeline_chart_event_count"] = ("Jumlah aktivitas (kumulatif)", "Event count (cumulative)");
        // Memperbarui `terms[”sessions.timeline_chart_events_per_turn”]` menggunakan tuple yang membawa bagian 1: ”Jumlah aktivitas per slot aksi”; bagian
        // 2: ”Event count per action slot” dalam AddSessions.
        terms["sessions.timeline_chart_events_per_turn"] = ("Jumlah aktivitas per slot aksi", "Event count per action slot");
        // Memperbarui `terms[”sessions.timeline_chart_action_slot”]` menggunakan tuple yang membawa bagian 1: ”Slot aksi”; bagian 2: ”Action slot” dalam
        // AddSessions.
        terms["sessions.timeline_chart_action_slot"] = ("Slot aksi", "Action slot");
        // Memperbarui `terms[”sessions.timeline_tooltip_minutes”]` menggunakan tuple yang membawa bagian 1: ”+{value} menit”; bagian 2: ”+{value} min”
        // dalam AddSessions.
        terms["sessions.timeline_tooltip_minutes"] = ("+{value} menit", "+{value} min");
        // Memperbarui `terms[”sessions.timeline_tooltip_event”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas #{event} | urut {seq} | {actor} |
        // {action}”; bagian 2: ”Event #{event} | seq {seq} | {actor} | {action}” dalam AddSessions.
        terms["sessions.timeline_tooltip_event"] = ("Aktivitas #{event} | urut {seq} | {actor} | {action}", "Event #{event} | seq {seq} | {actor} | {action}");
        // Memperbarui `terms[”sessions.timeline_tooltip_actor”]` menggunakan tuple yang membawa bagian 1: ”{actor} | hari {day} ({weekday}) | slot aksi
        // {turn} | urut {seq} | {action}”; bagian 2: ”{actor} | day {day} ({weekday}) | action slot {turn} | seq {seq} | {action}” dalam AddSessions.
        terms["sessions.timeline_tooltip_actor"] = ("{actor} | hari {day} ({weekday}) | slot aksi {turn} | urut {seq} | {action}", "{actor} | day {day} ({weekday}) | action slot {turn} | seq {seq} | {action}");
        // Memperbarui `terms[”sessions.timeline_actor_help”]` menggunakan tuple yang membawa bagian 1: ”Kategori aktor di grafik: siapa yang memicu
        // aktivitas selama sesi.”; bagian 2: ”Actor categories in the chart: who triggered events during the session.” dalam AddSessions.
        terms["sessions.timeline_actor_help"] = ("Kategori aktor di grafik: siapa yang memicu aktivitas selama sesi.", "Actor categories in the chart: who triggered events during the session.");
        // Memperbarui `terms[”sessions.journey.board_title”]` menggunakan tuple yang membawa bagian 1: ”Jejak perjalanan permainan”; bagian 2: ”Board-style
        // game journey” dalam AddSessions.
        terms["sessions.journey.board_title"] = ("Jejak perjalanan permainan", "Board-style game journey");
        // Memperbarui `terms[”sessions.journey.board_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Visual papan mengikuti urutan aktivitas
        // permainan terbaru secara otomatis.”; bagian 2: ”The board automatically follows the latest gameplay activity order.” dalam AddSessions.
        terms["sessions.journey.board_subtitle"] = ("Visual papan mengikuti urutan aktivitas permainan terbaru secara otomatis.", "The board automatically follows the latest gameplay activity order.");
        // Memperbarui `terms[”sessions.journey.day.sun”]` menggunakan tuple yang membawa bagian 1: ”Min”; bagian 2: ”Sun” dalam AddSessions.
        terms["sessions.journey.day.sun"] = ("Min", "Sun");
        // Memperbarui `terms[”sessions.journey.day.mon”]` menggunakan tuple yang membawa bagian 1: ”Sen”; bagian 2: ”Mon” dalam AddSessions.
        terms["sessions.journey.day.mon"] = ("Sen", "Mon");
        // Memperbarui `terms[”sessions.journey.day.tue”]` menggunakan tuple yang membawa bagian 1: ”Sel”; bagian 2: ”Tue” dalam AddSessions.
        terms["sessions.journey.day.tue"] = ("Sel", "Tue");
        // Memperbarui `terms[”sessions.journey.day.wed”]` menggunakan tuple yang membawa bagian 1: ”Rab”; bagian 2: ”Wed” dalam AddSessions.
        terms["sessions.journey.day.wed"] = ("Rab", "Wed");
        // Memperbarui `terms[”sessions.journey.day.thu”]` menggunakan tuple yang membawa bagian 1: ”Kam”; bagian 2: ”Thu” dalam AddSessions.
        terms["sessions.journey.day.thu"] = ("Kam", "Thu");
        // Memperbarui `terms[”sessions.journey.day.fri”]` menggunakan tuple yang membawa bagian 1: ”Jum”; bagian 2: ”Fri” dalam AddSessions.
        terms["sessions.journey.day.fri"] = ("Jum", "Fri");
        // Memperbarui `terms[”sessions.journey.day.sat”]` menggunakan tuple yang membawa bagian 1: ”Sab”; bagian 2: ”Sat” dalam AddSessions.
        terms["sessions.journey.day.sat"] = ("Sab", "Sat");
        // Memperbarui `terms[”sessions.journey.last_sync”]` menggunakan tuple yang membawa bagian 1: ”Sinkron Data Terakhir”; bagian 2: ”Last Data Sync”
        // dalam AddSessions.
        terms["sessions.journey.last_sync"] = ("Sinkron Data Terakhir", "Last Data Sync");
        // Memperbarui `terms[”sessions.journey.latest_activity”]` menggunakan tuple yang membawa bagian 1: ”Aksi Terakhir”; bagian 2: ”Latest Action” dalam
        // AddSessions.
        terms["sessions.journey.latest_activity"] = ("Aksi Terakhir", "Latest Action");
        // Memperbarui `terms[”sessions.journey.graph_title”]` menggunakan tuple yang membawa bagian 1: ”Grafik Interaktif Pergerakan Permainan”; bagian 2:
        // ”Interactive Gameplay Movement Chart” dalam AddSessions.
        terms["sessions.journey.graph_title"] = ("Grafik Interaktif Pergerakan Permainan", "Interactive Gameplay Movement Chart");
        // Memperbarui `terms[”sessions.journey.graph_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Klik titik grafik untuk menyorot pergerakan
        // pemain pada papan jejak dan daftar ...; bagian 2: ”Click chart points to highlight player movements on the journey board and activ... dalam
        // AddSessions.
        terms["sessions.journey.graph_subtitle"] = ("Klik titik grafik untuk menyorot pergerakan pemain pada papan jejak dan daftar aktivitas.", "Click chart points to highlight player movements on the journey board and activity feed.");
        // Memperbarui `terms[”sessions.journey.graph_unavailable”]` menggunakan tuple yang membawa bagian 1: ”Grafik interaktif belum tersedia.”; bagian 2:
        // ”Interactive graph is not available yet.” dalam AddSessions.
        terms["sessions.journey.graph_unavailable"] = ("Grafik interaktif belum tersedia.", "Interactive graph is not available yet.");
        // Memperbarui `terms[”sessions.journey.graph_empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada pergerakan untuk divisualkan pada
        // grafik.”; bagian 2: ”No movements are available to visualize on the chart.” dalam AddSessions.
        terms["sessions.journey.graph_empty"] = ("Belum ada pergerakan untuk divisualkan pada grafik.", "No movements are available to visualize on the chart.");
        // Memperbarui `terms[”sessions.journey.graph_axis_seq”]` menggunakan tuple yang membawa bagian 1: ”Urutan Aktivitas”; bagian 2: ”Event Sequence”
        // dalam AddSessions.
        terms["sessions.journey.graph_axis_seq"] = ("Urutan Aktivitas", "Event Sequence");
        // Memperbarui `terms[”sessions.journey.graph_axis_order”]` menggunakan tuple yang membawa bagian 1: ”Urutan Pergerakan”; bagian 2: ”Movement Order”
        // dalam AddSessions.
        terms["sessions.journey.graph_axis_order"] = ("Urutan Pergerakan", "Movement Order");
        // Memperbarui `terms[”sessions.journey.no_events”]` menggunakan tuple yang membawa bagian 1: ”Belum ada aktivitas untuk divisualkan.”; bagian 2:
        // ”No events available to visualize yet.” dalam AddSessions.
        terms["sessions.journey.no_events"] = ("Belum ada aktivitas untuk divisualkan.", "No events available to visualize yet.");
        // Memperbarui `terms[”sessions.journey.realtime_status”]` menggunakan tuple yang membawa bagian 1: ”Gagal sinkron alur permainan (status
        // {status}).”; bagian 2: ”Failed to sync gameplay journey (status {status}).” dalam AddSessions.
        terms["sessions.journey.realtime_status"] = ("Gagal sinkron alur permainan (status {status}).", "Failed to sync gameplay journey (status {status}).");
        // Memperbarui `terms[”sessions.journey.realtime_network”]` menggunakan tuple yang membawa bagian 1: ”Koneksi ke server terputus saat sinkron alur
        // permainan.”; bagian 2: ”Connection to server was lost while syncing gameplay journey.” dalam AddSessions.
        terms["sessions.journey.realtime_network"] = ("Koneksi ke server terputus saat sinkron alur permainan.", "Connection to server was lost while syncing gameplay journey.");
        // Memperbarui `terms[”sessions.error.too_many_requests”]` menggunakan tuple yang membawa bagian 1: ”Terlalu banyak permintaan. Tunggu sebentar lalu
        // coba lagi.”; bagian 2: ”Too many requests. Please wait and try again.” dalam AddSessions.
        terms["sessions.error.too_many_requests"] = ("Terlalu banyak permintaan. Tunggu sebentar lalu coba lagi.", "Too many requests. Please wait and try again.");
        // Memperbarui `terms[”sessions.error.load_sessions_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal mengambil daftar sesi. Status:
        // {status}”; bagian 2: ”Failed to fetch session list. Status: {status}” dalam AddSessions.
        terms["sessions.error.load_sessions_failed"] = ("Gagal mengambil daftar sesi. Status: {status}", "Failed to fetch session list. Status: {status}");
        // Memperbarui `terms[”sessions.error.load_timeline_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat alur aktivitas sesi. Status:
        // {status}”; bagian 2: ”Failed to load session activity timeline. Status: {status}” dalam AddSessions.
        terms["sessions.error.load_timeline_failed"] = ("Gagal memuat alur aktivitas sesi. Status: {status}", "Failed to load session activity timeline. Status: {status}");
        // Memperbarui `terms[”sessions.error.load_detail_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat rincian sesi. Status: {status}”;
        // bagian 2: ”Failed to load session details. Status: {status}” dalam AddSessions.
        terms["sessions.error.load_detail_failed"] = ("Gagal memuat rincian sesi. Status: {status}", "Failed to load session details. Status: {status}");
        // Memperbarui `terms[”sessions.error.ruleset_version_required”]` menggunakan tuple yang membawa bagian 1: ”Set aturan dan versinya wajib dipilih.”;
        // bagian 2: ”Ruleset and version are required.” dalam AddSessions.
        terms["sessions.error.ruleset_version_required"] = ("Set aturan dan versinya wajib dipilih.", "Ruleset and version are required.");
        // Memperbarui `terms[”sessions.error.activate_ruleset_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal mengaktifkan set aturan. Status:
        // {status}”; bagian 2: ”Failed to activate ruleset. Status: {status}” dalam AddSessions.
        terms["sessions.error.activate_ruleset_failed"] = ("Gagal mengaktifkan set aturan. Status: {status}", "Failed to activate ruleset. Status: {status}");
        // Memperbarui `terms[”sessions.error.load_rulesets_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat set aturan. Status: {status}”;
        // bagian 2: ”Failed to load rulesets. Status: {status}” dalam AddSessions.
        terms["sessions.error.load_rulesets_failed"] = ("Gagal memuat set aturan. Status: {status}", "Failed to load rulesets. Status: {status}");
        // Memperbarui `terms[”sessions.actor.player”]` menggunakan tuple yang membawa bagian 1: ”Pemain”; bagian 2: ”Player” dalam AddSessions.
        terms["sessions.actor.player"] = ("Pemain", "Player");
        // Memperbarui `terms[”sessions.actor.system”]` menggunakan tuple yang membawa bagian 1: ”Sistem”; bagian 2: ”System” dalam AddSessions.
        terms["sessions.actor.system"] = ("Sistem", "System");
        // Memperbarui `terms[”sessions.actor.instructor”]` menggunakan tuple yang membawa bagian 1: ”Instruktur”; bagian 2: ”Instructor” dalam AddSessions.
        terms["sessions.actor.instructor"] = ("Instruktur", "Instructor");
        // Memperbarui `terms[”sessions.actor.other”]` menggunakan tuple yang membawa bagian 1: ”Lainnya”; bagian 2: ”Other” dalam AddSessions.
        terms["sessions.actor.other"] = ("Lainnya", "Other");
        // Memperbarui `terms[”sessions.actor.player.desc”]` menggunakan tuple yang membawa bagian 1: ”Aksi yang dilakukan pemain selama permainan.”; bagian
        // 2: ”Actions performed by players during gameplay.” dalam AddSessions.
        terms["sessions.actor.player.desc"] = ("Aksi yang dilakukan pemain selama permainan.", "Actions performed by players during gameplay.");
        // Memperbarui `terms[”sessions.actor.system.desc”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas otomatis dari sistem/mesin aturan.”; bagian
        // 2: ”Automatic events generated by the system/rules engine.” dalam AddSessions.
        terms["sessions.actor.system.desc"] = ("Aktivitas otomatis dari sistem/mesin aturan.", "Automatic events generated by the system/rules engine.");
        // Memperbarui `terms[”sessions.actor.instructor.desc”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas yang dipicu instruktur (kontrol
        // sesi).”; bagian 2: ”Events triggered by instructors (session controls).” dalam AddSessions.
        terms["sessions.actor.instructor.desc"] = ("Aktivitas yang dipicu instruktur (kontrol sesi).", "Events triggered by instructors (session controls).");
        // Memperbarui `terms[”sessions.actor.other.desc”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas dengan tipe aktor di luar tiga kategori
        // utama.”; bagian 2: ”Events with actor types outside the three primary categories.” dalam AddSessions.
        terms["sessions.actor.other.desc"] = ("Aktivitas dengan tipe aktor di luar tiga kategori utama.", "Events with actor types outside the three primary categories.");
        // Memperbarui `terms[”status.session.created”]` menggunakan tuple yang membawa bagian 1: ”Dibuat”; bagian 2: ”Created” dalam AddSessions.
        terms["status.session.created"] = ("Dibuat", "Created");
        // Memperbarui `terms[”status.MulaiSesi”]` menggunakan tuple yang membawa bagian 1: ”Dimulai”; bagian 2: ”Started” dalam AddSessions.
        terms["status.MulaiSesi"] = ("Dimulai", "Started");
        // Memperbarui `terms[”status.AkhiriSesi”]` menggunakan tuple yang membawa bagian 1: ”Selesai”; bagian 2: ”Ended” dalam AddSessions.
        terms["status.AkhiriSesi"] = ("Selesai", "Ended");
        // Memperbarui `terms[”status.session.cancelled”]` menggunakan tuple yang membawa bagian 1: ”Dibatalkan”; bagian 2: ”Cancelled” dalam AddSessions.
        terms["status.session.cancelled"] = ("Dibatalkan", "Cancelled");
        // Memperbarui `terms[”status.session.unknown”]` menggunakan tuple yang membawa bagian 1: ”Tidak Diketahui”; bagian 2: ”Unknown” dalam AddSessions.
        terms["status.session.unknown"] = ("Tidak Diketahui", "Unknown");
    // Menutup scope metode AddSessions; bagian berikut berada di luar batas blok tersebut dalam AddSessions.
    }
// Menutup scope tipe UiTextLexicon; bagian berikut berada di luar batas blok tersebut.
}
