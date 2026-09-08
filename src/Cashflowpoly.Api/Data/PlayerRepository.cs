// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk PlayerRepository.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository domain pemain dengan sumber identitas app_users role PLAYER.
/// </summary>
// Mendefinisikan tipe class `PlayerRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerRepository
// Membuka scope tipe PlayerRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;

    // Mendefinisikan konstruktor PlayerRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public PlayerRepository(NpgsqlDataSource dataSource)
    // Membuka scope konstruktor PlayerRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PlayerRepository.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // PlayerRepository.
        _dataSource = dataSource;
    // Menutup scope konstruktor PlayerRepository; bagian berikut berada di luar batas blok tersebut dalam PlayerRepository.
    }

    // Mendefinisikan metode `GetPlayerAsync` dengan hasil bertipe `Task<PlayerDb?>`; operasi ini menangani get pemain asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `userId` bertipe `Guid` membawa
    // identitas akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<PlayerDb?> GetPlayerAsync(Guid userId, CancellationToken ct)
    // Membuka scope metode GetPlayerAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetPlayerAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `username,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `display_name,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null::uuid as
        // instructor_user_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `role,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
        // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where user_id = @userId`.
        // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and role = 'PLAYER'`.
        // Baris literal 13: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                user_id,
                username,
                display_name,
                null::uuid as instructor_user_id,
                role,
                is_active,
                created_at
            from app_users
            where user_id = @userId
              and role = 'PLAYER'
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<PlayerDb>` dengan `new
        // CommandDefinition(sql, new { userId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetPlayerAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(new CommandDefinition(sql, new { userId }, cancellationToken: ct));
    // Menutup scope metode GetPlayerAsync; bagian berikut berada di luar batas blok tersebut dalam GetPlayerAsync.
    }

    // Mendefinisikan metode `GetPlayerByUsernameAsync` dengan hasil bertipe `Task<PlayerDb?>`; operasi ini menangani get pemain berdasarkan username
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `username`
    // bertipe `string` membawa nama akun yang dipakai saat autentikasi; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<PlayerDb?> GetPlayerByUsernameAsync(string username, CancellationToken ct)
    // Membuka scope metode GetPlayerByUsernameAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetPlayerByUsernameAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `username,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `display_name,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null::uuid as
        // instructor_user_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `role,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
        // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where username = @username`.
        // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and role = 'PLAYER'`.
        // Baris literal 13: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 14: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                user_id,
                username,
                display_name,
                null::uuid as instructor_user_id,
                role,
                is_active,
                created_at
            from app_users
            where username = @username
              and role = 'PLAYER'
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<PlayerDb>` dengan `new
        // CommandDefinition(sql, new { username }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetPlayerByUsernameAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { username }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<PlayerDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek
            // anonim yang mengelompokkan username sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan
            // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { username }, cancellationToken: ct));
    // Menutup scope metode GetPlayerByUsernameAsync; bagian berikut berada di luar batas blok tersebut dalam GetPlayerByUsernameAsync.
    }

    // Mendefinisikan metode `ListPlayersAsync` dengan hasil bertipe `Task<List<PlayerDb>>`; operasi ini menangani daftar pemain asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<PlayerDb>> ListPlayersAsync(CancellationToken ct, Guid? instructorUserId = null)
    // Membuka scope metode ListPlayersAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListPlayersAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `username,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `display_name,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null::uuid as
        // instructor_user_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `role,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
        // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where role = 'PLAYER'`.
        // Baris literal 12: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by created_at desc`.
        // Baris literal 13: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                user_id,
                username,
                display_name,
                null::uuid as instructor_user_id,
                role,
                is_active,
                created_at
            from app_users
            where role = 'PLAYER'
              and (
                  cast(@instructorUserId as uuid) is null
                  or exists (
                      select 1
                      from session_participants sp
                      join sessions s on s.session_id = sp.session_id
                      where sp.user_id = app_users.user_id
                        and s.instructor_user_id = @instructorUserId
                        and not s.is_archived
                  )
              )
            order by created_at desc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<PlayerDb>(new CommandDefinition(sql, new { instructorUserId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListPlayersAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListPlayersAsync; bagian berikut berada di luar batas blok tersebut dalam ListPlayersAsync.
    }

    // Mendefinisikan metode `ListPlayersByPlayerScopeAsync` dengan hasil bertipe `Task<List<PlayerDb>>`; operasi ini menangani daftar pemain
    // berdasarkan pemain cakupan asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<PlayerDb>> ListPlayersByPlayerScopeAsync(Guid userId, CancellationToken ct)
    // Membuka scope metode ListPlayersByPlayerScopeAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ListPlayersByPlayerScopeAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select distinct`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `u.user_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `u.username,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `u.display_name,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null::uuid as
        // instructor_user_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `u.role,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `u.is_active,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `u.created_at`.
        // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from app_users u`.
        // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where u.role = 'PLAYER'`.
        // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `u.user_id = @userId`.
        // Baris literal 14: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or exists (`.
        // Baris literal 15: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 16: FROM memilih tabel/subquery sumber pembacaan: `from session_participants me`.
        // Baris literal 17: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants peer on peer.session_id =
        // me.session_id`.
        // Baris literal 18: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where me.user_id = @userId`.
        // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and peer.user_id = u.user_id`.
        // Baris literal 20: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 22: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by u.display_name asc, u.username asc`.
        // Baris literal 23: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select distinct
                u.user_id,
                u.username,
                u.display_name,
                null::uuid as instructor_user_id,
                u.role,
                u.is_active,
                u.created_at
            from app_users u
            where u.role = 'PLAYER'
              and (
                  u.user_id = @userId
                  or exists (
                      select 1
                      from session_participants me
                      join session_participants peer on peer.session_id = me.session_id
                      where me.user_id = @userId
                        and peer.user_id = u.user_id
                  )
              )
            order by u.display_name asc, u.username asc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { userId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<PlayerDb>(new CommandDefinition(sql, new { userId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListPlayersByPlayerScopeAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListPlayersByPlayerScopeAsync; bagian berikut berada di luar batas blok tersebut dalam ListPlayersByPlayerScopeAsync.
    }

    // Mendefinisikan metode `ListSessionPlayersAsync` dengan hasil bertipe `Task<List<SessionPlayerDb>>`; operasi ini menangani daftar sesi pemain
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionPlayerDb>> ListSessionPlayersAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode ListSessionPlayersAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListSessionPlayersAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.session_participant_id as
        // session_player_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.user_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `coalesce(nullif(sp.player_name, ''), u.display_name) as display_name,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.player_order_no as
        // player_order`.
        // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from session_participants sp`.
        // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join app_users u on u.user_id = sp.user_id`.
        // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
        // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sp.player_order_no asc, sp.joined_at
        // asc, sp.user_id asc`.
        // Baris literal 11: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                sp.session_participant_id as session_player_id,
                sp.user_id,
                coalesce(nullif(sp.player_name, ''), u.display_name) as display_name,
                sp.player_order_no as player_order
            from session_participants sp
            join app_users u on u.user_id = sp.user_id
            where sp.session_id = @sessionId
            order by sp.player_order_no asc, sp.joined_at asc, sp.user_id asc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<SessionPlayerDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<SessionPlayerDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi
            // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListSessionPlayersAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListSessionPlayersAsync; bagian berikut berada di luar batas blok tersebut dalam ListSessionPlayersAsync.
    }

    // Mendefinisikan metode `AddPlayerToSessionAndAssignPlayerOrderAsync` dengan hasil bertipe `Task<int>`; operasi ini menangani add pemain ke sesi
    // dan assign pemain urutan/pesanan asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId`
    // bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `playerOrder` bertipe `int?` membawa nomor urut pemain
    // untuk menentukan urutan tindakan; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<int> AddPlayerToSessionAndAssignPlayerOrderAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses.
        Guid userId,
        // Parameter `playerOrder` bertipe `int?` membawa nomor urut pemain untuk menentukan urutan tindakan; nilai null diizinkan ketika data opsional
        // belum tersedia.
        int? playerOrder,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode AddPlayerToSessionAndAssignPlayerOrderAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // AddPlayerToSessionAndAssignPlayerOrderAsync.
    {
        // Menyiapkan variabel lokal `insertSql` untuk nilai insert SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertSql =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participants (session_participant_id,
        // session_id, user_id, player_order_no, player_name, joined_at)`.
        // Baris literal 3: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionPlayerId,`.
        // Baris literal 5: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 6: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@userId,`.
        // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@initialPlayerOrder,`.
        // Baris literal 8: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `(select display_name from app_users where user_id = @userId),`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `@createdAt`.
        // Baris literal 10: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 11: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (session_id, user_id)
        // do update`.
        // Baris literal 12: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set player_name = excluded.player_name`.
        // Baris literal 13: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertSql = """
            insert into session_participants (session_participant_id, session_id, user_id, player_order_no, player_name, joined_at)
            values (
                @sessionPlayerId,
                @sessionId,
                @userId,
                @initialPlayerOrder,
                (select display_name from app_users where user_id = @userId),
                @createdAt
            )
            on conflict (session_id, user_id) do update
            set player_name = excluded.player_name
            """;

        // Menyiapkan variabel lokal `reorderByJoinedAtSql` untuk nilai reorder berdasarkan joined at SQL dengan literal multiline yang dirinci pada
        // komentar di dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // reorderByJoinedAtSql = ”””`.
        // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with ranked as (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (order by
        // joined_at asc, user_id asc)::int as new_player_order`.
        // Baris literal 5: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 7: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 8: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_participants sp`.
        // Baris literal 9: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set player_order_no = ranked.new_player_order`.
        // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from ranked`.
        // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_participant_id =
        // ranked.session_participant_id`.
        // Baris literal 12: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string reorderByJoinedAtSql = """
            with ranked as (
                select session_participant_id,
                       row_number() over (order by joined_at asc, user_id asc)::int as new_player_order
                from session_participants
                where session_id = @sessionId
            )
            update session_participants sp
            set player_order_no = ranked.new_player_order
            from ranked
            where sp.session_participant_id = ranked.session_participant_id
            """;

        // Menyiapkan variabel lokal `applyRequestedPlayerOrderSql` untuk nilai apply yang diminta pemain urutan/pesanan SQL dengan literal multiline yang
        // dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // applyRequestedPlayerOrderSql = ”””`.
        // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_participants`.
        // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set player_order_no = player_order_no + 1`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and user_id <> @userId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and player_order_no >= @playerOrder;`.
        // Baris literal 7: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 8: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_participants`.
        // Baris literal 9: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set player_order_no = @playerOrder`.
        // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and user_id = @userId;`.
        // Baris literal 12: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string applyRequestedPlayerOrderSql = """
            update session_participants
            set player_order_no = player_order_no + 1
            where session_id = @sessionId
              and user_id <> @userId
              and player_order_no >= @playerOrder;

            update session_participants
            set player_order_no = @playerOrder
            where session_id = @sessionId
              and user_id = @userId;
            """;

        // Menyiapkan variabel lokal `normalizePlayerOrderSql` untuk nilai normalize pemain urutan/pesanan SQL dengan literal multiline yang dirinci pada
        // komentar di dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // normalizePlayerOrderSql = ”””`.
        // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with ranked as (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (`.
        // Baris literal 5: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by player_order_no asc, joined_at asc,
        // user_id asc`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `)::int as new_player_order`.
        // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 9: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 10: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_participants sp`.
        // Baris literal 11: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set player_order_no = ranked.new_player_order`.
        // Baris literal 12: FROM memilih tabel/subquery sumber pembacaan: `from ranked`.
        // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_participant_id =
        // ranked.session_participant_id`.
        // Baris literal 14: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string normalizePlayerOrderSql = """
            with ranked as (
                select session_participant_id,
                       row_number() over (
                           order by player_order_no asc, joined_at asc, user_id asc
                       )::int as new_player_order
                from session_participants
                where session_id = @sessionId
            )
            update session_participants sp
            set player_order_no = ranked.new_player_order
            from ranked
            where sp.session_participant_id = ranked.session_participant_id
            """;

        // Menyiapkan variabel lokal `selectSql` untuk nilai select SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string selectSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id as
        // SessionParticipantId,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `player_order_no as
        // PlayerOrderNo`.
        // Baris literal 5: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and user_id = @userId`.
        // Baris literal 8: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 9: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string selectSql = """
            select
                session_participant_id as SessionParticipantId,
                player_order_no as PlayerOrderNo
            from session_participants
            where session_id = @sessionId
              and user_id = @userId
            limit 1
            """;

        // Menyiapkan variabel lokal `initializeBalanceSql` untuk nilai initialize saldo SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // initializeBalanceSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_balances (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coins,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saving,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `total_donasi,`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
        // Baris literal 10: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
        // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 12: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionParticipantId,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.starting_cash,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.starting_happiness,`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.starting_saving,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `0,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 21: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
        // Baris literal 22: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_game_settings rgs on rgs.ruleset_version_id =
        // s.ruleset_version_id`.
        // Baris literal 23: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_id = @sessionId`.
        // Baris literal 24: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
        // (session_participant_id) do nothing`.
        // Baris literal 25: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string initializeBalanceSql = """
            insert into session_participant_balances (
                session_id,
                session_participant_id,
                coins,
                happiness,
                saving,
                total_donasi,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @sessionParticipantId,
                rgs.starting_cash,
                rgs.starting_happiness,
                rgs.starting_saving,
                0,
                now(),
                now()
            from sessions s
            join ruleset_game_settings rgs on rgs.ruleset_version_id = s.ruleset_version_id
            where s.session_id = @sessionId
            on conflict (session_participant_id) do nothing
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memulai transaksi pada `conn` menggunakan `ct` agar perubahan terkait dapat diselesaikan bersama; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        await using var tx = await conn.BeginTransactionAsync(ct);

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ”set constraints
        // uq_session_participants_session_seat deferred”, transaction: tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan nilai literal `”set constraints uq_session_participants_session_seat deferred”` sebagai argumen ke konstruktor `CommandDefinition`.
            "set constraints uq_session_participants_session_seat deferred",
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen bernama `transaction`.
            transaction: tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(insertSql, new { sessionPlayerId =
        // Guid.NewGuid(), sessionId, userId, initialPlayerOrder = playerOrder ?? 1, createdAt = DateTimeOffset.UtcNow }, tx, can...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        await conn.ExecuteAsync(new CommandDefinition(insertSql, new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan sessionPlayerId, sessionId, userId, initialPlayerOrder, createdAt sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            sessionPlayerId = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan sessionPlayerId, sessionId, userId, initialPlayerOrder, createdAt sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            sessionId,
            // Meneruskan objek anonim yang mengelompokkan sessionPlayerId, sessionId, userId, initialPlayerOrder, createdAt sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            userId,
            // Meneruskan objek anonim yang mengelompokkan sessionPlayerId, sessionId, userId, initialPlayerOrder, createdAt sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            initialPlayerOrder = playerOrder ?? 1,
            // Meneruskan objek anonim yang mengelompokkan sessionPlayerId, sessionId, userId, initialPlayerOrder, createdAt sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            createdAt = DateTimeOffset.UtcNow
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        }, tx, cancellationToken: ct));

        // Memeriksa `playerOrder.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
        if (playerOrder.HasValue)
        // Membuka scope cabang if untuk kondisi `playerOrder.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        {
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( applyRequestedPlayerOrderSql, new
            // { sessionId, userId, playerOrder = playerOrder.Value }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
            await conn.ExecuteAsync(new CommandDefinition(
                // Meneruskan `applyRequestedPlayerOrderSql` (nilai apply yang diminta pemain urutan/pesanan SQL) sebagai argumen ke konstruktor
                // `CommandDefinition`.
                applyRequestedPlayerOrderSql,
                // Meneruskan objek anonim yang mengelompokkan sessionId, userId, playerOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { sessionId, userId, playerOrder = playerOrder.Value },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(normalizePlayerOrderSql, new {
            // sessionId }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
            await conn.ExecuteAsync(new CommandDefinition(normalizePlayerOrderSql, new { sessionId }, tx, cancellationToken: ct));
        // Menutup scope cabang if untuk kondisi `playerOrder.HasValue`; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
        {
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(reorderByJoinedAtSql, new {
            // sessionId }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
            await conn.ExecuteAsync(new CommandDefinition(reorderByJoinedAtSql, new { sessionId }, tx, cancellationToken: ct));
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
        }

        // Menyiapkan variabel lokal `assignment` untuk nilai assignment dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<SessionParticipantAssignmentDb>` dengan `new CommandDefinition(selectSql, new { sessionId, userId }, tx,
        // cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assignment = await conn.QuerySingleOrDefaultAsync<SessionParticipantAssignmentDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (selectSql, new { sessionId, userId }, tx, cancellationToken: ct) sebagai
            // argumen ke `conn.QuerySingleOrDefaultAsync<SessionParticipantAssignmentDb>`; Meneruskan `selectSql` (nilai select SQL) sebagai argumen ke
            // konstruktor `CommandDefinition`; Meneruskan objek anonim yang mengelompokkan sessionId, userId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
            // berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(selectSql, new { sessionId, userId }, tx, cancellationToken: ct));

        // Memeriksa hasil pencocokan `assignment` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        if (assignment is not null)
        // Membuka scope cabang if untuk kondisi `assignment is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        {
            // Menyiapkan variabel lokal `parameters` untuk nilai parameters dengan objek anonim yang mengelompokkan sessionId, sessionParticipantId sebagai
            // satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var parameters = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AddPlayerToSessionAndAssignPlayerOrderAsync.
            {
                // Menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai bagian ekspresi yang sedang disusun dalam
                // AddPlayerToSessionAndAssignPlayerOrderAsync.
                sessionId,
                // Menggunakan `sessionParticipantId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai bagian ekspresi yang sedang disusun dalam
                // AddPlayerToSessionAndAssignPlayerOrderAsync.
                sessionParticipantId = assignment.SessionParticipantId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AddPlayerToSessionAndAssignPlayerOrderAsync.
            };
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(initializeBalanceSql, parameters,
            // tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
            await conn.ExecuteAsync(new CommandDefinition(initializeBalanceSql, parameters, tx, cancellationToken: ct));
        // Menutup scope cabang if untuk kondisi `assignment is not null`; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        }

        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam AddPlayerToSessionAndAssignPlayerOrderAsync.
        await tx.CommitAsync(ct);

        // Memeriksa hasil pencocokan `assignment` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        if (assignment is null)
        // Membuka scope cabang if untuk kondisi `assignment is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Pemain gagal terdaftar pada sesi.”) dalam
            // AddPlayerToSessionAndAssignPlayerOrderAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Pemain gagal terdaftar pada sesi.");
        // Menutup scope cabang if untuk kondisi `assignment is null`; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayerToSessionAndAssignPlayerOrderAsync.
        }

        // Mengembalikan `assignment.PlayerOrderNo` (nilai pemain urutan/pesanan no) kepada pemanggil dalam AddPlayerToSessionAndAssignPlayerOrderAsync;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return assignment.PlayerOrderNo;
    // Menutup scope metode AddPlayerToSessionAndAssignPlayerOrderAsync; bagian berikut berada di luar batas blok tersebut dalam
    // AddPlayerToSessionAndAssignPlayerOrderAsync.
    }

    // Mendefinisikan metode `GetSessionPlayerPlayerOrderMapAsync` dengan hasil bertipe `Task<Dictionary<Guid, int>>`; operasi ini menangani get sesi
    // pemain pemain urutan/pesanan pemetaan asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    public async Task<Dictionary<Guid, int>> GetSessionPlayerPlayerOrderMapAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetSessionPlayerPlayerOrderMapAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetSessionPlayerPlayerOrderMapAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select user_id, player_order_no as player_order`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by player_order_no asc, joined_at asc`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select user_id, player_order_no as player_order
            from session_participants
            where session_id = @sessionId
            order by player_order_no asc, joined_at asc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = await conn.QueryAsync<SessionPlayerPlayerOrderDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<SessionPlayerPlayerOrderDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek
            // anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan
            // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        // Mengembalikan membangun kamus dari `rows` dengan pemilihan kunci/nilai `row => row.UserId`, `row => row.PlayerOrder`; kunci harus unik agar
        // konversi berhasil kepada pemanggil dalam GetSessionPlayerPlayerOrderMapAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return rows.ToDictionary(row => row.UserId, row => row.PlayerOrder);
    // Menutup scope metode GetSessionPlayerPlayerOrderMapAsync; bagian berikut berada di luar batas blok tersebut dalam
    // GetSessionPlayerPlayerOrderMapAsync.
    }

    // Mendefinisikan metode `GetSessionParticipantPlayerOrderMapAsync` dengan hasil bertipe `Task<Dictionary<Guid, int>>`; operasi ini menangani get
    // sesi participant pemain urutan/pesanan pemetaan asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi
    // ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan
    // atau aplikasi berhenti.
    public async Task<Dictionary<Guid, int>> GetSessionParticipantPlayerOrderMapAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetSessionParticipantPlayerOrderMapAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetSessionParticipantPlayerOrderMapAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id, player_order_no as player_order`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by player_order_no asc, joined_at asc`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select session_participant_id, player_order_no as player_order
            from session_participants
            where session_id = @sessionId
            order by player_order_no asc, joined_at asc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = await conn.QueryAsync<SessionParticipantPlayerOrderDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<SessionParticipantPlayerOrderDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        // Mengembalikan membangun kamus dari `rows` dengan pemilihan kunci/nilai `row => row.SessionParticipantId`, `row => row.PlayerOrder`; kunci harus
        // unik agar konversi berhasil kepada pemanggil dalam GetSessionParticipantPlayerOrderMapAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return rows.ToDictionary(row => row.SessionParticipantId, row => row.PlayerOrder);
    // Menutup scope metode GetSessionParticipantPlayerOrderMapAsync; bagian berikut berada di luar batas blok tersebut dalam
    // GetSessionParticipantPlayerOrderMapAsync.
    }

    // Mendefinisikan metode `GetSessionParticipantIdAsync` dengan hasil bertipe `Task<Guid?>`; operasi ini menangani get sesi participant identitas
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa identitas
    // akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<Guid?> GetSessionParticipantIdAsync(Guid sessionId, Guid userId, CancellationToken ct)
    // Membuka scope metode GetSessionParticipantIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetSessionParticipantIdAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and user_id = @userId`.
        // Baris literal 6: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select session_participant_id
            from session_participants
            where session_id = @sessionId
              and user_id = @userId
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new
        // CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam GetSessionParticipantIdAsync; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<Guid?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<Guid?>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim
            // yang mengelompokkan sessionId, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan
            // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    // Menutup scope metode GetSessionParticipantIdAsync; bagian berikut berada di luar batas blok tersebut dalam GetSessionParticipantIdAsync.
    }

    // Mendefinisikan metode `CountPlayersInSessionAsync` dengan hasil bertipe `Task<int>`; operasi ini menangani jumlah pemain in sesi asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<int> CountPlayersInSessionAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode CountPlayersInSessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CountPlayersInSessionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)::int`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select count(*)::int
            from session_participants
            where session_id = @sessionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId },
        // cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada
        // pemanggil dalam CountPlayersInSessionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    // Menutup scope metode CountPlayersInSessionAsync; bagian berikut berada di luar batas blok tersebut dalam CountPlayersInSessionAsync.
    }

    // Mendefinisikan metode `IsPlayerInSessionAsync` dengan hasil bertipe `Task<bool>`; operasi ini menangani berstatus pemain in sesi asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang
    // datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    public async Task<bool> IsPlayerInSessionAsync(Guid sessionId, Guid userId, CancellationToken ct)
    // Membuka scope metode IsPlayerInSessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsPlayerInSessionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId and user_id = @userId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select 1
            from session_participants
            where session_id = @sessionId and user_id = @userId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct)` dan mengambil nilai
        // skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
        // Mengembalikan `result.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong kepada pemanggil dalam IsPlayerInSessionAsync; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return result.HasValue;
    // Menutup scope metode IsPlayerInSessionAsync; bagian berikut berada di luar batas blok tersebut dalam IsPlayerInSessionAsync.
    }

    // Mendefinisikan tipe class `SessionPlayerPlayerOrderDb`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SessionPlayerPlayerOrderDb
    // Membuka scope tipe SessionPlayerPlayerOrderDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid UserId { get; init; }
        // Mendefinisikan properti `PlayerOrder` bertipe `int` untuk nomor urut pemain untuk menentukan urutan tindakan; get menyediakan pembacaan nilai,
        // init membatasi pengisian saat inisialisasi objek.
        public int PlayerOrder { get; init; }
    // Menutup scope tipe SessionPlayerPlayerOrderDb; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `SessionParticipantPlayerOrderDb`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SessionParticipantPlayerOrderDb
    // Membuka scope tipe SessionParticipantPlayerOrderDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionParticipantId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionParticipantId { get; init; }
        // Mendefinisikan properti `PlayerOrder` bertipe `int` untuk nomor urut pemain untuk menentukan urutan tindakan; get menyediakan pembacaan nilai,
        // init membatasi pengisian saat inisialisasi objek.
        public int PlayerOrder { get; init; }
    // Menutup scope tipe SessionParticipantPlayerOrderDb; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `SessionParticipantAssignmentDb`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SessionParticipantAssignmentDb
    // Membuka scope tipe SessionParticipantAssignmentDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionParticipantId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionParticipantId { get; init; }
        // Mendefinisikan properti `PlayerOrderNo` bertipe `int` untuk nilai pemain urutan/pesanan no; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int PlayerOrderNo { get; init; }
    // Menutup scope tipe SessionParticipantAssignmentDb; bagian berikut berada di luar batas blok tersebut.
    }
// Menutup scope tipe PlayerRepository; bagian berikut berada di luar batas blok tersebut.
}
