// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk PlayersController.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Security` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Security;
// Mengimpor namespace `Microsoft.AspNetCore.Authorization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authorization;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Controllers;

// mengaktifkan perilaku API controller, termasuk inferensi binding dan respons otomatis atas model tidak valid.
[ApiController]
// menetapkan pola rute (”api/v1/players”) untuk pencocokan URL permintaan.
[Route("api/v1/players")]
// mewajibkan otorisasi pengguna sesuai kebijakan autentikasi aplikasi.
[Authorize]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class PlayersController : ControllerBase
{
    private readonly PlayerRepository _players;
    private readonly SessionRepository _sessions;
    private readonly SessionStateRepository _state;
    private readonly RulesetRepository _rulesets;
    private readonly UserRepository _users;

    public PlayersController(
        // Parameter `players` bertipe `PlayerRepository` membawa nilai pemain.
        PlayerRepository players,
        // Parameter `sessions` bertipe `SessionRepository` membawa nilai sessions.
        SessionRepository sessions,
        // Parameter `state` bertipe `SessionStateRepository` membawa keadaan permainan yang menjadi sumber atau hasil pembaruan.
        SessionStateRepository state,
        // Parameter `rulesets` bertipe `RulesetRepository` membawa nilai aturan.
        RulesetRepository rulesets,
        // Parameter `users` bertipe `UserRepository` membawa nilai pengguna.
        UserRepository users)
    {
        _players = players;
        _sessions = sessions;
        _state = state;
        _rulesets = rulesets;
        _users = users;
    }

    // mendaftarkan action untuk metode HTTP POST pada rute controller saat ini.
    [HttpPost]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(PlayerResponse), StatusCodes.Status201Created)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(PlayerResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out _))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Nama pemain wajib diisi",
                new ErrorDetail("display_name", "REQUIRED")));
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username wajib diisi",
                new ErrorDetail("username", "REQUIRED")));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Password wajib diisi",
                new ErrorDetail("password", "REQUIRED")));
        }

        if (request.DisplayName.Trim().Length > 80)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Nama pemain maksimal 80 karakter",
                new ErrorDetail("display_name", "OUT_OF_RANGE")));
        }

        var username = request.Username.Trim();
        if (username.Length < 3 || username.Length > 80)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username harus 3-80 karakter",
                new ErrorDetail("username", "OUT_OF_RANGE")));
        }

        if (request.Password.Length < PasswordPolicy.MinPasswordLength)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                $"Password minimal {PasswordPolicy.MinPasswordLength} karakter",
                new ErrorDetail("password", "OUT_OF_RANGE")));
        }

        if (!PasswordPolicy.IsWithinBcryptLimit(request.Password))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                $"Password maksimal {PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8",
                new ErrorDetail("password", "OUT_OF_RANGE")));
        }

        AuthenticatedUserDb createdUser;
        try
        {
            createdUser = await _users.CreatePlayerUserAsync(
                username,
                request.Password,
                request.DisplayName.Trim(),
                ct);
        }
        // Menangani exception `PostgresException` melalui variabel exception hanya jika filter `exception.SqlState == PostgresErrorCodes.UniqueViolation`
        // terpenuhi dalam CreatePlayer.
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return Conflict(ApiErrorHelper.BuildError(HttpContext, "DUPLICATE", "Username sudah digunakan"));
        }

        return Created($"/api/v1/players/{createdUser.UserId}", new PlayerResponse(createdUser.UserId, createdUser.DisplayName));
    }

    // mendaftarkan action untuk metode HTTP GET pada rute controller saat ini.
    [HttpGet]
    // menerapkan metadata `ProducesResponseType(typeof(PlayerListResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(PlayerListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListPlayers(CancellationToken ct, [FromQuery] bool inMySessions = false)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        List<PlayerDb> players;

        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            players = await _players.ListPlayersAsync(ct, inMySessions ? userId : null);
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            players = await _players.ListPlayersByPlayerScopeAsync(playerUserId.Value, ct);
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        var items = players.Select(p => new PlayerResponse(p.UserId, p.DisplayName)).ToList();
        return Ok(new PlayerListResponse(items));
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”/api/v1/sessions/{sessionId:guid}/players”).
    [HttpGet("/api/v1/sessions/{sessionId:guid}/players")]
    // menerapkan metadata `ProducesResponseType(typeof(SessionPlayerListResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(SessionPlayerListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSessionPlayers(Guid sessionId, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            if (await _sessions.GetSessionForInstructorAsync(sessionId, userId, ct) is null)
            {
                return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
            }
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue || !await _players.IsPlayerInSessionAsync(sessionId, playerUserId.Value, ct))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Pemain tidak terdaftar pada sesi ini"));
            }
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        var items = (await _players.ListSessionPlayersAsync(sessionId, ct))
            .Select(player => new SessionPlayerResponse(player.SessionPlayerId, player.UserId, player.DisplayName, player.PlayerOrder))
            .ToList();
        return Ok(new SessionPlayerListResponse(items));
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”/api/v1/sessions/{sessionId:guid}/players”).
    [HttpPost("/api/v1/sessions/{sessionId:guid}/players")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(AddSessionPlayerResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(AddSessionPlayerResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddPlayerToSession(Guid sessionId, [FromBody] AddSessionPlayerRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var session = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        if (!request.UserId.HasValue && string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "User ID atau username wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"),
                new ErrorDetail("username", "REQUIRED")));
        }

        if (await _state.HasSessionSetupAsync(sessionId, ct))
        {
            return Conflict(ApiErrorHelper.BuildError(
                HttpContext,
                "SESSION_ROSTER_LOCKED",
                "Daftar pemain sudah dikunci sejak pembagian awal disimpan"));
        }

        if (request.PlayerOrder is <= 0)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Seat number minimal 1",
                new ErrorDetail("player_order_no", "OUT_OF_RANGE")));
        }

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        if (!activeRulesetVersionId.HasValue)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Session belum memiliki ruleset aktif"));
        }

        var activeRulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
        if (activeRulesetVersion?.Definition is null ||
            !string.Equals(activeRulesetVersion.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(activeRulesetVersion.Mode, session.Mode, StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Ruleset aktif session tidak valid"));
        }

        var maxPlayers = activeRulesetVersion.Definition.Settings.MaxPlayers;
        if (request.PlayerOrder is > 0 && request.PlayerOrder > maxPlayers)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                $"Seat number maksimal {maxPlayers}"));
        }

        PlayerDb? player = null;
        if (request.UserId.HasValue)
        {
            player = await _players.GetPlayerAsync(request.UserId.Value, ct);
        }
        else if (!string.IsNullOrWhiteSpace(request.Username))
        {
            player = await _players.GetPlayerByUsernameAsync(request.Username.Trim(), ct);
        }

        if (player is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Player tidak ditemukan"));
        }

        var userId = player.UserId;
        var alreadyInSession = await _players.IsPlayerInSessionAsync(sessionId, userId, ct);
        if (!alreadyInSession)
        {
            var playersInSession = await _players.CountPlayersInSessionAsync(sessionId, ct);
            if (playersInSession >= maxPlayers)
            {
                return UnprocessableEntity(ApiErrorHelper.BuildError(
                    HttpContext,
                    "DOMAIN_RULE_VIOLATION",
                    $"Sesi maksimal {maxPlayers} pemain"));
            }
        }

        var assignment = await _players.AddPlayerToSessionAndAssignPlayerOrderAsync(
            sessionId,
            userId,
            request.PlayerOrder,
            ct);

        if (assignment.Error is not null)
        {
            return assignment.Error == "SESSION_FULL"
                ? UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION", $"Sesi maksimal {maxPlayers} pemain"))
                : Conflict(ApiErrorHelper.BuildError(HttpContext, assignment.Error,
                    "Daftar pemain sudah terkunci. Muat ulang sesi untuk melihat pembagian awal terbaru."));
        }

        return Ok(new AddSessionPlayerResponse(userId, assignment.PlayerOrder));
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }

}
