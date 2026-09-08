// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui UiTextLexicon.Core.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `UiTextLexicon`.
internal static partial class UiTextLexicon
// Membuka scope tipe UiTextLexicon; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `AddCore` dengan hasil bertipe `void`; operasi ini menangani add core. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddCore(Dictionary<string, (string Id, string En)> terms)
    // Membuka scope metode AddCore; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddCore.
    {
        // Memperbarui `terms[”nav.home”]` menggunakan tuple yang membawa bagian 1: ”Beranda”; bagian 2: ”Home” dalam AddCore.
        terms["nav.home"] = ("Beranda", "Home");
        // Memperbarui `terms[”nav.sessions”]` menggunakan tuple yang membawa bagian 1: ”Sesi Permainan”; bagian 2: ”Game Sessions” dalam AddCore.
        terms["nav.sessions"] = ("Sesi Permainan", "Game Sessions");
        // Memperbarui `terms[”nav.players”]` menggunakan tuple yang membawa bagian 1: ”Direktori Pemain”; bagian 2: ”Player Directory” dalam AddCore.
        terms["nav.players"] = ("Direktori Pemain", "Player Directory");
        // Memperbarui `terms[”nav.ruleset”]` menggunakan tuple yang membawa bagian 1: ”Set Aturan”; bagian 2: ”Rulesets” dalam AddCore.
        terms["nav.ruleset"] = ("Set Aturan", "Rulesets");
        // Memperbarui `terms[”nav.privacy”]` menggunakan tuple yang membawa bagian 1: ”Buku Aturan”; bagian 2: ”Rulebook” dalam AddCore.
        terms["nav.privacy"] = ("Buku Aturan", "Rulebook");
        // Memperbarui `terms[”auth.login”]` menggunakan tuple yang membawa bagian 1: ”Masuk”; bagian 2: ”Sign In” dalam AddCore.
        terms["auth.login"] = ("Masuk", "Sign In");
        // Memperbarui `terms[”auth.logout”]` menggunakan tuple yang membawa bagian 1: ”Keluar”; bagian 2: ”Logout” dalam AddCore.
        terms["auth.logout"] = ("Keluar", "Logout");
        // Memperbarui `terms[”auth.access”]` menggunakan tuple yang membawa bagian 1: ”Akses Dasbor”; bagian 2: ”Dashboard Access” dalam AddCore.
        terms["auth.access"] = ("Akses Dasbor", "Dashboard Access");
        // Memperbarui `terms[”auth.title”]` menggunakan tuple yang membawa bagian 1: ”Masuk ke Cashflowpoly”; bagian 2: ”Sign In to Cashflowpoly” dalam
        // AddCore.
        terms["auth.title"] = ("Masuk ke Cashflowpoly", "Sign In to Cashflowpoly");
        // Memperbarui `terms[”auth.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Masuk dengan akun instruktur atau pemain untuk melanjutkan.”;
        // bagian 2: ”Sign in with an instructor or player account to continue.” dalam AddCore.
        terms["auth.subtitle"] = ("Masuk dengan akun instruktur atau pemain untuk melanjutkan.", "Sign in with an instructor or player account to continue.");
        // Memperbarui `terms[”auth.display_name”]` menggunakan tuple yang membawa bagian 1: ”Nama Tampilan”; bagian 2: ”Display Name” dalam AddCore.
        terms["auth.display_name"] = ("Nama Tampilan", "Display Name");
        // Memperbarui `terms[”auth.username”]` menggunakan tuple yang membawa bagian 1: ”Nama Pengguna”; bagian 2: ”Username” dalam AddCore.
        terms["auth.username"] = ("Nama Pengguna", "Username");
        // Memperbarui `terms[”auth.password”]` menggunakan tuple yang membawa bagian 1: ”Kata Sandi”; bagian 2: ”Password” dalam AddCore.
        terms["auth.password"] = ("Kata Sandi", "Password");
        // Memperbarui `terms[”auth.show_password”]` menggunakan tuple yang membawa bagian 1: ”Lihat”; bagian 2: ”Show” dalam AddCore.
        terms["auth.show_password"] = ("Lihat", "Show");
        // Memperbarui `terms[”auth.hide_password”]` menggunakan tuple yang membawa bagian 1: ”Sembunyikan”; bagian 2: ”Hide” dalam AddCore.
        terms["auth.hide_password"] = ("Sembunyikan", "Hide");
        // Memperbarui `terms[”auth.submit”]` menggunakan tuple yang membawa bagian 1: ”Masuk”; bagian 2: ”Sign In” dalam AddCore.
        terms["auth.submit"] = ("Masuk", "Sign In");
        // Memperbarui `terms[”auth.no_account”]` menggunakan tuple yang membawa bagian 1: ”Belum punya akun?”; bagian 2: ”Don't have an account?” dalam
        // AddCore.
        terms["auth.no_account"] = ("Belum punya akun?", "Don't have an account?");
        // Memperbarui `terms[”auth.have_account”]` menggunakan tuple yang membawa bagian 1: ”Sudah punya akun?”; bagian 2: ”Already have an account?” dalam
        // AddCore.
        terms["auth.have_account"] = ("Sudah punya akun?", "Already have an account?");
        // Memperbarui `terms[”auth.register”]` menggunakan tuple yang membawa bagian 1: ”Daftar”; bagian 2: ”Register” dalam AddCore.
        terms["auth.register"] = ("Daftar", "Register");
        // Memperbarui `terms[”auth.register_title”]` menggunakan tuple yang membawa bagian 1: ”Buat Akun Baru”; bagian 2: ”Register New Account” dalam
        // AddCore.
        terms["auth.register_title"] = ("Buat Akun Baru", "Register New Account");
        // Memperbarui `terms[”auth.register_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Buat akun baru untuk mengakses sesi dan analitika
        // permainan.”; bagian 2: ”Create a new account to access sessions and gameplay analytics.” dalam AddCore.
        terms["auth.register_subtitle"] = ("Buat akun baru untuk mengakses sesi dan analitika permainan.", "Create a new account to access sessions and gameplay analytics.");
        // Memperbarui `terms[”auth.confirm_password”]` menggunakan tuple yang membawa bagian 1: ”Konfirmasi Kata Sandi”; bagian 2: ”Confirm Password” dalam
        // AddCore.
        terms["auth.confirm_password"] = ("Konfirmasi Kata Sandi", "Confirm Password");
        // Memperbarui `terms[”auth.role”]` menggunakan tuple yang membawa bagian 1: ”Peran Pengguna”; bagian 2: ”User Role” dalam AddCore.
        terms["auth.role"] = ("Peran Pengguna", "User Role");
        // Memperbarui `terms[”auth.role_player”]` menggunakan tuple yang membawa bagian 1: ”Pemain”; bagian 2: ”Player” dalam AddCore.
        terms["auth.role_player"] = ("Pemain", "Player");
        // Memperbarui `terms[”auth.role_instructor”]` menggunakan tuple yang membawa bagian 1: ”Instruktur”; bagian 2: ”Instructor” dalam AddCore.
        terms["auth.role_instructor"] = ("Instruktur", "Instructor");
        // Memperbarui `terms[”auth.submit_register”]` menggunakan tuple yang membawa bagian 1: ”Buat Akun”; bagian 2: ”Create Account” dalam AddCore.
        terms["auth.submit_register"] = ("Buat Akun", "Create Account");
        // Memperbarui `terms[”auth.login_panel_title”]` menggunakan tuple yang membawa bagian 1: ”Sudah Punya Akun?”; bagian 2: ”Already Have an Account?”
        // dalam AddCore.
        terms["auth.login_panel_title"] = ("Sudah Punya Akun?", "Already Have an Account?");
        // Memperbarui `terms[”auth.login_panel_desc”]` menggunakan tuple yang membawa bagian 1: ”Masuk dengan akun Anda untuk melanjutkan pekerjaan.”;
        // bagian 2: ”Sign in with your account to continue your work.” dalam AddCore.
        terms["auth.login_panel_desc"] = ("Masuk dengan akun Anda untuk melanjutkan pekerjaan.", "Sign in with your account to continue your work.");
        // Memperbarui `terms[”auth.login_panel_cta”]` menggunakan tuple yang membawa bagian 1: ”MASUK”; bagian 2: ”SIGN IN” dalam AddCore.
        terms["auth.login_panel_cta"] = ("MASUK", "SIGN IN");
        // Memperbarui `terms[”auth.register_panel_title”]` menggunakan tuple yang membawa bagian 1: ”Belum Punya Akun?”; bagian 2: ”Don't Have an Account
        // Yet?” dalam AddCore.
        terms["auth.register_panel_title"] = ("Belum Punya Akun?", "Don't Have an Account Yet?");
        // Memperbarui `terms[”auth.register_panel_desc”]` menggunakan tuple yang membawa bagian 1: ”Buat akun baru untuk mengakses analitika sesi dan data
        // permainan.”; bagian 2: ”Create a new account to access session analytics and gameplay data.” dalam AddCore.
        terms["auth.register_panel_desc"] = ("Buat akun baru untuk mengakses analitika sesi dan data permainan.", "Create a new account to access session analytics and gameplay data.");
        // Memperbarui `terms[”auth.register_panel_cta”]` menggunakan tuple yang membawa bagian 1: ”DAFTAR”; bagian 2: ”REGISTER” dalam AddCore.
        terms["auth.register_panel_cta"] = ("DAFTAR", "REGISTER");
        // Memperbarui `terms[”auth.placeholder.full_name”]` menggunakan tuple yang membawa bagian 1: ”Nama Lengkap”; bagian 2: ”Full Name” dalam AddCore.
        terms["auth.placeholder.full_name"] = ("Nama Lengkap", "Full Name");
        // Memperbarui `terms[”auth.placeholder.new_username”]` menggunakan tuple yang membawa bagian 1: ”pemain_baru”; bagian 2: ”new_player” dalam
        // AddCore.
        terms["auth.placeholder.new_username"] = ("pemain_baru", "new_player");
        // Memperbarui `terms[”auth.login_note.quick_access”]` menggunakan tuple yang membawa bagian 1: ”Akses sesi lebih cepat”; bagian 2: ”Faster session
        // access” dalam AddCore.
        terms["auth.login_note.quick_access"] = ("Akses sesi lebih cepat", "Faster session access");
        // Memperbarui `terms[”auth.login_note.realtime_monitoring”]` menggunakan tuple yang membawa bagian 1: ”Pemantauan analitika waktu nyata”; bagian 2:
        // ”Real-time analytics monitoring” dalam AddCore.
        terms["auth.login_note.realtime_monitoring"] = ("Pemantauan analitika waktu nyata", "Real-time analytics monitoring");
        // Memperbarui `terms[”auth.login_note.centralized_ruleset”]` menggunakan tuple yang membawa bagian 1: ”Referensi Set Aturan terpusat”; bagian 2:
        // ”Centralized ruleset reference” dalam AddCore.
        terms["auth.login_note.centralized_ruleset"] = ("Referensi Set Aturan terpusat", "Centralized ruleset reference");
        // Memperbarui `terms[”auth.error.login_required”]` menggunakan tuple yang membawa bagian 1: ”Nama Pengguna dan Kata Sandi wajib diisi.”; bagian 2:
        // ”Username and password are required.” dalam AddCore.
        terms["auth.error.login_required"] = ("Nama Pengguna dan Kata Sandi wajib diisi.", "Username and password are required.");
        // Memperbarui `terms[”auth.error.login_failed”]` menggunakan tuple yang membawa bagian 1: ”Login gagal.”; bagian 2: ”Login failed.” dalam AddCore.
        terms["auth.error.login_failed"] = ("Login gagal.", "Login failed.");
        // Memperbarui `terms[”auth.error.api_unavailable”]` menggunakan tuple yang membawa bagian 1: ”Layanan data belum aktif. Jalankan layanan aplikasi
        // lalu coba lagi.”; bagian 2: ”The data service is not running. Start the application service and try again.” dalam AddCore.
        terms["auth.error.api_unavailable"] = ("Layanan data belum aktif. Jalankan layanan aplikasi lalu coba lagi.", "The data service is not running. Start the application service and try again.");
        // Memperbarui `terms[”auth.error.login_response_invalid”]` menggunakan tuple yang membawa bagian 1: ”Respons login tidak valid.”; bagian 2:
        // ”Invalid login response.” dalam AddCore.
        terms["auth.error.login_response_invalid"] = ("Respons login tidak valid.", "Invalid login response.");
        // Memperbarui `terms[”auth.error.register_required”]` menggunakan tuple yang membawa bagian 1: ”Nama tampilan, Nama Pengguna, Kata Sandi, dan
        // konfirmasi Kata Sandi wajib diisi...; bagian 2: ”Display name, username, password, and confirmation are required.” dalam AddCore.
        terms["auth.error.register_required"] = ("Nama tampilan, Nama Pengguna, Kata Sandi, dan konfirmasi Kata Sandi wajib diisi.", "Display name, username, password, and confirmation are required.");
        // Memperbarui `terms[”auth.error.role_invalid”]` menggunakan tuple yang membawa bagian 1: ”Peran tidak valid.”; bagian 2: ”Invalid role.” dalam
        // AddCore.
        terms["auth.error.role_invalid"] = ("Peran tidak valid.", "Invalid role.");
        // Memperbarui `terms[”auth.error.confirm_mismatch”]` menggunakan tuple yang membawa bagian 1: ”Konfirmasi Kata Sandi tidak sama.”; bagian 2:
        // ”Password confirmation does not match.” dalam AddCore.
        terms["auth.error.confirm_mismatch"] = ("Konfirmasi Kata Sandi tidak sama.", "Password confirmation does not match.");
        // Memperbarui `terms[”auth.error.register_failed”]` menggunakan tuple yang membawa bagian 1: ”Registrasi gagal.”; bagian 2: ”Registration failed.”
        // dalam AddCore.
        terms["auth.error.register_failed"] = ("Registrasi gagal.", "Registration failed.");
        // Memperbarui `terms[”auth.error.register_response_invalid”]` menggunakan tuple yang membawa bagian 1: ”Respons registrasi tidak valid.”; bagian 2:
        // ”Invalid registration response.” dalam AddCore.
        terms["auth.error.register_response_invalid"] = ("Respons registrasi tidak valid.", "Invalid registration response.");
        // Memperbarui `terms[”lang.label”]` menggunakan tuple yang membawa bagian 1: ”Bahasa”; bagian 2: ”Language” dalam AddCore.
        terms["lang.label"] = ("Bahasa", "Language");
        // Memperbarui `terms[”lang.code.id”]` menggunakan tuple yang membawa bagian 1: ”ID”; bagian 2: ”ID” dalam AddCore.
        terms["lang.code.id"] = ("ID", "ID");
        // Memperbarui `terms[”lang.code.en”]` menggunakan tuple yang membawa bagian 1: ”EN”; bagian 2: ”EN” dalam AddCore.
        terms["lang.code.en"] = ("EN", "EN");
        // Memperbarui `terms[”lang.option.id”]` menggunakan tuple yang membawa bagian 1: ”Bahasa Indonesia (ID)”; bagian 2: ”Indonesian (ID)” dalam
        // AddCore.
        terms["lang.option.id"] = ("Bahasa Indonesia (ID)", "Indonesian (ID)");
        // Memperbarui `terms[”lang.option.en”]` menggunakan tuple yang membawa bagian 1: ”Bahasa Inggris (EN)”; bagian 2: ”English (EN)” dalam AddCore.
        terms["lang.option.en"] = ("Bahasa Inggris (EN)", "English (EN)");
        // Memperbarui `terms[”profile.role”]` menggunakan tuple yang membawa bagian 1: ”Peran”; bagian 2: ”Role” dalam AddCore.
        terms["profile.role"] = ("Peran", "Role");
        // Memperbarui `terms[”layout.skip_to_content”]` menggunakan tuple yang membawa bagian 1: ”Lewati ke konten”; bagian 2: ”Skip to content” dalam
        // AddCore.
        terms["layout.skip_to_content"] = ("Lewati ke konten", "Skip to content");
        // Memperbarui `terms[”layout.toggle_nav”]` menggunakan tuple yang membawa bagian 1: ”Buka/tutup menu navigasi”; bagian 2: ”Toggle navigation menu”
        // dalam AddCore.
        terms["layout.toggle_nav"] = ("Buka/tutup menu navigasi", "Toggle navigation menu");
        // Memperbarui `terms[”layout.logo_alt”]` menggunakan tuple yang membawa bagian 1: ”Logo Cashflowpoly”; bagian 2: ”Cashflowpoly logo” dalam AddCore.
        terms["layout.logo_alt"] = ("Logo Cashflowpoly", "Cashflowpoly logo");
        // Memperbarui `terms[”layout.profile_alt”]` menggunakan tuple yang membawa bagian 1: ”Profil”; bagian 2: ”Profile” dalam AddCore.
        terms["layout.profile_alt"] = ("Profil", "Profile");
        // Memperbarui `terms[”layout.brand_name”]` menggunakan tuple yang membawa bagian 1: ”Cashflowpoly”; bagian 2: ”Cashflowpoly” dalam AddCore.
        terms["layout.brand_name"] = ("Cashflowpoly", "Cashflowpoly");
        // Memperbarui `terms[”layout.quickstart.aria”]` menggunakan tuple yang membawa bagian 1: ”Panduan cepat penggunaan”; bagian 2: ”Quick usage guide”
        // dalam AddCore.
        terms["layout.quickstart.aria"] = ("Panduan cepat penggunaan", "Quick usage guide");
        // Memperbarui `terms[”layout.quickstart.kicker”]` menggunakan tuple yang membawa bagian 1: ”Mulai dari sini”; bagian 2: ”Start here” dalam AddCore.
        terms["layout.quickstart.kicker"] = ("Mulai dari sini", "Start here");
        // Memperbarui `terms[”layout.quickstart.hide”]` menggunakan tuple yang membawa bagian 1: ”Sembunyikan panduan”; bagian 2: ”Hide guide” dalam
        // AddCore.
        terms["layout.quickstart.hide"] = ("Sembunyikan panduan", "Hide guide");
        // Memperbarui `terms[”layout.quickstart.show”]` menggunakan tuple yang membawa bagian 1: ”Tampilkan panduan”; bagian 2: ”Show guide” dalam AddCore.
        terms["layout.quickstart.show"] = ("Tampilkan panduan", "Show guide");
        // Memperbarui `terms[”layout.quickstart.current”]` menggunakan tuple yang membawa bagian 1: ”Anda sedang di langkah ini”; bagian 2: ”You are
        // currently on this step” dalam AddCore.
        terms["layout.quickstart.current"] = ("Anda sedang di langkah ini", "You are currently on this step");
        // Memperbarui `terms[”layout.quickstart.open_menu”]` menggunakan tuple yang membawa bagian 1: ”Buka menu”; bagian 2: ”Open menu” dalam AddCore.
        terms["layout.quickstart.open_menu"] = ("Buka menu", "Open menu");
        // Memperbarui `terms[”layout.quickstart.instructor.title”]` menggunakan tuple yang membawa bagian 1: ”Panduan instruktur”; bagian 2: ”Instructor
        // guide” dalam AddCore.
        terms["layout.quickstart.instructor.title"] = ("Panduan instruktur", "Instructor guide");
        // Memperbarui `terms[”layout.quickstart.instructor.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Gunakan empat langkah ini untuk memeriksa
        // aturan, membaca sesi, dan menindaklan...; bagian 2: ”Use these four steps to review rules, read sessions, and follow up on player re... dalam
        // AddCore.
        terms["layout.quickstart.instructor.subtitle"] = ("Gunakan empat langkah ini untuk memeriksa aturan, membaca sesi, dan menindaklanjuti hasil pemain.", "Use these four steps to review rules, read sessions, and follow up on player results.");
        // Memperbarui `terms[”layout.quickstart.instructor.step1.title”]` menggunakan tuple yang membawa bagian 1: ”Cek Set Aturan”; bagian 2: ”Review
        // Rulesets” dalam AddCore.
        terms["layout.quickstart.instructor.step1.title"] = ("Cek Set Aturan", "Review Rulesets");
        // Memperbarui `terms[”layout.quickstart.instructor.step1.desc”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan mode, target, dan fitur aktif
        // agar hasil sesi dibaca dengan acuan ya...; bagian 2: ”Compare the mode, targets, and enabled features so session results use the corr... dalam
        // AddCore.
        terms["layout.quickstart.instructor.step1.desc"] = ("Bandingkan mode, target, dan fitur aktif agar hasil sesi dibaca dengan acuan yang benar.", "Compare the mode, targets, and enabled features so session results use the correct reference.");
        // Memperbarui `terms[”layout.quickstart.instructor.step2.title”]` menggunakan tuple yang membawa bagian 1: ”Pantau sesi”; bagian 2: ”Monitor
        // sessions” dalam AddCore.
        terms["layout.quickstart.instructor.step2.title"] = ("Pantau sesi", "Monitor sessions");
        // Memperbarui `terms[”layout.quickstart.instructor.step2.desc”]` menggunakan tuple yang membawa bagian 1: ”Pilih sesi, periksa status dan
        // ringkasannya, lalu telusuri urutan aktivitas per...; bagian 2: ”Choose a session, review its status and summary, then trace the gameplay
        // activi... dalam AddCore.
        terms["layout.quickstart.instructor.step2.desc"] = ("Pilih sesi, periksa status dan ringkasannya, lalu telusuri urutan aktivitas permainan.", "Choose a session, review its status and summary, then trace the gameplay activity sequence.");
        // Memperbarui `terms[”layout.quickstart.instructor.step3.title”]` menggunakan tuple yang membawa bagian 1: ”Evaluasi per pemain”; bagian 2:
        // ”Evaluate each player” dalam AddCore.
        terms["layout.quickstart.instructor.step3.title"] = ("Evaluasi per pemain", "Evaluate each player");
        // Memperbarui `terms[”layout.quickstart.instructor.step3.desc”]` menggunakan tuple yang membawa bagian 1: ”Buka pemain pada sesi yang sama, lalu
        // gunakan Analitika untuk membahas hasil ti...; bagian 2: ”Open players from the same session, then use Analytics to discuss each individu... dalam
        // AddCore.
        terms["layout.quickstart.instructor.step3.desc"] = ("Buka pemain pada sesi yang sama, lalu gunakan Analitika untuk membahas hasil tiap individu.", "Open players from the same session, then use Analytics to discuss each individual's results.");
        // Memperbarui `terms[”layout.quickstart.instructor.step4.title”]` menggunakan tuple yang membawa bagian 1: ”Rujuk Buku Aturan”; bagian 2: ”Consult
        // the Rulebook” dalam AddCore.
        terms["layout.quickstart.instructor.step4.title"] = ("Rujuk Buku Aturan", "Consult the Rulebook");
        // Memperbarui `terms[”layout.quickstart.instructor.step4.desc”]` menggunakan tuple yang membawa bagian 1: ”Cocokkan istilah, alur giliran, dan
        // perhitungan poin kebahagiaan sebelum menari...; bagian 2: ”Verify terms, turn flow, and score calculations before drawing conclusions.” dalam
        // AddCore.
        terms["layout.quickstart.instructor.step4.desc"] = ("Cocokkan istilah, alur giliran, dan perhitungan poin kebahagiaan sebelum menarik kesimpulan.", "Verify terms, turn flow, and score calculations before drawing conclusions.");
        // Memperbarui `terms[”layout.quickstart.player.title”]` menggunakan tuple yang membawa bagian 1: ”Panduan pemain”; bagian 2: ”Player guide” dalam
        // AddCore.
        terms["layout.quickstart.player.title"] = ("Panduan pemain", "Player guide");
        // Memperbarui `terms[”layout.quickstart.player.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Gunakan empat langkah ini untuk menemukan
        // sesi, membaca hasil, dan memahami atu...; bagian 2: ”Use these four steps to find sessions, review results, and understand the rules... dalam
        // AddCore.
        terms["layout.quickstart.player.subtitle"] = ("Gunakan empat langkah ini untuk menemukan sesi, membaca hasil, dan memahami aturan yang berlaku.", "Use these four steps to find sessions, review results, and understand the rules in effect.");
        // Memperbarui `terms[”layout.quickstart.player.step1.title”]` menggunakan tuple yang membawa bagian 1: ”Pilih sesi”; bagian 2: ”Pick a session”
        // dalam AddCore.
        terms["layout.quickstart.player.step1.title"] = ("Pilih sesi", "Pick a session");
        // Memperbarui `terms[”layout.quickstart.player.step1.desc”]` menggunakan tuple yang membawa bagian 1: ”Buka Sesi Permainan untuk melihat seluruh
        // sesi yang pernah atau sedang Anda iku...; bagian 2: ”Open Game Sessions to view every session you have joined or are currently playi... dalam
        // AddCore.
        terms["layout.quickstart.player.step1.desc"] = ("Buka Sesi Permainan untuk melihat seluruh sesi yang pernah atau sedang Anda ikuti.", "Open Game Sessions to view every session you have joined or are currently playing.");
        // Memperbarui `terms[”layout.quickstart.player.step2.title”]` menggunakan tuple yang membawa bagian 1: ”Baca progres sesi”; bagian 2: ”Read session
        // progress” dalam AddCore.
        terms["layout.quickstart.player.step2.title"] = ("Baca progres sesi", "Read session progress");
        // Memperbarui `terms[”layout.quickstart.player.step2.desc”]` menggunakan tuple yang membawa bagian 1: ”Buka analitika sesi untuk membaca poin
        // kebahagiaan, arus kas, posisi permainan,...; bagian 2: ”Open session analytics to review your scores, cashflow, game position, and acti... dalam
        // AddCore.
        terms["layout.quickstart.player.step2.desc"] = ("Buka analitika sesi untuk membaca poin kebahagiaan, arus kas, posisi permainan, dan aktivitas Anda.", "Open session analytics to review your scores, cashflow, game position, and activity.");
        // Memperbarui `terms[”layout.quickstart.player.step3.title”]` menggunakan tuple yang membawa bagian 1: ”Pahami Set Aturan”; bagian 2: ”Understand
        // rulesets” dalam AddCore.
        terms["layout.quickstart.player.step3.title"] = ("Pahami Set Aturan", "Understand rulesets");
        // Memperbarui `terms[”layout.quickstart.player.step3.desc”]` menggunakan tuple yang membawa bagian 1: ”Buka Set Aturan dari sesi untuk mengetahui
        // target, penalti, batas aksi, dan fit...; bagian 2: ”Open the session ruleset to understand targets, penalties, action limits, and m... dalam
        // AddCore.
        terms["layout.quickstart.player.step3.desc"] = ("Buka Set Aturan dari sesi untuk mengetahui target, penalti, batas aksi, dan fitur sesuai mode.", "Open the session ruleset to understand targets, penalties, action limits, and mode-specific features.");
        // Memperbarui `terms[”layout.quickstart.player.step4.title”]` menggunakan tuple yang membawa bagian 1: ”Rujuk Buku Aturan”; bagian 2: ”Consult the
        // Rulebook” dalam AddCore.
        terms["layout.quickstart.player.step4.title"] = ("Rujuk Buku Aturan", "Consult the Rulebook");
        // Memperbarui `terms[”layout.quickstart.player.step4.desc”]` menggunakan tuple yang membawa bagian 1: ”Gunakan Buku Aturan saat arti istilah,
        // langkah giliran, atau cara menghitung po...; bagian 2: ”Use the Rulebook when a term, turn step, or score calculation needs clarificati... dalam
        // AddCore.
        terms["layout.quickstart.player.step4.desc"] = ("Gunakan Buku Aturan saat arti istilah, langkah giliran, atau cara menghitung poin kebahagiaan belum jelas.", "Use the Rulebook when a term, turn step, or score calculation needs clarification.");
        // Memperbarui `terms[”footer.rulebook_copyright”]` menggunakan tuple yang membawa bagian 1: ”Hak Cipta Buku Aturan”; bagian 2: ”Rulebook Copyright”
        // dalam AddCore.
        terms["footer.rulebook_copyright"] = ("Hak Cipta Buku Aturan", "Rulebook Copyright");
        // Memperbarui `terms[”footer.privacy”]` menggunakan tuple yang membawa bagian 1: ”Kebijakan Privasi”; bagian 2: ”Privacy Policy” dalam AddCore.
        terms["footer.privacy"] = ("Kebijakan Privasi", "Privacy Policy");
        // Memperbarui `terms[”footer.terms”]` menggunakan tuple yang membawa bagian 1: ”Ketentuan Penggunaan”; bagian 2: ”Terms of Use” dalam AddCore.
        terms["footer.terms"] = ("Ketentuan Penggunaan", "Terms of Use");
        // Memperbarui `terms[”footer.legal_navigation”]` menggunakan tuple yang membawa bagian 1: ”Tautan informasi dan kebijakan”; bagian 2: ”Information
        // and policy links” dalam AddCore.
        terms["footer.legal_navigation"] = ("Tautan informasi dan kebijakan", "Information and policy links");
        // Memperbarui `terms[”legal.academic_notice”]` menggunakan tuple yang membawa bagian 1: ”Cashflowpoly merupakan proyek akademik dan bukan badan
        // usaha atau badan hukum.”; bagian 2: ”Cashflowpoly is an academic project, not a business or legal entity.” dalam AddCore.
        terms["legal.academic_notice"] = ("Cashflowpoly merupakan proyek akademik dan bukan badan usaha atau badan hukum.", "Cashflowpoly is an academic project, not a business or legal entity.");
        // Memperbarui `terms[”privacy.kicker”]` menggunakan tuple yang membawa bagian 1: ”Informasi data”; bagian 2: ”Data information” dalam AddCore.
        terms["privacy.kicker"] = ("Informasi data", "Data information");
        // Memperbarui `terms[”privacy.title”]` menggunakan tuple yang membawa bagian 1: ”Kebijakan Privasi”; bagian 2: ”Privacy Policy” dalam AddCore.
        terms["privacy.title"] = ("Kebijakan Privasi", "Privacy Policy");
        // Memperbarui `terms[”privacy.intro”]` menggunakan tuple yang membawa bagian 1: ”Halaman ini menjelaskan data yang dicatat, tujuan pemakaiannya,
        // dan batas layan...; bagian 2: ”This page explains what data is recorded, why it is used, and the limits of dat... dalam AddCore.
        terms["privacy.intro"] = ("Halaman ini menjelaskan data yang dicatat, tujuan pemakaiannya, dan batas layanan pengelolaan data pada proyek ini.", "This page explains what data is recorded, why it is used, and the limits of data-management services in this project.");
        // Memperbarui `terms[”privacy.scope.title”]` menggunakan tuple yang membawa bagian 1: ”Cakupan”; bagian 2: ”Scope” dalam AddCore.
        terms["privacy.scope.title"] = ("Cakupan", "Scope");
        // Memperbarui `terms[”privacy.scope.desc”]` menggunakan tuple yang membawa bagian 1: ”Kebijakan ini berlaku untuk akun, sesi, setup, aktivitas
        // permainan, transaksi, ...; bagian 2: ”This policy applies to accounts, sessions, setup, gameplay activity, transactio... dalam AddCore.
        terms["privacy.scope.desc"] = ("Kebijakan ini berlaku untuk akun, sesi, setup, aktivitas permainan, transaksi, hasil, dan analitika yang diproses oleh aplikasi Cashflowpoly.", "This policy applies to accounts, sessions, setup, gameplay activity, transactions, results, and analytics processed by the Cashflowpoly application.");
        // Memperbarui `terms[”privacy.data.title”]` menggunakan tuple yang membawa bagian 1: ”Data yang dicatat”; bagian 2: ”Data we record” dalam AddCore.
        terms["privacy.data.title"] = ("Data yang dicatat", "Data we record");
        // Memperbarui `terms[”privacy.data.account”]` menggunakan tuple yang membawa bagian 1: ”Data akun: nama tampilan, nama pengguna, peran, dan
        // informasi autentikasi yang ...; bagian 2: ”Account data: display name, username, role, and secured authentication informat... dalam AddCore.
        terms["privacy.data.account"] = ("Data akun: nama tampilan, nama pengguna, peran, dan informasi autentikasi yang telah diamankan.", "Account data: display name, username, role, and secured authentication information.");
        // Memperbarui `terms[”privacy.data.gameplay”]` menggunakan tuple yang membawa bagian 1: ”Data permainan: peserta, setup, urutan kejadian, perubahan
        // koin, kartu yang dil...; bagian 2: ”Gameplay data: participants, setup, event sequence, coin changes, reported card... dalam AddCore.
        terms["privacy.data.gameplay"] = ("Data permainan: peserta, setup, urutan kejadian, perubahan koin, kartu yang dilaporkan, skor, dan analitika.", "Gameplay data: participants, setup, event sequence, coin changes, reported cards, scores, and analytics.");
        // Memperbarui `terms[”privacy.data.technical”]` menggunakan tuple yang membawa bagian 1: ”Data teknis terbatas: waktu akses, hasil login, kode
        // status, dan ID pelacakan u...; bagian 2: ”Limited technical data: access time, login result, status code, and trace ID fo... dalam AddCore.
        terms["privacy.data.technical"] = ("Data teknis terbatas: waktu akses, hasil login, kode status, dan ID pelacakan untuk keamanan serta perbaikan gangguan.", "Limited technical data: access time, login result, status code, and trace ID for security and troubleshooting.");
        // Memperbarui `terms[”privacy.purpose.title”]` menggunakan tuple yang membawa bagian 1: ”Tujuan penggunaan”; bagian 2: ”How data is used” dalam
        // AddCore.
        terms["privacy.purpose.title"] = ("Tujuan penggunaan", "How data is used");
        // Memperbarui `terms[”privacy.purpose.desc”]` menggunakan tuple yang membawa bagian 1: ”Data digunakan untuk menjalankan permainan yang dicatat
        // melalui aplikasi pendam...; bagian 2: ”Data is used to run gameplay recorded through the companion application, enforc... dalam AddCore.
        terms["privacy.purpose.desc"] = ("Data digunakan untuk menjalankan permainan yang dicatat melalui aplikasi pendamping, menerapkan aturan, menampilkan hasil, menghasilkan analitika pembelajaran, menjaga keamanan, dan memperbaiki aplikasi.", "Data is used to run gameplay recorded through the companion application, enforce rules, present results, produce learning analytics, maintain security, and improve the application.");
        // Memperbarui `terms[”privacy.access.title”]` menggunakan tuple yang membawa bagian 1: ”Akses dan kerahasiaan”; bagian 2: ”Access and
        // confidentiality” dalam AddCore.
        terms["privacy.access.title"] = ("Akses dan kerahasiaan", "Access and confidentiality");
        // Memperbarui `terms[”privacy.access.desc”]` menggunakan tuple yang membawa bagian 1: ”Akses mengikuti peran. Instruktur dapat mengelola sesi dan
        // melihat hasil pesert...; bagian 2: ”Access follows user roles. Instructors can manage sessions and view participant... dalam AddCore.
        terms["privacy.access.desc"] = ("Akses mengikuti peran. Instruktur dapat mengelola sesi dan melihat hasil peserta, sedangkan pemain hanya memperoleh data yang diizinkan untuk dirinya. Misi rahasia pemain lain tidak dibuka.", "Access follows user roles. Instructors can manage sessions and view participant results, while players receive only data permitted for them. Other players' secret missions are not disclosed.");
        // Memperbarui `terms[”privacy.retention.title”]` menggunakan tuple yang membawa bagian 1: ”Lama penyimpanan”; bagian 2: ”Retention period” dalam
        // AddCore.
        terms["privacy.retention.title"] = ("Lama penyimpanan", "Retention period");
        // Memperbarui `terms[”privacy.retention.desc”]` menggunakan tuple yang membawa bagian 1: ”Data akun dan permainan disimpan selama proyek berjalan.
        // Proyek ini belum menye...; bagian 2: ”Account and gameplay data are retained while the project is running. This proje... dalam AddCore.
        terms["privacy.retention.desc"] = ("Data akun dan permainan disimpan selama proyek berjalan. Proyek ini belum menyediakan fitur mandiri untuk meminta salinan atau menghapus data.", "Account and gameplay data are retained while the project is running. This project does not currently provide self-service features to request a copy of or delete data.");
        // Memperbarui `terms[”privacy.security.title”]` menggunakan tuple yang membawa bagian 1: ”Perlindungan data”; bagian 2: ”Data protection” dalam
        // AddCore.
        terms["privacy.security.title"] = ("Perlindungan data", "Data protection");
        // Memperbarui `terms[”privacy.security.desc”]` menggunakan tuple yang membawa bagian 1: ”Aplikasi memakai kontrol akses berbasis peran,
        // autentikasi, validasi input, kon...; bagian 2: ”The application uses role-based access control, authentication, input validatio... dalam AddCore.
        terms["privacy.security.desc"] = ("Aplikasi memakai kontrol akses berbasis peran, autentikasi, validasi input, koneksi produksi terenkripsi, dan pencatatan operasional terbatas untuk mengurangi risiko akses yang tidak sah.", "The application uses role-based access control, authentication, input validation, encrypted production connections, and limited operational logging to reduce unauthorized-access risks.");
        // Memperbarui `terms[”privacy.limitations.title”]` menggunakan tuple yang membawa bagian 1: ”Batas layanan”; bagian 2: ”Service limitations” dalam
        // AddCore.
        terms["privacy.limitations.title"] = ("Batas layanan", "Service limitations");
        // Memperbarui `terms[”privacy.limitations.desc”]` menggunakan tuple yang membawa bagian 1: ”Karena ini proyek akademik, layanan dapat berubah atau
        // dihentikan. Tidak tersed...; bagian 2: ”Because this is an academic project, the service may change or stop. It does no... dalam AddCore.
        terms["privacy.limitations.desc"] = ("Karena ini proyek akademik, layanan dapat berubah atau dihentikan. Tidak tersedia layanan kontak, petugas perlindungan data, maupun proses formal permintaan data seperti pada layanan komersial.", "Because this is an academic project, the service may change or stop. It does not provide a contact service, a data protection officer, or a formal data-request process like a commercial service.");
        // Memperbarui `terms[”terms.kicker”]` menggunakan tuple yang membawa bagian 1: ”Aturan penggunaan”; bagian 2: ”Usage rules” dalam AddCore.
        terms["terms.kicker"] = ("Aturan penggunaan", "Usage rules");
        // Memperbarui `terms[”terms.title”]` menggunakan tuple yang membawa bagian 1: ”Ketentuan Penggunaan”; bagian 2: ”Terms of Use” dalam AddCore.
        terms["terms.title"] = ("Ketentuan Penggunaan", "Terms of Use");
        // Memperbarui `terms[”terms.intro”]` menggunakan tuple yang membawa bagian 1: ”Dengan memakai aplikasi ini, pengguna memahami tujuan akademik,
        // peran masing-ma...; bagian 2: ”By using this application, users acknowledge its academic purpose, their assign... dalam AddCore.
        terms["terms.intro"] = ("Dengan memakai aplikasi ini, pengguna memahami tujuan akademik, peran masing-masing, dan tanggung jawab saat mencatat permainan.", "By using this application, users acknowledge its academic purpose, their assigned roles, and their responsibilities when recording gameplay.");
        // Memperbarui `terms[”terms.purpose.title”]` menggunakan tuple yang membawa bagian 1: ”Tujuan aplikasi”; bagian 2: ”Application purpose” dalam
        // AddCore.
        terms["terms.purpose.title"] = ("Tujuan aplikasi", "Application purpose");
        // Memperbarui `terms[”terms.purpose.desc”]` menggunakan tuple yang membawa bagian 1: ”Aplikasi mendukung pencatatan dan analitika permainan fisik
        // Cashflowpoly. Aplik...; bagian 2: ”The application supports recording and analytics for the physical Cashflowpoly ... dalam AddCore.
        terms["terms.purpose.desc"] = ("Aplikasi mendukung pencatatan dan analitika permainan fisik Cashflowpoly. Aplikasi bukan permainan kartu virtual dan bukan layanan keuangan, investasi, atau konsultasi profesional.", "The application supports recording and analytics for the physical Cashflowpoly game. It is not a virtual card game or a financial, investment, or professional advisory service.");
        // Memperbarui `terms[”terms.roles.title”]` menggunakan tuple yang membawa bagian 1: ”Peran pengguna”; bagian 2: ”User roles” dalam AddCore.
        terms["terms.roles.title"] = ("Peran pengguna", "User roles");
        // Memperbarui `terms[”terms.roles.desc”]` menggunakan tuple yang membawa bagian 1: ”Instruktur bertanggung jawab memilih set aturan, menyiapkan
        // sesi, mengonfirmasi...; bagian 2: ”Instructors are responsible for selecting rulesets, preparing sessions, confirm... dalam AddCore.
        terms["terms.roles.desc"] = ("Instruktur bertanggung jawab memilih set aturan, menyiapkan sesi, mengonfirmasi setup fisik, dan mengawasi pencatatan. Pemain menggunakan aksesnya untuk melihat sesi serta data yang diizinkan.", "Instructors are responsible for selecting rulesets, preparing sessions, confirming the physical setup, and supervising records. Players use their access to view permitted sessions and data.");
        // Memperbarui `terms[”terms.account.title”]` menggunakan tuple yang membawa bagian 1: ”Tanggung jawab akun”; bagian 2: ”Account responsibility”
        // dalam AddCore.
        terms["terms.account.title"] = ("Tanggung jawab akun", "Account responsibility");
        // Memperbarui `terms[”terms.account.desc”]` menggunakan tuple yang membawa bagian 1: ”Pengguna harus memberikan data yang benar, menjaga kata
        // sandi, memakai akun ses...; bagian 2: ”Users must provide accurate data, protect their passwords, use accounts accordi... dalam AddCore.
        terms["terms.account.desc"] = ("Pengguna harus memberikan data yang benar, menjaga kata sandi, memakai akun sesuai peran, dan tidak mencoba mengakses data atau fungsi yang tidak diizinkan.", "Users must provide accurate data, protect their passwords, use accounts according to their roles, and not attempt to access unauthorized data or functions.");
        // Memperbarui `terms[”terms.demo.title”]` menggunakan tuple yang membawa bagian 1: ”Akun demo”; bagian 2: ”Demo accounts” dalam AddCore.
        terms["terms.demo.title"] = ("Akun demo", "Demo accounts");
        // Memperbarui `terms[”terms.demo.desc”]` menggunakan tuple yang membawa bagian 1: ”Akun Seed 2 merupakan akun demo dengan kata sandi tetap dan
        // akses penuh sesuai ...; bagian 2: ”Seed 2 accounts are demo accounts with fixed passwords and full access for thei... dalam AddCore.
        terms["terms.demo.desc"] = ("Akun Seed 2 merupakan akun demo dengan kata sandi tetap dan akses penuh sesuai perannya. Data pada akun demo dapat dilihat atau diubah oleh pengguna lain yang memiliki kredensial yang sama.", "Seed 2 accounts are demo accounts with fixed passwords and full access for their roles. Demo-account data may be viewed or changed by other users who have the same credentials.");
        // Memperbarui `terms[”terms.use.title”]` menggunakan tuple yang membawa bagian 1: ”Penggunaan yang diperbolehkan”; bagian 2: ”Acceptable use” dalam
        // AddCore.
        terms["terms.use.title"] = ("Penggunaan yang diperbolehkan", "Acceptable use");
        // Memperbarui `terms[”terms.use.desc”]` menggunakan tuple yang membawa bagian 1: ”Gunakan aplikasi untuk pembelajaran, pengujian, dan pencatatan
        // permainan yang s...; bagian 2: ”Use the application for learning, testing, and legitimate gameplay records. Do ... dalam AddCore.
        terms["terms.use.desc"] = ("Gunakan aplikasi untuk pembelajaran, pengujian, dan pencatatan permainan yang sah. Jangan mengganggu layanan, menyalahgunakan akun, memasukkan data berbahaya, atau mencoba melewati validasi keamanan.", "Use the application for learning, testing, and legitimate gameplay records. Do not disrupt the service, misuse accounts, submit harmful data, or attempt to bypass security validation.");
        // Memperbarui `terms[”terms.guarantee.title”]` menggunakan tuple yang membawa bagian 1: ”Tidak ada jaminan”; bagian 2: ”No warranty” dalam AddCore.
        terms["terms.guarantee.title"] = ("Tidak ada jaminan", "No warranty");
        // Memperbarui `terms[”terms.guarantee.desc”]` menggunakan tuple yang membawa bagian 1: ”Aplikasi disediakan untuk kebutuhan akademik sebagaimana
        // adanya. Ketepatan data...; bagian 2: ”The application is provided as-is for academic use. Data accuracy still depends... dalam AddCore.
        terms["terms.guarantee.desc"] = ("Aplikasi disediakan untuk kebutuhan akademik sebagaimana adanya. Ketepatan data tetap bergantung pada pencatatan setup dan kejadian fisik yang benar oleh pengguna.", "The application is provided as-is for academic use. Data accuracy still depends on users correctly recording the physical setup and events.");
        // Memperbarui `terms[”terms.termination.title”]` menggunakan tuple yang membawa bagian 1: ”Perubahan, penghentian, dan data”; bagian 2: ”Changes,
        // discontinuation, and data” dalam AddCore.
        terms["terms.termination.title"] = ("Perubahan, penghentian, dan data", "Changes, discontinuation, and data");
        // Memperbarui `terms[”terms.termination.desc”]` menggunakan tuple yang membawa bagian 1: ”Akses dapat dibatasi jika terjadi penyalahgunaan. Fitur,
        // aturan, dan layanan da...; bagian 2: ”Access may be restricted following misuse. Features, rules, and services may ch... dalam AddCore.
        terms["terms.termination.desc"] = ("Akses dapat dibatasi jika terjadi penyalahgunaan. Fitur, aturan, dan layanan dapat diubah atau dihentikan selama pengembangan. Data akun dan permainan dapat tetap disimpan selama proyek berjalan sebagaimana dijelaskan dalam Kebijakan Privasi.", "Access may be restricted following misuse. Features, rules, and services may change or stop during development. Account and gameplay data may remain stored while the project runs as described in the Privacy Policy.");
        // Memperbarui `terms[”components.total_components”]` menggunakan tuple yang membawa bagian 1: ”Total Komponen”; bagian 2: ”Total Components” dalam
        // AddCore.
        terms["components.total_components"] = ("Total Komponen", "Total Components");
        // Memperbarui `terms[”home.badge”]` menggunakan tuple yang membawa bagian 1: ”Cashflowpoly”; bagian 2: ”Cashflowpoly” dalam AddCore.
        terms["home.badge"] = ("Cashflowpoly", "Cashflowpoly");
        // Memperbarui `terms[”home.title”]` menggunakan tuple yang membawa bagian 1: ”Dashboard Analitika Cashflowpoly.”; bagian 2: ”Cashflowpoly Analytics
        // Dashboard.” dalam AddCore.
        terms["home.title"] = ("Dashboard Analitika Cashflowpoly.", "Cashflowpoly Analytics Dashboard.");
        // Memperbarui `terms[”home.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Pantau sesi, performa pemain, aktivitas permainan, dan aturan
        // aktif dari satu t...; bagian 2: ”Monitor sessions, player performance, gameplay activity, and active rules in on... dalam AddCore.
        terms["home.subtitle"] = ("Pantau sesi, performa pemain, aktivitas permainan, dan aturan aktif dari satu tempat.", "Monitor sessions, player performance, gameplay activity, and active rules in one place.");
        // Memperbarui `terms[”home.player_badge”]` menggunakan tuple yang membawa bagian 1: ”Dasbor Pemain”; bagian 2: ”Player Dashboard” dalam AddCore.
        terms["home.player_badge"] = ("Dasbor Pemain", "Player Dashboard");
        // Memperbarui `terms[”home.player_title”]` menggunakan tuple yang membawa bagian 1: ”Pantau progres permainan Anda secara waktu nyata.”; bagian 2:
        // ”Track your game progress in real time.” dalam AddCore.
        terms["home.player_title"] = ("Pantau progres permainan Anda secara waktu nyata.", "Track your game progress in real time.");
        // Memperbarui `terms[”home.player_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Lihat status sesi dan ringkasan performa pribadi; Set
        // Aturan aktif tersedia dar...; bagian 2: ”View session status and your personal performance summary; the active ruleset i... dalam AddCore.
        terms["home.player_subtitle"] = ("Lihat status sesi dan ringkasan performa pribadi; Set Aturan aktif tersedia dari Rincian Sesi.", "View session status and your personal performance summary; the active ruleset is available from Session Details.");
        // Memperbarui `terms[”home.cta.sessions”]` menggunakan tuple yang membawa bagian 1: ”Buka Sesi”; bagian 2: ”Open Sessions” dalam AddCore.
        terms["home.cta.sessions"] = ("Buka Sesi", "Open Sessions");
        // Memperbarui `terms[”home.cta.my_progress”]` menggunakan tuple yang membawa bagian 1: ”Lihat progres saya”; bagian 2: ”View my progress” dalam
        // AddCore.
        terms["home.cta.my_progress"] = ("Lihat progres saya", "View my progress");
        // Memperbarui `terms[”home.cta.open_ruleset”]` menggunakan tuple yang membawa bagian 1: ”Buka Set Aturan”; bagian 2: ”Open rulesets” dalam AddCore.
        terms["home.cta.open_ruleset"] = ("Buka Set Aturan", "Open rulesets");
        // Memperbarui `terms[”home.cta.session_ruleset”]` menggunakan tuple yang membawa bagian 1: ”Lihat Aturan di Sesi”; bagian 2: ”View ruleset in
        // sessions” dalam AddCore.
        terms["home.cta.session_ruleset"] = ("Lihat Aturan di Sesi", "View ruleset in sessions");
        // Memperbarui `terms[”home.total_sessions”]` menggunakan tuple yang membawa bagian 1: ”Total Sesi”; bagian 2: ”Total Sessions” dalam AddCore.
        terms["home.total_sessions"] = ("Total Sesi", "Total Sessions");
        // Memperbarui `terms[”home.my_sessions”]` menggunakan tuple yang membawa bagian 1: ”Sesi saya”; bagian 2: ”My sessions” dalam AddCore.
        terms["home.my_sessions"] = ("Sesi saya", "My sessions");
        // Memperbarui `terms[”home.players_in_my_sessions”]` menggunakan tuple yang membawa bagian 1: ”Pemain dalam sesi saya”; bagian 2: ”Players in my
        // sessions” dalam AddCore.
        terms["home.players_in_my_sessions"] = ("Pemain dalam sesi saya", "Players in my sessions");
        // Memperbarui `terms[”home.available_rulesets”]` menggunakan tuple yang membawa bagian 1: ”Set Aturan tersedia”; bagian 2: ”Available rulesets”
        // dalam AddCore.
        terms["home.available_rulesets"] = ("Set Aturan tersedia", "Available rulesets");
        // Memperbarui `terms[”home.active_sessions”]` menggunakan tuple yang membawa bagian 1: ”Sesi Berjalan”; bagian 2: ”Active Sessions” dalam AddCore.
        terms["home.active_sessions"] = ("Sesi Berjalan", "Active Sessions");
        // Memperbarui `terms[”home.auto_refresh_note”]` menggunakan tuple yang membawa bagian 1: ”Data diperbarui otomatis setiap 30 detik.”; bagian 2:
        // ”Data refreshes automatically every 30 seconds.” dalam AddCore.
        terms["home.auto_refresh_note"] = ("Data diperbarui otomatis setiap 30 detik.", "Data refreshes automatically every 30 seconds.");
        // Memperbarui `terms[”home.empty_workspace.title”]` menggunakan tuple yang membawa bagian 1: ”Workspace masih kosong”; bagian 2: ”Workspace is
        // still empty” dalam AddCore.
        terms["home.empty_workspace.title"] = ("Workspace masih kosong", "Workspace is still empty");
        // Memperbarui `terms[”home.empty_workspace.instructor”]` menggunakan tuple yang membawa bagian 1: ”Data belum tersedia. Buat dan jalankan sesi
        // permainan agar dasbor analitika mul...; bagian 2: ”No data is available yet. Create and run a gameplay session to populate the ana... dalam
        // AddCore.
        terms["home.empty_workspace.instructor"] = ("Data belum tersedia. Buat dan jalankan sesi permainan agar dasbor analitika mulai terisi.", "No data is available yet. Create and run a gameplay session to populate the analytics dashboard.");
        // Memperbarui `terms[”home.empty_workspace.player”]` menggunakan tuple yang membawa bagian 1: ”Belum ada data sesi untuk akun Anda. Tunggu
        // instruktur membuka sesi lalu cek pr...; bagian 2: ”No session data is available for your account yet. Wait for the instructor to o... dalam
        // AddCore.
        terms["home.empty_workspace.player"] = ("Belum ada data sesi untuk akun Anda. Tunggu instruktur membuka sesi lalu cek progres Anda di menu Sesi.", "No session data is available for your account yet. Wait for the instructor to open a session, then review your progress in Sessions.");
        // Memperbarui `terms[”home.error.realtime_status”]` menggunakan tuple yang membawa bagian 1: ”Gagal sinkron waktu nyata ({status}).”; bagian 2:
        // ”Realtime sync failed ({status}).” dalam AddCore.
        terms["home.error.realtime_status"] = ("Gagal sinkron waktu nyata ({status}).", "Realtime sync failed ({status}).");
        // Memperbarui `terms[”home.error.realtime_network”]` menggunakan tuple yang membawa bagian 1: ”Gagal sinkron waktu nyata (kesalahan jaringan).”;
        // bagian 2: ”Realtime sync failed (network error).” dalam AddCore.
        terms["home.error.realtime_network"] = ("Gagal sinkron waktu nyata (kesalahan jaringan).", "Realtime sync failed (network error).");
        // Memperbarui `terms[”home.error.partial_realtime_failed”]` menggunakan tuple yang membawa bagian 1: ”Sebagian data waktu nyata gagal dimuat
        // ({details}).”; bagian 2: ”Some realtime data failed to load ({details}).” dalam AddCore.
        terms["home.error.partial_realtime_failed"] = ("Sebagian data waktu nyata gagal dimuat ({details}).", "Some realtime data failed to load ({details}).");
        // Memperbarui `terms[”home.ruleset_active”]` menggunakan tuple yang membawa bagian 1: ”Set Aturan Aktif”; bagian 2: ”Active Rulesets” dalam
        // AddCore.
        terms["home.ruleset_active"] = ("Set Aturan Aktif", "Active Rulesets");
        // Memperbarui `terms[”home.players_monitored”]` menggunakan tuple yang membawa bagian 1: ”Pemain Dipantau”; bagian 2: ”Players Monitored” dalam
        // AddCore.
        terms["home.players_monitored"] = ("Pemain Dipantau", "Players Monitored");
        // Memperbarui `terms[”home.guide.menu”]` menggunakan tuple yang membawa bagian 1: ”Panduan Menu”; bagian 2: ”Menu Guide” dalam AddCore.
        terms["home.guide.menu"] = ("Panduan Menu", "Menu Guide");
        // Memperbarui `terms[”home.guide.title”]` menggunakan tuple yang membawa bagian 1: ”Kumpulan cara penggunaan”; bagian 2: ”Usage playbook” dalam
        // AddCore.
        terms["home.guide.title"] = ("Kumpulan cara penggunaan", "Usage playbook");
        // Memperbarui `terms[”home.guide.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Pilih tujuan Anda untuk langsung menuju aturan, sesi, atau
        // analitika pemain yan...; bagian 2: ”Choose your goal to go directly to the rules, session, or player analytics you ... dalam AddCore.
        terms["home.guide.subtitle"] = ("Pilih tujuan Anda untuk langsung menuju aturan, sesi, atau analitika pemain yang dibutuhkan.", "Choose your goal to go directly to the rules, session, or player analytics you need.");
        // Memperbarui `terms[”home.guide.sessions.title”]` menggunakan tuple yang membawa bagian 1: ”Sesi”; bagian 2: ”Sessions” dalam AddCore.
        terms["home.guide.sessions.title"] = ("Sesi", "Sessions");
        // Memperbarui `terms[”home.guide.sessions.desc”]` menggunakan tuple yang membawa bagian 1: ”Periksa status, ringkasan hasil, Set Aturan aktif, dan
        // urutan aktivitas pada sa...; bagian 2: ”Review status, result summaries, the active ruleset, and the activity sequence ... dalam AddCore.
        terms["home.guide.sessions.desc"] = ("Periksa status, ringkasan hasil, Set Aturan aktif, dan urutan aktivitas pada satu sesi.", "Review status, result summaries, the active ruleset, and the activity sequence for a session.");
        // Memperbarui `terms[”home.guide.sessions.link”]` menggunakan tuple yang membawa bagian 1: ”Buka menu Sesi”; bagian 2: ”Open Sessions” dalam
        // AddCore.
        terms["home.guide.sessions.link"] = ("Buka menu Sesi", "Open Sessions");
        // Memperbarui `terms[”home.guide.ruleset.title”]` menggunakan tuple yang membawa bagian 1: ”Set Aturan”; bagian 2: ”Rulesets” dalam AddCore.
        terms["home.guide.ruleset.title"] = ("Set Aturan", "Rulesets");
        // Memperbarui `terms[”home.guide.ruleset.desc”]` menggunakan tuple yang membawa bagian 1: ”Bandingkan mode, versi, target, dan fitur aktif sebelum
        // menafsirkan hasil perma...; bagian 2: ”Compare mode, version, targets, and enabled features before interpreting gamepl... dalam AddCore.
        terms["home.guide.ruleset.desc"] = ("Bandingkan mode, versi, target, dan fitur aktif sebelum menafsirkan hasil permainan.", "Compare mode, version, targets, and enabled features before interpreting gameplay results.");
        // Memperbarui `terms[”home.guide.ruleset.link”]` menggunakan tuple yang membawa bagian 1: ”Buka menu Set Aturan”; bagian 2: ”Open Rulesets” dalam
        // AddCore.
        terms["home.guide.ruleset.link"] = ("Buka menu Set Aturan", "Open Rulesets");
        // Memperbarui `terms[”home.guide.players.title”]` menggunakan tuple yang membawa bagian 1: ”Pemain”; bagian 2: ”Players” dalam AddCore.
        terms["home.guide.players.title"] = ("Pemain", "Players");
        // Memperbarui `terms[”home.guide.players.desc”]` menggunakan tuple yang membawa bagian 1: ”Lihat peserta per sesi, lalu buka Analitika pemain untuk
        // membahas hasil individ...; bagian 2: ”View participants by session, then open player Analytics to discuss individual ... dalam AddCore.
        terms["home.guide.players.desc"] = ("Lihat peserta per sesi, lalu buka Analitika pemain untuk membahas hasil individunya.", "View participants by session, then open player Analytics to discuss individual results.");
        // Memperbarui `terms[”home.guide.players.link”]` menggunakan tuple yang membawa bagian 1: ”Buka menu Pemain”; bagian 2: ”Open Players” dalam
        // AddCore.
        terms["home.guide.players.link"] = ("Buka menu Pemain", "Open Players");
        // Memperbarui `terms[”home.quick_flow.title”]` menggunakan tuple yang membawa bagian 1: ”Panduan penggunaan”; bagian 2: ”Usage guide” dalam
        // AddCore.
        terms["home.quick_flow.title"] = ("Panduan penggunaan", "Usage guide");
        // Memperbarui `terms[”home.quick_flow.step1”]` menggunakan tuple yang membawa bagian 1: ”Siapkan sesi dan pilih Set Aturan yang sesuai dengan
        // tujuan permainan.”; bagian 2: ”Prepare a session and choose the ruleset that matches the gameplay objective.” dalam AddCore.
        terms["home.quick_flow.step1"] = ("Siapkan sesi dan pilih Set Aturan yang sesuai dengan tujuan permainan.", "Prepare a session and choose the ruleset that matches the gameplay objective.");
        // Memperbarui `terms[”home.quick_flow.step2”]` menggunakan tuple yang membawa bagian 1: ”Tempatkan pemain pada urutan yang benar, lalu mulai
        // sesi.”; bagian 2: ”Place players in the correct order, then start the session.” dalam AddCore.
        terms["home.quick_flow.step2"] = ("Tempatkan pemain pada urutan yang benar, lalu mulai sesi.", "Place players in the correct order, then start the session.");
        // Memperbarui `terms[”home.quick_flow.step3”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas permainan dicatat menjadi urutan kejadian,
        // perubahan uang, dan hasil ...; bagian 2: ”Gameplay activity is recorded as an event sequence, money changes, and player r... dalam AddCore.
        terms["home.quick_flow.step3"] = ("Aktivitas permainan dicatat menjadi urutan kejadian, perubahan uang, dan hasil pemain.", "Gameplay activity is recorded as an event sequence, money changes, and player results.");
        // Memperbarui `terms[”home.quick_flow.step4”]` menggunakan tuple yang membawa bagian 1: ”Buka Analitika Sesi untuk membaca aktivitas, arus kas,
        // performa, dan pola keput...; bagian 2: ”Open Session Analytics to review activity, cashflow, performance, and decision ... dalam AddCore.
        terms["home.quick_flow.step4"] = ("Buka Analitika Sesi untuk membaca aktivitas, arus kas, performa, dan pola keputusan.", "Open Session Analytics to review activity, cashflow, performance, and decision patterns.");
        // Memperbarui `terms[”home.quick_flow.step5”]` menggunakan tuple yang membawa bagian 1: ”Buka Analitika Pemain untuk membahas keputusan dan hasil
        // tiap individu.”; bagian 2: ”Open Player Analytics to discuss each individual's decisions and results.” dalam AddCore.
        terms["home.quick_flow.step5"] = ("Buka Analitika Pemain untuk membahas keputusan dan hasil tiap individu.", "Open Player Analytics to discuss each individual's decisions and results.");
        // Memperbarui `terms[”home.player_guide.menu”]` menggunakan tuple yang membawa bagian 1: ”Panduan Pemain”; bagian 2: ”Player Guide” dalam AddCore.
        terms["home.player_guide.menu"] = ("Panduan Pemain", "Player Guide");
        // Memperbarui `terms[”home.player_guide.title”]` menggunakan tuple yang membawa bagian 1: ”Langkah cepat melihat progres”; bagian 2: ”Quick steps
        // to view progress” dalam AddCore.
        terms["home.player_guide.title"] = ("Langkah cepat melihat progres", "Quick steps to view progress");
        // Memperbarui `terms[”home.player_guide.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Tampilan pemain difokuskan untuk membaca hasil
        // permainan Anda dengan cepat.”; bagian 2: ”The player view is focused on reading your own game results quickly.” dalam AddCore.
        terms["home.player_guide.subtitle"] = ("Tampilan pemain difokuskan untuk membaca hasil permainan Anda dengan cepat.", "The player view is focused on reading your own game results quickly.");
        // Memperbarui `terms[”home.player_guide.analytics.title”]` menggunakan tuple yang membawa bagian 1: ”Analitika Sesi Saya”; bagian 2: ”My Session
        // Analytics” dalam AddCore.
        terms["home.player_guide.analytics.title"] = ("Analitika Sesi Saya", "My Session Analytics");
        // Memperbarui `terms[”home.player_guide.analytics.desc”]` menggunakan tuple yang membawa bagian 1: ”Pilih sesi untuk melihat arus kas, poin
        // kebahagiaan, posisi permainan, dan risi...; bagian 2: ”Choose a session to review your cashflow, scores, game position, and personal r... dalam
        // AddCore.
        terms["home.player_guide.analytics.desc"] = ("Pilih sesi untuk melihat arus kas, poin kebahagiaan, posisi permainan, dan risiko pribadi Anda.", "Choose a session to review your cashflow, scores, game position, and personal risks.");
        // Memperbarui `terms[”home.player_guide.analytics.link”]` menggunakan tuple yang membawa bagian 1: ”Buka menu Sesi”; bagian 2: ”Open Sessions”
        // dalam AddCore.
        terms["home.player_guide.analytics.link"] = ("Buka menu Sesi", "Open Sessions");
        // Memperbarui `terms[”home.player_guide.ruleset.title”]` menggunakan tuple yang membawa bagian 1: ”Set Aturan Aktif”; bagian 2: ”Active Rulesets”
        // dalam AddCore.
        terms["home.player_guide.ruleset.title"] = ("Set Aturan Aktif", "Active Rulesets");
        // Memperbarui `terms[”home.player_guide.ruleset.desc”]` menggunakan tuple yang membawa bagian 1: ”Dari sesi yang dipilih, buka Set Aturan aktif
        // untuk memahami target, penalti, b...; bagian 2: ”From the selected session, open the active ruleset to understand targets, penal... dalam
        // AddCore.
        terms["home.player_guide.ruleset.desc"] = ("Dari sesi yang dipilih, buka Set Aturan aktif untuk memahami target, penalti, batas aksi, dan fitur mode.", "From the selected session, open the active ruleset to understand targets, penalties, action limits, and mode features.");
        // Memperbarui `terms[”home.player_guide.ruleset.link”]` menggunakan tuple yang membawa bagian 1: ”Pilih Sesi Permainan”; bagian 2: ”Choose a Game
        // Session” dalam AddCore.
        terms["home.player_guide.ruleset.link"] = ("Pilih Sesi Permainan", "Choose a Game Session");
        // Memperbarui `terms[”home.player_guide.rulebook.title”]` menggunakan tuple yang membawa bagian 1: ”Buku Aturan”; bagian 2: ”Rulebook” dalam
        // AddCore.
        terms["home.player_guide.rulebook.title"] = ("Buku Aturan", "Rulebook");
        // Memperbarui `terms[”home.player_guide.rulebook.desc”]` menggunakan tuple yang membawa bagian 1: ”Cari istilah, alur giliran, dan cara menghitung
        // poin kebahagiaan saat hasil per...; bagian 2: ”Look up terms, turn flow, and scoring whenever gameplay results need confirmati... dalam AddCore.
        terms["home.player_guide.rulebook.desc"] = ("Cari istilah, alur giliran, dan cara menghitung poin kebahagiaan saat hasil permainan perlu dikonfirmasi.", "Look up terms, turn flow, and scoring whenever gameplay results need confirmation.");
        // Memperbarui `terms[”home.player_guide.rulebook.link”]` menggunakan tuple yang membawa bagian 1: ”Buka Buku Aturan”; bagian 2: ”Open Rulebook”
        // dalam AddCore.
        terms["home.player_guide.rulebook.link"] = ("Buka Buku Aturan", "Open Rulebook");
        // Memperbarui `terms[”common.name”]` menggunakan tuple yang membawa bagian 1: ”Nama”; bagian 2: ”Name” dalam AddCore.
        terms["common.name"] = ("Nama", "Name");
        // Memperbarui `terms[”common.description”]` menggunakan tuple yang membawa bagian 1: ”Deskripsi”; bagian 2: ”Description” dalam AddCore.
        terms["common.description"] = ("Deskripsi", "Description");
        // Memperbarui `terms[”common.status”]` menggunakan tuple yang membawa bagian 1: ”Status”; bagian 2: ”Status” dalam AddCore.
        terms["common.status"] = ("Status", "Status");
        // Memperbarui `terms[”common.created”]` menggunakan tuple yang membawa bagian 1: ”Dibuat”; bagian 2: ”Created” dalam AddCore.
        terms["common.created"] = ("Dibuat", "Created");
        // Memperbarui `terms[”common.mode”]` menggunakan tuple yang membawa bagian 1: ”Mode”; bagian 2: ”Mode” dalam AddCore.
        terms["common.mode"] = ("Mode", "Mode");
        // Memperbarui `terms[”common.action”]` menggunakan tuple yang membawa bagian 1: ”Aksi”; bagian 2: ”Action” dalam AddCore.
        terms["common.action"] = ("Aksi", "Action");
        // Memperbarui `terms[”common.detail”]` menggunakan tuple yang membawa bagian 1: ”Rincian”; bagian 2: ”Detail” dalam AddCore.
        terms["common.detail"] = ("Rincian", "Detail");
        // Memperbarui `terms[”common.view”]` menggunakan tuple yang membawa bagian 1: ”Lihat”; bagian 2: ”View” dalam AddCore.
        terms["common.view"] = ("Lihat", "View");
        // Memperbarui `terms[”common.close”]` menggunakan tuple yang membawa bagian 1: ”Tutup”; bagian 2: ”Close” dalam AddCore.
        terms["common.close"] = ("Tutup", "Close");
        // Memperbarui `terms[”common.cancel”]` menggunakan tuple yang membawa bagian 1: ”Batal”; bagian 2: ”Cancel” dalam AddCore.
        terms["common.cancel"] = ("Batal", "Cancel");
        // Memperbarui `terms[”common.version”]` menggunakan tuple yang membawa bagian 1: ”Versi”; bagian 2: ”Version” dalam AddCore.
        terms["common.version"] = ("Versi", "Version");
        // Memperbarui `terms[”common.latest_version”]` menggunakan tuple yang membawa bagian 1: ”Versi Terbaru”; bagian 2: ”Latest Version” dalam AddCore.
        terms["common.latest_version"] = ("Versi Terbaru", "Latest Version");
        // Memperbarui `terms[”common.player”]` menggunakan tuple yang membawa bagian 1: ”Pemain”; bagian 2: ”Player” dalam AddCore.
        terms["common.player"] = ("Pemain", "Player");
        // Memperbarui `terms[”common.player_id”]` menggunakan tuple yang membawa bagian 1: ”ID Pemain”; bagian 2: ”Player ID” dalam AddCore.
        terms["common.player_id"] = ("ID Pemain", "Player ID");
        // Memperbarui `terms[”common.rank”]` menggunakan tuple yang membawa bagian 1: ”Peringkat”; bagian 2: ”Rank” dalam AddCore.
        terms["common.rank"] = ("Peringkat", "Rank");
        // Memperbarui `terms[”common.turn_order”]` menggunakan tuple yang membawa bagian 1: ”Urutan Giliran”; bagian 2: ”Turn Order” dalam AddCore.
        terms["common.turn_order"] = ("Urutan Giliran", "Turn Order");
        // Memperbarui `terms[”common.ruleset”]` menggunakan tuple yang membawa bagian 1: ”Set Aturan”; bagian 2: ”Ruleset” dalam AddCore.
        terms["common.ruleset"] = ("Set Aturan", "Ruleset");
        // Memperbarui `terms[”common.transaction_amount”]` menggunakan tuple yang membawa bagian 1: ”Nominal”; bagian 2: ”Amount” dalam AddCore.
        terms["common.transaction_amount"] = ("Nominal", "Amount");
        // Memperbarui `terms[”common.time”]` menggunakan tuple yang membawa bagian 1: ”Waktu”; bagian 2: ”Time” dalam AddCore.
        terms["common.time"] = ("Waktu", "Time");
        // Memperbarui `terms[”common.direction”]` menggunakan tuple yang membawa bagian 1: ”Arah”; bagian 2: ”Direction” dalam AddCore.
        terms["common.direction"] = ("Arah", "Direction");
        // Memperbarui `terms[”common.category”]` menggunakan tuple yang membawa bagian 1: ”Kategori”; bagian 2: ”Category” dalam AddCore.
        terms["common.category"] = ("Kategori", "Category");
        // Memperbarui `terms[”common.path”]` menggunakan tuple yang membawa bagian 1: ”Path”; bagian 2: ”Path” dalam AddCore.
        terms["common.path"] = ("Path", "Path");
        // Memperbarui `terms[”common.value”]` menggunakan tuple yang membawa bagian 1: ”Nilai”; bagian 2: ”Value” dalam AddCore.
        terms["common.value"] = ("Nilai", "Value");
        // Memperbarui `terms[”common.computed”]` menggunakan tuple yang membawa bagian 1: ”Dihitung”; bagian 2: ”Computed” dalam AddCore.
        terms["common.computed"] = ("Dihitung", "Computed");
        // Memperbarui `terms[”common.start”]` menggunakan tuple yang membawa bagian 1: ”Mulai”; bagian 2: ”Start” dalam AddCore.
        terms["common.start"] = ("Mulai", "Start");
        // Memperbarui `terms[”common.finish”]` menggunakan tuple yang membawa bagian 1: ”Selesai”; bagian 2: ”Finish” dalam AddCore.
        terms["common.finish"] = ("Selesai", "Finish");
        // Memperbarui `terms[”common.session”]` menggunakan tuple yang membawa bagian 1: ”Sesi”; bagian 2: ”Session” dalam AddCore.
        terms["common.session"] = ("Sesi", "Session");
        // Memperbarui `terms[”common.na”]` menggunakan tuple yang membawa bagian 1: ”Tidak Ada”; bagian 2: ”N/A” dalam AddCore.
        terms["common.na"] = ("Tidak Ada", "N/A");
        // Memperbarui `terms[”common.running_balance”]` menggunakan tuple yang membawa bagian 1: ”Saldo Berjalan”; bagian 2: ”Running Balance” dalam
        // AddCore.
        terms["common.running_balance"] = ("Saldo Berjalan", "Running Balance");
        // Memperbarui `terms[”error.title”]` menggunakan tuple yang membawa bagian 1: ”Terjadi Kesalahan”; bagian 2: ”An Error Occurred” dalam AddCore.
        terms["error.title"] = ("Terjadi Kesalahan", "An Error Occurred");
        // Memperbarui `terms[”error.notice”]` menggunakan tuple yang membawa bagian 1: ”Pemberitahuan Sistem”; bagian 2: ”System Notice” dalam AddCore.
        terms["error.notice"] = ("Pemberitahuan Sistem", "System Notice");
        // Memperbarui `terms[”error.message”]` menggunakan tuple yang membawa bagian 1: ”Terjadi kesalahan saat memproses permintaan.”; bagian 2: ”An error
        // occurred while processing your request.” dalam AddCore.
        terms["error.message"] = ("Terjadi kesalahan saat memproses permintaan.", "An error occurred while processing your request.");
        // Memperbarui `terms[”error.submessage”]` menggunakan tuple yang membawa bagian 1: ”Silakan coba lagi, atau cek rincian permintaan bila masalah
        // berlanjut.”; bagian 2: ”Please try again, or check request details if the issue persists.” dalam AddCore.
        terms["error.submessage"] = ("Silakan coba lagi, atau cek rincian permintaan bila masalah berlanjut.", "Please try again, or check request details if the issue persists.");
        // Memperbarui `terms[”error.request_id”]` menggunakan tuple yang membawa bagian 1: ”ID Permintaan”; bagian 2: ”Request ID” dalam AddCore.
        terms["error.request_id"] = ("ID Permintaan", "Request ID");
        // Memperbarui `terms[”error.dev_mode”]` menggunakan tuple yang membawa bagian 1: ”Mode Pengembangan”; bagian 2: ”Development Mode” dalam AddCore.
        terms["error.dev_mode"] = ("Mode Pengembangan", "Development Mode");
        // Memperbarui `terms[”error.dev_desc”]` menggunakan tuple yang membawa bagian 1: ”Mode Pengembangan menampilkan rincian kesalahan untuk debugging
        // lokal.”; bagian 2: ”Development mode shows detailed errors for local debugging.” dalam AddCore.
        terms["error.dev_desc"] = ("Mode Pengembangan menampilkan rincian kesalahan untuk debugging lokal.", "Development mode shows detailed errors for local debugging.");
        // Memperbarui `terms[”error.dev_warning”]` menggunakan tuple yang membawa bagian 1: ”Jangan aktifkan Pengembangan di produksi karena bisa
        // mengekspos informasi sensi...; bagian 2: ”Do not enable Development in production because it can expose sensitive informa... dalam AddCore.
        terms["error.dev_warning"] = ("Jangan aktifkan Pengembangan di produksi karena bisa mengekspos informasi sensitif.", "Do not enable Development in production because it can expose sensitive information.");
        // Memperbarui `terms[”error.dev_hint”]` menggunakan tuple yang membawa bagian 1: ”Untuk debugging lokal, set ASPNETCORE_ENVIRONMENT ke
        // Pengembangan.”; bagian 2: ”For local debugging, set ASPNETCORE_ENVIRONMENT to Development.” dalam AddCore.
        terms["error.dev_hint"] = ("Untuk debugging lokal, set ASPNETCORE_ENVIRONMENT ke Pengembangan.", "For local debugging, set ASPNETCORE_ENVIRONMENT to Development.");
        // Memperbarui `terms[”state.no_sessions”]` menggunakan tuple yang membawa bagian 1: ”Belum ada sesi tersedia.”; bagian 2: ”No sessions available
        // yet.” dalam AddCore.
        terms["state.no_sessions"] = ("Belum ada sesi tersedia.", "No sessions available yet.");
        // Memperbarui `terms[”state.no_players”]` menggunakan tuple yang membawa bagian 1: ”Belum ada data pemain.”; bagian 2: ”No player data yet.” dalam
        // AddCore.
        terms["state.no_players"] = ("Belum ada data pemain.", "No player data yet.");
        // Memperbarui `terms[”state.no_player_data”]` menggunakan tuple yang membawa bagian 1: ”Belum ada data pemain.”; bagian 2: ”No player data yet.”
        // dalam AddCore.
        terms["state.no_player_data"] = ("Belum ada data pemain.", "No player data yet.");
        // Memperbarui `terms[”state.no_rulesets”]` menggunakan tuple yang membawa bagian 1: ”Belum ada set aturan.”; bagian 2: ”No rulesets yet.” dalam
        // AddCore.
        terms["state.no_rulesets"] = ("Belum ada set aturan.", "No rulesets yet.");
        // Memperbarui `terms[”state.no_versions”]` menggunakan tuple yang membawa bagian 1: ”Belum ada versi.”; bagian 2: ”No versions yet.” dalam AddCore.
        terms["state.no_versions"] = ("Belum ada versi.", "No versions yet.");
        // Memperbarui `terms[”state.true”]` menggunakan tuple yang membawa bagian 1: ”Ya”; bagian 2: ”Yes” dalam AddCore.
        terms["state.true"] = ("Ya", "Yes");
        // Memperbarui `terms[”state.false”]` menggunakan tuple yang membawa bagian 1: ”Tidak”; bagian 2: ”No” dalam AddCore.
        terms["state.false"] = ("Tidak", "No");
        // Memperbarui `terms[”state.null”]` menggunakan tuple yang membawa bagian 1: ”Kosong”; bagian 2: ”Null” dalam AddCore.
        terms["state.null"] = ("Kosong", "Null");

        // SEO Localization Keys
        // Memperbarui `terms[”layout.meta_description”]` menggunakan tuple yang membawa bagian 1: ”Narafin adalah platform analitika pembelajaran literasi
        // keuangan interaktif ber...; bagian 2: ”Narafin is an interactive financial literacy learning analytics platform based ... dalam AddCore.
        terms["layout.meta_description"] = ("Narafin adalah platform analitika pembelajaran literasi keuangan interaktif berbasis simulasi game Cashflowpoly.", "Narafin is an interactive financial literacy learning analytics platform based on the Cashflowpoly game simulation.");
        // Memperbarui `terms[”layout.meta_keywords”]` menggunakan tuple yang membawa bagian 1: ”narafin, cashflowpoly, literasi keuangan, game keuangan,
        // analitika simulasi, ed...; bagian 2: ”narafin, cashflowpoly, financial literacy, finance game, simulation analytics, ... dalam AddCore.
        terms["layout.meta_keywords"] = ("narafin, cashflowpoly, literasi keuangan, game keuangan, analitika simulasi, education, technology, finance game, financial literacy, pembelajaran interaktif, interactive learning, dashboard analitika, marco marcello hugo, web developer", "narafin, cashflowpoly, financial literacy, finance game, simulation analytics, learning analytics, financial education, interactive learning, tech, technology, education, marco marcello hugo, web developer");
        // Memperbarui `terms[”home.meta_desc”]` menggunakan tuple yang membawa bagian 1: ”Pantau sesi, performa pemain, dan analitika game Cashflowpoly
        // secara waktu nyat...; bagian 2: ”Monitor sessions, player performance, and Cashflowpoly game analytics in real t... dalam AddCore.
        terms["home.meta_desc"] = ("Pantau sesi, performa pemain, dan analitika game Cashflowpoly secara waktu nyata di Narafin.", "Monitor sessions, player performance, and Cashflowpoly game analytics in real time on Narafin.");
        // Memperbarui `terms[”home.meta_keywords”]` menggunakan tuple yang membawa bagian 1: ”narafin dashboard, analitika cashflowpoly, pemantauan
        // realtime, cashflowpoly an...; bagian 2: ”narafin dashboard, cashflowpoly analytics, realtime monitoring, live session st... dalam AddCore.
        terms["home.meta_keywords"] = ("narafin dashboard, analitika cashflowpoly, pemantauan realtime, cashflowpoly analytics, live session stats, monitoring, data analytics, marco marcello hugo", "narafin dashboard, cashflowpoly analytics, realtime monitoring, live session stats, data analytics, marco marcello hugo");
        // Memperbarui `terms[”rulebook.meta_desc”]` menggunakan tuple yang membawa bagian 1: ”Buku Panduan dan Aturan resmi permainan Cashflowpoly.
        // Pelajari cara bermain, ak...; bagian 2: ”Official guide and rules of the Cashflowpoly game. Learn how to play, turn acti... dalam AddCore.
        terms["rulebook.meta_desc"] = ("Buku Panduan dan Aturan resmi permainan Cashflowpoly. Pelajari cara bermain, aksi giliran, perhitungan poin kebahagiaan, dan mode mahir.", "Official guide and rules of the Cashflowpoly game. Learn how to play, turn actions, scoring, and advanced mode.");
        // Memperbarui `terms[”rulebook.meta_keywords”]` menggunakan tuple yang membawa bagian 1: ”aturan cashflowpoly, cara bermain cashflowpoly, panduan
        // keuangan, rulebook cash...; bagian 2: ”cashflowpoly rules, how to play cashflowpoly, financial rulebook, cashflowpoly ... dalam AddCore.
        terms["rulebook.meta_keywords"] = ("aturan cashflowpoly, cara bermain cashflowpoly, panduan keuangan, rulebook cashflowpoly, cashflowpoly rules, cheat sheet, board game, marco marcello hugo", "cashflowpoly rules, how to play cashflowpoly, financial rulebook, cashflowpoly guide, cheat sheet, board game, marco marcello hugo");
        // Memperbarui `terms[”login.meta_desc”]` menggunakan tuple yang membawa bagian 1: ”Masuk ke dashboard Narafin untuk memantau data permainan,
        // progres sesi, dan dir...; bagian 2: ”Sign in to the Narafin dashboard to monitor game data, session progress, and pl... dalam AddCore.
        terms["login.meta_desc"] = ("Masuk ke dashboard Narafin untuk memantau data permainan, progres sesi, dan direktori pemain.", "Sign in to the Narafin dashboard to monitor game data, session progress, and player directory.");
        // Memperbarui `terms[”login.meta_keywords”]` menggunakan tuple yang membawa bagian 1: ”narafin login, login cashflowpoly, akses dasbor narafin,
        // narafin dashboard logi...; bagian 2: ”narafin login, cashflowpoly sign in, narafin dashboard access, sign in, marco m... dalam AddCore.
        terms["login.meta_keywords"] = ("narafin login, login cashflowpoly, akses dasbor narafin, narafin dashboard login, sign in, marco marcello hugo", "narafin login, cashflowpoly sign in, narafin dashboard access, sign in, marco marcello hugo");
        // Memperbarui `terms[”register.meta_desc”]` menggunakan tuple yang membawa bagian 1: ”Daftar akun Narafin untuk mengakses analitika sesi simulasi
        // dan evaluasi game C...; bagian 2: ”Register a Narafin account to access simulation session analytics and Cashflowp... dalam AddCore.
        terms["register.meta_desc"] = ("Daftar akun Narafin untuk mengakses analitika sesi simulasi dan evaluasi game Cashflowpoly.", "Register a Narafin account to access simulation session analytics and Cashflowpoly game evaluation.");
        // Memperbarui `terms[”register.meta_keywords”]` menggunakan tuple yang membawa bagian 1: ”daftar narafin, registrasi cashflowpoly, buat akun
        // narafin, register narafin, s...; bagian 2: ”narafin register, cashflowpoly sign up, create narafin account, sign up, marco ... dalam AddCore.
        terms["register.meta_keywords"] = ("daftar narafin, registrasi cashflowpoly, buat akun narafin, register narafin, sign up, marco marcello hugo", "narafin register, cashflowpoly sign up, create narafin account, sign up, marco marcello hugo");
        // Memperbarui `terms[”privacy.meta_desc”]` menggunakan tuple yang membawa bagian 1: ”Penjelasan data akun, permainan, akses, penyimpanan, dan batas
        // layanan pada pro...; bagian 2: ”An explanation of account data, gameplay data, access, retention, and service l... dalam AddCore.
        terms["privacy.meta_desc"] = ("Penjelasan data akun, permainan, akses, penyimpanan, dan batas layanan pada proyek akademik Cashflowpoly.", "An explanation of account data, gameplay data, access, retention, and service limitations in the Cashflowpoly academic project.");
        // Memperbarui `terms[”privacy.meta_keywords”]` menggunakan tuple yang membawa bagian 1: ”kebijakan privasi cashflowpoly, data permainan, proyek
        // akademik”; bagian 2: ”cashflowpoly privacy policy, gameplay data, academic project” dalam AddCore.
        terms["privacy.meta_keywords"] = ("kebijakan privasi cashflowpoly, data permainan, proyek akademik", "cashflowpoly privacy policy, gameplay data, academic project");
        // Memperbarui `terms[”terms.meta_desc”]` menggunakan tuple yang membawa bagian 1: ”Ketentuan penggunaan, peran, tanggung jawab akun, dan batas
        // layanan proyek akad...; bagian 2: ”Terms of use, roles, account responsibilities, and service limitations for the ... dalam AddCore.
        terms["terms.meta_desc"] = ("Ketentuan penggunaan, peran, tanggung jawab akun, dan batas layanan proyek akademik Cashflowpoly.", "Terms of use, roles, account responsibilities, and service limitations for the Cashflowpoly academic project.");
        // Memperbarui `terms[”terms.meta_keywords”]` menggunakan tuple yang membawa bagian 1: ”ketentuan penggunaan cashflowpoly, aturan akun, proyek
        // akademik”; bagian 2: ”cashflowpoly terms of use, account rules, academic project” dalam AddCore.
        terms["terms.meta_keywords"] = ("ketentuan penggunaan cashflowpoly, aturan akun, proyek akademik", "cashflowpoly terms of use, account rules, academic project");
    // Menutup scope metode AddCore; bagian berikut berada di luar batas blok tersebut dalam AddCore.
    }
// Menutup scope tipe UiTextLexicon; bagian berikut berada di luar batas blok tersebut.
}
