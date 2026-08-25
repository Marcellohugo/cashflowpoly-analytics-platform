// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk SessionsController.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Contracts;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/sessions")]
[Authorize]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class SessionsController : ControllerBase
{
    private readonly RulesetRepository _rulesets;
    private readonly SessionRepository _sessions;
    private readonly SessionStateRepository _state;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;

    public SessionsController(
        RulesetRepository rulesets,
        SessionRepository sessions,
        SessionStateRepository state,
        PlayerRepository players,
        UserRepository users)
    {
        _rulesets = rulesets;
        _sessions = sessions;
        _state = state;
        _players = players;
        _users = users;
    }

    [HttpGet]
    [ProducesResponseType(typeof(SessionListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSessions(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        List<SessionDb> sessions;

        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            sessions = await _sessions.ListSessionsByInstructorAsync(userId, ct);
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            sessions = await _sessions.ListSessionsByPlayerAsync(playerUserId.Value, ct);
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        var items = sessions.Select(s => new SessionListItem(
            s.SessionId,
            s.SessionName,
            s.Mode,
            s.Status,
            s.CreatedAt,
            s.StartedAt,
            s.EndedAt)).ToList();

        return Ok(new SessionListResponse(items));
    }

    [HttpPost]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(CreateSessionResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        if (string.IsNullOrWhiteSpace(request.SessionName))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Field wajib tidak lengkap",
                new ErrorDetail("session_name", "REQUIRED")));
        }

        if (!TryNormalizeMode(request.Mode, out var mode, out var modeError))
        {
            return modeError!;
        }

        if (request.PlayerNames is not null)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "player_names tidak lagi didukung. Buat sesi, tambahkan pemain, lalu kirim pembagian awal dari IDN.",
                new ErrorDetail("player_names", "NOT_ALLOWED")));
        }

        if (!request.RulesetVersionId.HasValue)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Field wajib tidak lengkap",
                new ErrorDetail("ruleset_version_id", "REQUIRED")));
        }

        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(request.RulesetVersionId.Value, ct);
        if (rulesetVersion is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        }

        var ruleset = await _rulesets.GetRulesetForSessionAsync(rulesetVersion.RulesetId, instructorUserId, ct);
        if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        if (!string.Equals(rulesetVersion.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Ruleset version harus ACTIVE sebelum dipakai sesi"));
        }

        if (!string.Equals(rulesetVersion.Mode, mode, StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Mode session harus sama dengan mode ruleset version",
                new ErrorDetail("mode", "MODE_MISMATCH")));
        }

        if (rulesetVersion.Definition is null)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Definition ruleset tidak valid",
                new ErrorDetail("definition", "REQUIRED")));
        }

        if (!RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out _, out var errors))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Definition ruleset tidak valid",
                errors.ToArray()));
        }

        var sessionId = await _sessions.CreateSessionAsync(
            request.SessionName.Trim(),
            mode,
            rulesetVersion.RulesetVersionId,
            instructorUserId,
            GetActorName(),
            ct);

        return Created(
            $"/api/v1/sessions/{sessionId}",
            new CreateSessionResponse(sessionId, rulesetVersion.RulesetId, rulesetVersion.RulesetVersionId));
    }

    [HttpPost("{sessionId:guid}/setup/validate")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(SessionSetupValidationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateSetup(
        Guid sessionId,
        [FromBody] SessionSetupRequest request,
        CancellationToken ct)
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

        if (!string.Equals(session.Status, "CREATED", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Pembagian awal hanya dapat diperiksa sebelum sesi dimulai"));
        }

        var activeRuleset = await GetActiveRulesetAsync(session, ct);
        if (activeRuleset is null)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Session belum memiliki ruleset ACTIVE yang valid"));
        }

        var sessionPlayers = await _players.ListSessionPlayersAsync(sessionId, ct);
        var errors = SessionSetupValidator.Validate(
            request,
            activeRuleset.Value.Version.Definition!,
            session.Mode,
            sessionPlayers);
        if (errors.Count > 0)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "INVALID_SESSION_SETUP",
                "Pembagian awal tidak sesuai dengan pemain dan set aturan sesi",
                errors.ToArray()));
        }

        return Ok(new SessionSetupValidationResponse(true));
    }

    [HttpPost("{sessionId:guid}/setup")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(SessionSetupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SessionSetupResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> SaveSetup(
        Guid sessionId,
        [FromBody] SessionSetupRequest request,
        CancellationToken ct)
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

        if (!string.Equals(session.Status, "CREATED", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Pembagian awal tidak dapat diubah setelah sesi dimulai"));
        }

        var activeRuleset = await GetActiveRulesetAsync(session, ct);
        if (activeRuleset is null)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Session belum memiliki ruleset ACTIVE yang valid"));
        }

        var normalized = SessionSetupValidator.Normalize(request);
        var sessionPlayers = await _players.ListSessionPlayersAsync(sessionId, ct);
        var errors = SessionSetupValidator.Validate(
            normalized,
            activeRuleset.Value.Version.Definition!,
            session.Mode,
            sessionPlayers);
        if (errors.Count > 0)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "INVALID_SESSION_SETUP",
                "Pembagian awal tidak sesuai dengan pemain dan set aturan sesi",
                errors.ToArray()));
        }

        var existingRequestId = await _state.GetSessionSetupByClientRequestAsync(
            instructorUserId,
            normalized.ClientRequestId,
            ct);
        if (existingRequestId is not null)
        {
            var existingRequest = SessionStateRepository.DeserializeSetup(existingRequestId);
            if (existingRequestId.SessionId != sessionId || !SessionSetupMatches(existingRequest, normalized))
            {
                return Conflict(ApiErrorHelper.BuildError(
                    HttpContext,
                    "CLIENT_REQUEST_ID_CONFLICT",
                    "client_request_id sudah dipakai untuk pembagian awal lain"));
            }

            return Ok(BuildSetupResponse(existingRequestId, existingRequest));
        }

        SessionSetupDb stored;
        try
        {
            stored = await _state.CreateSessionSetupRevisionAsync(
                sessionId,
                activeRuleset.Value.Version.RulesetVersionId,
                normalized,
                instructorUserId,
                DateTimeOffset.UtcNow,
                ct);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return Conflict(ApiErrorHelper.BuildError(
                HttpContext,
                "CLIENT_REQUEST_ID_CONFLICT",
                "client_request_id sudah dipakai untuk pembagian awal lain"));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiErrorHelper.BuildError(HttpContext, "SESSION_SETUP_LOCKED", ex.Message));
        }

        return Created(
            $"/api/v1/sessions/{sessionId}/setup",
            BuildSetupResponse(stored, normalized));
    }

    [HttpGet("{sessionId:guid}/setup")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(SessionSetupResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSetup(Guid sessionId, CancellationToken ct)
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

        var stored = await _state.GetSessionSetupAsync(sessionId, ct);
        if (stored is null)
        {
            return NotFound(ApiErrorHelper.BuildError(
                HttpContext,
                "SETUP_NOT_FOUND",
                "Pembagian awal belum dikirim oleh IDN"));
        }

        return Ok(BuildSetupResponse(stored, SessionStateRepository.DeserializeSetup(stored)));
    }

    [HttpPost("{sessionId:guid}/start")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(SessionStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartSession(Guid sessionId, CancellationToken ct)
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

        if (!string.Equals(session.Status, "CREATED", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION", "Status sesi tidak valid"));
        }

        var sessionPlayers = await _players.ListSessionPlayersAsync(sessionId, ct);
        var activeRuleset = await GetActiveRulesetAsync(session, ct);
        if (activeRuleset is null)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Session belum memiliki ruleset ACTIVE yang valid"));
        }

        var settings = activeRuleset.Value.Version.Definition!.Settings;
        if (sessionPlayers.Count < settings.MinPlayers || sessionPlayers.Count > settings.MaxPlayers)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                $"Session membutuhkan {settings.MinPlayers} sampai {settings.MaxPlayers} pemain",
                new ErrorDetail("player_count", "COUNT_OUT_OF_RANGE")));
        }

        var storedSetup = await _state.GetSessionSetupAsync(sessionId, ct);
        if (storedSetup is null)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "SETUP_REQUIRED",
                "Kirim dan kunci pembagian awal dari IDN sebelum memulai sesi"));
        }

        if (storedSetup.RulesetVersionId != activeRuleset.Value.Version.RulesetVersionId)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "INVALID_SESSION_SETUP",
                "Pembagian awal tidak menggunakan set aturan sesi yang aktif"));
        }

        SessionSetupRequest setupRequest;
        try
        {
            setupRequest = SessionStateRepository.DeserializeSetup(storedSetup);
        }
        catch (InvalidOperationException)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "INVALID_SESSION_SETUP",
                "Data pembagian awal tidak dapat dibaca"));
        }

        var setupErrors = SessionSetupValidator.Validate(
            setupRequest,
            activeRuleset.Value.Version.Definition!,
            session.Mode,
            sessionPlayers);
        if (setupErrors.Count > 0)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "INVALID_SESSION_SETUP",
                "Pembagian awal tidak lagi sesuai dengan pemain dan set aturan sesi",
                setupErrors.ToArray()));
        }

        setupRequest = SessionSetupValidator.Normalize(setupRequest);

        var startedAt = DateTimeOffset.UtcNow;
        try
        {
            await _state.StartSessionWithSetupAsync(
                sessionId,
                session.Mode,
                activeRuleset.Value.Version.RulesetVersionId,
                activeRuleset.Value.Version.Definition!,
                setupRequest,
                storedSetup.Revision,
                startedAt,
                ct);
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                ex.Message));
        }

        return Ok(new SessionStatusResponse("STARTED"));
    }

    [HttpPost("{sessionId:guid}/end")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(SessionStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> EndSession(Guid sessionId, CancellationToken ct)
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

        if (!string.Equals(session.Status, "STARTED", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION", "Status sesi tidak valid"));
        }

        var playersInSession = await _players.CountPlayersInSessionAsync(sessionId, ct);
        var activeRuleset = await GetActiveRulesetAsync(session, ct);
        if (activeRuleset is null)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Session belum memiliki ruleset ACTIVE yang valid"));
        }

        var settings = activeRuleset.Value.Version.Definition!.Settings;
        if (playersInSession < settings.MinPlayers || playersInSession > settings.MaxPlayers)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                $"Session membutuhkan {settings.MinPlayers} sampai {settings.MaxPlayers} pemain",
                new ErrorDetail("player_count", "COUNT_OUT_OF_RANGE")));
        }

        await _state.ComputeFinalScoresAsync(sessionId, ct);

        var endedAt = DateTimeOffset.UtcNow;
        await _sessions.UpdateStatusAsync(sessionId, "ENDED", session.StartedAt, endedAt, ct);

        return Ok(new SessionStatusResponse("ENDED"));
    }

    [HttpGet("{sessionId:guid}/state")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(SessionStateResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetState(Guid sessionId, CancellationToken ct)
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

        var state = await _state.GetStateAsync(sessionId, ct);
        if (state is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "State session tidak ditemukan"));
        }

        return Ok(state);
    }

    [HttpPut("{sessionId:guid}/state")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status410Gone)]
    public async Task<IActionResult> SaveState(Guid sessionId, [FromBody] SaveSessionStateRequest request, CancellationToken ct)
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

        return StatusCode(
            StatusCodes.Status410Gone,
            ApiErrorHelper.BuildError(
                HttpContext,
                "STATE_WRITE_DISABLED",
                "State permainan hanya dapat diubah melalui event ingestion"));
    }

    private IActionResult? ValidateStateRequest(
        SaveSessionStateRequest request,
        SessionStateResponse existingState,
        RulesetSectionCatalog catalog)
    {
        if (request.StateVersion < 1)
        {
            return BadRequestError("state_version", "OUT_OF_RANGE", "State version tidak valid");
        }

        if (request.FinishDay < 1 || request.Day < 1 || request.Day > request.FinishDay)
        {
            return BadRequestError("day", "OUT_OF_RANGE", "Hari permainan tidak valid");
        }

        if (request.ActionSlotsLeft < 0 || request.ActionSlotsLeft > 10)
        {
            return BadRequestError("action_slots_left", "OUT_OF_RANGE", "Moves left tidak valid");
        }

        var players = request.Players;
        if (players is null)
        {
            return BadRequestError("players", "REQUIRED", "Daftar pemain wajib diisi");
        }

        if (players.Count != existingState.Players.Count || players.Count is < 2 or > 4)
        {
            return UnprocessableError("players", "COUNT_MISMATCH", "Jumlah pemain tidak sesuai session");
        }

        if (request.Turn < 1 || request.Turn > players.Count)
        {
            return BadRequestError("turn", "OUT_OF_RANGE", "Turn tidak valid");
        }

        var existingPlayerIds = existingState.Players.Select(player => player.SessionPlayerId).ToHashSet();
        var seenPlayerIds = new HashSet<Guid>();
        var seenPlayerIndexes = new HashSet<int>();
        foreach (var player in players)
        {
            if (player.SessionPlayerId == Guid.Empty || !existingPlayerIds.Contains(player.SessionPlayerId))
            {
                return UnprocessableError("players.session_player_id", "UNKNOWN_REFERENCE", "Session player tidak valid");
            }

            if (!seenPlayerIds.Add(player.SessionPlayerId))
            {
                return UnprocessableError("players.session_player_id", "DUPLICATE", "Session player duplikat");
            }

            if (player.PlayerIndex < 1 || player.PlayerIndex > players.Count || !seenPlayerIndexes.Add(player.PlayerIndex))
            {
                return BadRequestError("players.player_order_no", "INVALID_VALUE", "Seat number tidak valid");
            }

            if (string.IsNullOrWhiteSpace(player.Name) || player.Name.Trim().Length > 80)
            {
                return BadRequestError("players.name", "INVALID_VALUE", "Nama pemain tidak valid");
            }

            if (player.Coins < 0 || player.Happiness < 0 || player.Saving < 0 || player.TotalDonasi < 0)
            {
                return UnprocessableError("players", "NEGATIVE_VALUE", "Nilai player tidak boleh negatif");
            }

            var playerValidation = ValidatePlayerChildren(player, catalog);
            if (playerValidation is not null)
            {
                return playerValidation;
            }
        }

        foreach (var donationEvent in request.DonationEvents ?? [])
        {
            if (donationEvent.EventKe < 1 || donationEvent.Day < 1)
            {
                return BadRequestError("donationEvents", "OUT_OF_RANGE", "Event donasi tidak valid");
            }

            var rankingRanks = new HashSet<int>();
            foreach (var ranking in donationEvent.Rankings)
            {
                if (ranking.Rank < 1 || !rankingRanks.Add(ranking.Rank))
                {
                    return BadRequestError("donationEvents.rankings.rank", "INVALID_VALUE", "Ranking donasi tidak valid");
                }

                if (!existingPlayerIds.Contains(ranking.SessionPlayerId))
                {
                    return UnprocessableError("donationEvents.rankings.session_player_id", "UNKNOWN_REFERENCE", "Ranking donasi merujuk player tidak valid");
                }

                if (ranking.TotalDonasi < 0)
                {
                    return UnprocessableError("donationEvents.rankings.total_donasi", "NEGATIVE_VALUE", "Total donasi tidak boleh negatif");
                }
            }
        }

        return null;
    }

    private async Task<(RulesetVersionDb Version, RulesetSettingsDto Settings)?> GetActiveRulesetAsync(
        SessionDb session,
        CancellationToken ct)
    {
        var rulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(session.SessionId, ct);
        if (!rulesetVersionId.HasValue)
        {
            return null;
        }

        var version = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId.Value, ct);
        if (version?.Definition is null ||
            !string.Equals(version.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(version.Mode, session.Mode, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return (version, version.Definition.Settings);
    }

    private IActionResult? ValidatePlayerChildren(SessionPlayerStateDto player, RulesetSectionCatalog catalog)
    {
        foreach (var item in player.Bahan)
        {
            if (string.IsNullOrWhiteSpace(item.Nama) || !catalog.BahanNames.Contains(item.Nama))
            {
                return UnprocessableError("players.bahan.nama", "UNKNOWN_REFERENCE", "Bahan tidak ditemukan di catalog");
            }

            if (item.Jumlah <= 0)
            {
                return BadRequestError("players.bahan.jumlah", "OUT_OF_RANGE", "Jumlah bahan harus lebih dari 0");
            }
        }

        foreach (var item in player.Kebutuhan)
        {
            if (string.IsNullOrWhiteSpace(item.Nama) || !catalog.KebutuhanTypes.TryGetValue(item.Nama, out var expectedTipe))
            {
                return UnprocessableError("players.kebutuhan.nama", "UNKNOWN_REFERENCE", "Kebutuhan tidak ditemukan di catalog");
            }

            if (!string.Equals(expectedTipe, item.Tipe, StringComparison.OrdinalIgnoreCase))
            {
                return UnprocessableError("players.kebutuhan.tipe", "INVALID_REFERENCE", "Tipe kebutuhan tidak sesuai catalog");
            }
        }

        foreach (var item in player.TujuanFinansial)
        {
            if (string.IsNullOrWhiteSpace(item.Nama) || !catalog.TujuanFinansialNames.Contains(item.Nama))
            {
                return UnprocessableError("players.tujuanFinansial.nama", "UNKNOWN_REFERENCE", "Tujuan finansial tidak ditemukan di catalog");
            }

            if (item.CurrentAmount < 0 || item.TargetAmount < 0)
            {
                return UnprocessableError("players.tujuanFinansial", "NEGATIVE_VALUE", "Nilai tujuan finansial tidak boleh negatif");
            }

            var normalizedStatus = string.IsNullOrWhiteSpace(item.Status) ? "ONGOING" : item.Status.Trim().ToUpperInvariant();
            if (normalizedStatus is not ("ONGOING" or "COMPLETED" or "FAILED"))
            {
                return BadRequestError("players.tujuanFinansial.status", "INVALID_ENUM", "Status tujuan finansial tidak valid");
            }

            if (item.PurchasedAtDay.HasValue && item.PurchasedAtDay.Value < 1)
            {
                return BadRequestError("players.tujuanFinansial.purchased_at_day", "OUT_OF_RANGE", "Hari pembelian tujuan tidak valid");
            }

            if ((normalizedStatus is "ONGOING" or "FAILED") && item.PurchasedAtDay.HasValue)
            {
                return UnprocessableError("players.tujuanFinansial.purchased_at_day", "INVALID_REFERENCE", "Hari pembelian hanya boleh diisi saat status COMPLETED");
            }

            if (normalizedStatus == "COMPLETED" && !item.PurchasedAtDay.HasValue)
            {
                return UnprocessableError("players.tujuanFinansial.purchased_at_day", "REQUIRED", "Hari pembelian wajib diisi saat status COMPLETED");
            }
        }

        foreach (var item in player.TargetKebutuhan)
        {
            if (string.IsNullOrWhiteSpace(item.Id) || !catalog.TargetKebutuhanIds.Contains(item.Id))
            {
                return UnprocessableError("players.targetKebutuhan.id", "UNKNOWN_REFERENCE", "Target kebutuhan tidak ditemukan di catalog");
            }
        }

        foreach (var item in player.ActionCounters)
        {
            if (string.IsNullOrWhiteSpace(item.Aksi) || !catalog.Actions.Contains(item.Aksi))
            {
                return UnprocessableError("players.actionCounters.aksi", "UNKNOWN_REFERENCE", "Aksi tidak ditemukan di catalog");
            }

            if (item.Count < 0)
            {
                return UnprocessableError("players.actionCounters.count", "NEGATIVE_VALUE", "Counter aksi tidak boleh negatif");
            }
        }

        return null;
    }

    private bool TryNormalizeMode(string? rawMode, out string mode, out IActionResult? error)
    {
        mode = string.IsNullOrWhiteSpace(rawMode) ? "MAHIR" : rawMode.Trim().ToUpperInvariant();
        error = null;
        if (mode is "PEMULA" or "MAHIR")
        {
            return true;
        }

        error = BadRequestError("mode", "INVALID_ENUM", "Mode tidak valid");
        return false;
    }

    private static bool SessionSetupMatches(SessionSetupRequest first, SessionSetupRequest second)
    {
        var firstJson = JsonSerializer.Serialize(SessionSetupValidator.Normalize(first));
        var secondJson = JsonSerializer.Serialize(SessionSetupValidator.Normalize(second));
        return string.Equals(firstJson, secondJson, StringComparison.Ordinal);
    }

    private static SessionSetupResponse BuildSetupResponse(SessionSetupDb setup, SessionSetupRequest request)
    {
        var normalized = SessionSetupValidator.Normalize(request);
        return new SessionSetupResponse(
            setup.SessionId,
            setup.RulesetVersionId,
            setup.Revision,
            setup.LockedAt.HasValue ? "LOCKED" : "EDITABLE",
            normalized.ClientRequestId,
            normalized.Players,
            setup.SavedAt,
            setup.LockedAt);
    }

    private IActionResult BadRequestError(string field, string issue, string message)
    {
        return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", message, new ErrorDetail(field, issue)));
    }

    private IActionResult UnprocessableError(string field, string issue, string message)
    {
        return UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION", message, new ErrorDetail(field, issue)));
    }

    private string? GetActorName()
    {
        return User.FindFirstValue(ClaimTypes.Name) ??
               User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }

}
