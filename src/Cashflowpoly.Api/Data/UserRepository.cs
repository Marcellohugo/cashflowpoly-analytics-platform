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
{
    private readonly NpgsqlDataSource _dataSource;

    public UserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<AuthenticatedUserDb?> AuthenticateAsync(string username, string password, CancellationToken ct)
    {
        const string sql = """
            select user_id, username, display_name, role, is_active, is_demo
            from app_users
            where username = @username
              and is_active = true
              and password_hash = crypt(@password, password_hash)
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QueryFirstOrDefaultAsync<AuthenticatedUserDb>(
            new CommandDefinition(sql, new { username, password }, cancellationToken: ct));
    }

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
    {
        return CreateUserAsync(username, password, "PLAYER", displayName, ct);
    }

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
    {
        const string sql = """
            insert into app_users (user_id, username, display_name, password_hash, role, is_active, created_at)
            values (@userId, @username, @displayName, crypt(@password, gen_salt('bf', 10)), @role, true, now())
            returning user_id, username, display_name, role, is_active, is_demo
            """;

        var userId = Guid.NewGuid();
        var resolvedDisplayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName.Trim();

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleAsync<AuthenticatedUserDb>(new CommandDefinition(sql, new
        {
            userId,
            username,
            displayName = resolvedDisplayName,
            password,
            role
        }, cancellationToken: ct));
    }

    public async Task<Guid?> GetPlayerUserIdAsync(Guid userId, CancellationToken ct)
    {
        const string sql = """
            select user_id
            from app_users
            where user_id = @userId
              and role = 'PLAYER'
              and is_active = true
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(sql, new { userId }, cancellationToken: ct));
    }

    public async Task<Dictionary<Guid, string>> GetUsernamesByUserIdsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken ct)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var normalizedUserIds = userIds
            .Where(userId => userId != Guid.Empty)
            .Distinct()
            .ToArray();
        if (normalizedUserIds.Length == 0)
        {
            return new Dictionary<Guid, string>();
        }

        const string sql = """
            select user_id as UserId, username as Username
            from app_users
            where user_id = any(@userIds)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<UserNameRow>(
            new CommandDefinition(sql, new { userIds = normalizedUserIds }, cancellationToken: ct));

        return rows
            .Where(row => row.UserId != Guid.Empty && !string.IsNullOrWhiteSpace(row.Username))
            .GroupBy(row => row.UserId)
            .ToDictionary(group => group.Key, group => group.First().Username.Trim());
    }

    private sealed class UserNameRow
    {
        public Guid UserId { get; init; }
        public string Username { get; init; } = string.Empty;
    }
}
