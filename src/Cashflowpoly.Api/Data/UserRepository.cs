// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk UserRepository.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Record data user yang berhasil diautentikasi: ID, username, display name, role, dan status aktif.
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AuthenticatedUserDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AuthenticatedUserDb(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses.
    Guid UserId,
    // Parameter `Username` bertipe `string` membawa nama akun yang dipakai saat autentikasi.
    string Username,
    // Parameter `DisplayName` bertipe `string` membawa nilai display nama.
    string DisplayName,
    // Parameter `Role` bertipe `string` membawa peran pengguna yang menentukan hak akses.
    string Role,
    // Parameter `IsActive` bertipe `bool` membawa nilai berstatus aktif.
    bool IsActive,
    // Parameter `IsDemo` bertipe `bool` membawa nilai berstatus demo; bila argumen tidak diberikan digunakan false, yaitu kondisi nonaktif/tidak
    // terpenuhi.
    bool IsDemo = false);

/// <summary>
/// Repository untuk autentikasi user aplikasi dan identitas player berbasis app_users.
/// </summary>
// Mendefinisikan tipe class `UserRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class UserRepository
// Membuka scope tipe UserRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;

    // Mendefinisikan konstruktor UserRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public UserRepository(NpgsqlDataSource dataSource)
    // Membuka scope konstruktor UserRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam UserRepository.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // UserRepository.
        _dataSource = dataSource;
    // Menutup scope konstruktor UserRepository; bagian berikut berada di luar batas blok tersebut dalam UserRepository.
    }

    // Mendefinisikan metode `AuthenticateAsync` dengan hasil bertipe `Task<AuthenticatedUserDb?>`; operasi ini menangani authenticate asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `username` bertipe `string`
    // membawa nama akun yang dipakai saat autentikasi; Parameter `password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai kebijakan
    // autentikasi; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    public async Task<AuthenticatedUserDb?> AuthenticateAsync(string username, string password, CancellationToken ct)
    // Membuka scope metode AuthenticateAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AuthenticateAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select user_id, username, display_name, role, is_active, is_demo`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where username = @username`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active = true`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and password_hash = crypt(@password, password_hash)`.
        // Baris literal 7: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 8: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select user_id, username, display_name, role, is_active, is_demo
            from app_users
            where username = @username
              and is_active = true
              and password_hash = crypt(@password, password_hash)
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QueryFirstOrDefaultAsync<AuthenticatedUserDb>` dengan `new
        // CommandDefinition(sql, new { username, password }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam AuthenticateAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return await conn.QueryFirstOrDefaultAsync<AuthenticatedUserDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { username, password }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryFirstOrDefaultAsync<AuthenticatedUserDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // objek anonim yang mengelompokkan username, password sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct`
            // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { username, password }, cancellationToken: ct));
    // Menutup scope metode AuthenticateAsync; bagian berikut berada di luar batas blok tersebut dalam AuthenticateAsync.
    }

    // Mendefinisikan metode `CreatePlayerUserAsync` dengan hasil bertipe `Task<AuthenticatedUserDb>`; operasi ini menangani create pemain pengguna
    // asinkron. Masukan: Parameter `username` bertipe `string` membawa nama akun yang dipakai saat autentikasi; Parameter `password` bertipe `string`
    // membawa kata sandi masukan yang diperiksa sesuai kebijakan autentikasi; Parameter `displayName` bertipe `string` membawa nilai display nama;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
    public Task<AuthenticatedUserDb> CreatePlayerUserAsync(
        // Parameter `username` bertipe `string` membawa nama akun yang dipakai saat autentikasi.
        string username,
        // Parameter `password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai kebijakan autentikasi.
        string password,
        // Parameter `displayName` bertipe `string` membawa nilai display nama.
        string displayName,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode CreatePlayerUserAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreatePlayerUserAsync.
    {
        // Mengembalikan memanggil `CreateUserAsync` dengan `username`, `password`, `”PLAYER”`, `displayName`, `ct` kepada pemanggil dalam
        // CreatePlayerUserAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return CreateUserAsync(username, password, "PLAYER", displayName, ct);
    // Menutup scope metode CreatePlayerUserAsync; bagian berikut berada di luar batas blok tersebut dalam CreatePlayerUserAsync.
    }

    // Mendefinisikan metode `CreateUserAsync` dengan hasil bertipe `Task<AuthenticatedUserDb>`; operasi ini menangani create pengguna asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `username` bertipe `string`
    // membawa nama akun yang dipakai saat autentikasi; Parameter `password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai kebijakan
    // autentikasi; Parameter `role` bertipe `string` membawa peran pengguna yang menentukan hak akses; Parameter `displayName` bertipe `string?`
    // membawa nilai display nama; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<AuthenticatedUserDb> CreateUserAsync(
        // Parameter `username` bertipe `string` membawa nama akun yang dipakai saat autentikasi.
        string username,
        // Parameter `password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai kebijakan autentikasi.
        string password,
        // Parameter `role` bertipe `string` membawa peran pengguna yang menentukan hak akses.
        string role,
        // Parameter `displayName` bertipe `string?` membawa nilai display nama; nilai null diizinkan ketika data opsional belum tersedia.
        string? displayName,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode CreateUserAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateUserAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into app_users (user_id, username, display_name,
        // password_hash, role, is_active, created_at)`.
        // Baris literal 3: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (@userId,
        // @username, @displayName, crypt(@password, gen_salt('bf', 10)), @role, true, now())`.
        // Baris literal 4: RETURNING mengembalikan kolom dari baris yang baru ditambahkan/diubah: `returning user_id, username, display_name, role,
        // is_active, is_demo`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            insert into app_users (user_id, username, display_name, password_hash, role, is_active, created_at)
            values (@userId, @username, @displayName, crypt(@password, gen_salt('bf', 10)), @role, true, now())
            returning user_id, username, display_name, role, is_active, is_demo
            """;

        // Menyiapkan variabel lokal `userId` untuk identitas akun pengguna yang datanya sedang diproses dengan memanggil `Guid.NewGuid` dengan tanpa
        // argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var userId = Guid.NewGuid();
        // Menyiapkan variabel lokal `resolvedDisplayName` untuk nilai hasil resolusi display nama dengan hasil pemilihan bersyarat: ketika
        // `string.IsNullOrWhiteSpace(displayName)` benar gunakan `username`, jika tidak gunakan `displayName.Trim()`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var resolvedDisplayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName.Trim();

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca tepat satu baris basis data melalui `conn.QuerySingleAsync<AuthenticatedUserDb>` dengan `new
        // CommandDefinition(sql, new { userId, username, displayName = resolvedDisplayName, password, role }, cancellationToken: ct)`; jumlah baris selain
        // satu menyebabkan exception; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam CreateUserAsync;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleAsync<AuthenticatedUserDb>(new CommandDefinition(sql, new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateUserAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan userId, username, displayName, password, role sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            userId,
            // Meneruskan objek anonim yang mengelompokkan userId, username, displayName, password, role sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            username,
            // Meneruskan objek anonim yang mengelompokkan userId, username, displayName, password, role sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            displayName = resolvedDisplayName,
            // Meneruskan objek anonim yang mengelompokkan userId, username, displayName, password, role sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            password,
            // Meneruskan objek anonim yang mengelompokkan userId, username, displayName, password, role sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            role
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateUserAsync.
        }, cancellationToken: ct));
    // Menutup scope metode CreateUserAsync; bagian berikut berada di luar batas blok tersebut dalam CreateUserAsync.
    }

    // Mendefinisikan metode `GetPlayerUserIdAsync` dengan hasil bertipe `Task<Guid?>`; operasi ini menangani get pemain pengguna identitas asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `userId` bertipe
    // `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<Guid?> GetPlayerUserIdAsync(Guid userId, CancellationToken ct)
    // Membuka scope metode GetPlayerUserIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetPlayerUserIdAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where user_id = @userId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and role = 'PLAYER'`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active = true`.
        // Baris literal 7: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 8: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select user_id
            from app_users
            where user_id = @userId
              and role = 'PLAYER'
              and is_active = true
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new
        // CommandDefinition(sql, new { userId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetPlayerUserIdAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return await conn.QuerySingleOrDefaultAsync<Guid?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { userId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<Guid?>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim
            // yang mengelompokkan userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi
            // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { userId }, cancellationToken: ct));
    // Menutup scope metode GetPlayerUserIdAsync; bagian berikut berada di luar batas blok tersebut dalam GetPlayerUserIdAsync.
    }

    // Mendefinisikan metode `GetUsernamesByUserIdsAsync` dengan hasil bertipe `Task<Dictionary<Guid, string>>`; operasi ini menangani get usernames
    // berdasarkan pengguna identitas asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `userIds` bertipe `IReadOnlyCollection<Guid>` membawa nilai pengguna identitas; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<Dictionary<Guid, string>> GetUsernamesByUserIdsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken ct)
    // Membuka scope metode GetUsernamesByUserIdsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetUsernamesByUserIdsAsync.
    {
        // Memeriksa perbandingan kesamaan antara `userIds.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetUsernamesByUserIdsAsync.
        if (userIds.Count == 0)
        // Membuka scope cabang if untuk kondisi `userIds.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetUsernamesByUserIdsAsync.
        {
            // Mengembalikan objek baru bertipe `Dictionary<Guid, string>` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam
            // GetUsernamesByUserIdsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new Dictionary<Guid, string>();
        // Menutup scope cabang if untuk kondisi `userIds.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam GetUsernamesByUserIdsAsync.
        }

        // Menyiapkan variabel lokal `normalizedUserIds` untuk nilai normalized pengguna identitas dengan mematerialisasi urutan `userIds .Where(userId =>
        // userId != Guid.Empty) .Distinct()` menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalizedUserIds = userIds
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(userId => userId != Guid.Empty) dalam GetUsernamesByUserIdsAsync; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(userId => userId != Guid.Empty)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Distinct() dalam GetUsernamesByUserIdsAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Distinct()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToArray(); dalam GetUsernamesByUserIdsAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToArray();
        // Memeriksa perbandingan kesamaan antara `normalizedUserIds.Length` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetUsernamesByUserIdsAsync.
        if (normalizedUserIds.Length == 0)
        // Membuka scope cabang if untuk kondisi `normalizedUserIds.Length == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetUsernamesByUserIdsAsync.
        {
            // Mengembalikan objek baru bertipe `Dictionary<Guid, string>` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam
            // GetUsernamesByUserIdsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new Dictionary<Guid, string>();
        // Menutup scope cabang if untuk kondisi `normalizedUserIds.Length == 0`; bagian berikut berada di luar batas blok tersebut dalam
        // GetUsernamesByUserIdsAsync.
        }

        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select user_id as UserId, username as Username`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where user_id = any(@userIds)`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select user_id as UserId, username as Username
            from app_users
            where user_id = any(@userIds)
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { userIds = normalizedUserIds }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = await conn.QueryAsync<UserNameRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { userIds = normalizedUserIds }, cancellationToken: ct) sebagai
            // argumen ke `conn.QueryAsync<UserNameRow>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek
            // anonim yang mengelompokkan userIds sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar
            // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { userIds = normalizedUserIds }, cancellationToken: ct));

        // Mengembalikan membangun kamus dari `rows .Where(row => row.UserId != Guid.Empty && !string.IsNullOrWhiteSpace(row.Username)) .GroupBy(row =>
        // row.UserId)` dengan pemilihan kunci/nilai `group => group.Key`, `group => group.First().Username.Trim()`; kunci harus unik agar konversi berhasil
        // kepada pemanggil dalam GetUsernamesByUserIdsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return rows
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(row => row.UserId != Guid.Empty &&
            // !string.IsNullOrWhiteSpace(row.Username)) dalam GetUsernamesByUserIdsAsync; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Where(row => row.UserId != Guid.Empty && !string.IsNullOrWhiteSpace(row.Username))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(row => row.UserId) dalam GetUsernamesByUserIdsAsync; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(row => row.UserId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.First().Username.Trim());
            // dalam GetUsernamesByUserIdsAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.First().Username.Trim());
    // Menutup scope metode GetUsernamesByUserIdsAsync; bagian berikut berada di luar batas blok tersebut dalam GetUsernamesByUserIdsAsync.
    }

    // Mendefinisikan tipe class `UserNameRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class UserNameRow
    // Membuka scope tipe UserNameRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid UserId { get; init; }
        // Mendefinisikan properti `Username` bertipe `string` untuk nama akun yang dipakai saat autentikasi; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Username { get; init; } = string.Empty;
    // Menutup scope tipe UserNameRow; bagian berikut berada di luar batas blok tersebut.
    }
// Menutup scope tipe UserRepository; bagian berikut berada di luar batas blok tersebut.
}
