using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller UI untuk halaman sesi.
/// </summary>
[Route("sessions")]
public sealed class SessionsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IWebHostEnvironment _environment;

    public SessionsController(IHttpClientFactory clientFactory, IWebHostEnvironment environment)
    {
        _clientFactory = clientFactory;
        _environment = environment;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/sessions", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return View(new SessionListViewModel
            {
                ErrorMessage = $"Gagal mengambil daftar sesi. Status: {(int)response.StatusCode}"
            });
        }

        var data = await response.Content.ReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
        return View(new SessionListViewModel
        {
            Items = data?.Items ?? new List<SessionListItemDto>()
        });
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> Details(Guid sessionId, CancellationToken ct)
    {
        var detail = await BuildSessionDetailViewModel(sessionId, ct);
        return detail.Result ?? View(detail.Model);
    }

    [HttpPost("{sessionId:guid}/end")]
    public async Task<IActionResult> End(Guid sessionId, CancellationToken ct)
    {
        if (!_environment.IsDevelopment() || !HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.PostAsync($"api/v1/sessions/{sessionId}/end", null, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        var apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
        var detail = await BuildSessionDetailViewModel(
            sessionId,
            ct,
            apiError?.Message ?? $"Gagal menyelesaikan sesi. Status: {(int)response.StatusCode}");
        return detail.Result ?? View("Details", detail.Model);
    }

    [HttpGet("{sessionId:guid}/ruleset")]
    public async Task<IActionResult> Ruleset(Guid sessionId, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/rulesets", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return View(new SessionRulesetViewModel
            {
                SessionId = sessionId,
                ErrorMessage = $"Gagal memuat ruleset. Status: {(int)response.StatusCode}"
            });
        }

        var data = await response.Content.ReadFromJsonAsync<RulesetListResponseDto>(cancellationToken: ct);
        return View(new SessionRulesetViewModel
        {
            SessionId = sessionId,
            Rulesets = data?.Items ?? new List<RulesetListItemDto>()
        });
    }

    [HttpPost("{sessionId:guid}/ruleset")]
    public async Task<IActionResult> Ruleset(Guid sessionId, SessionRulesetViewModel model, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        if (model.SelectedRulesetId is null || model.SelectedVersion is null)
        {
            model.ErrorMessage = "Ruleset dan versi wajib dipilih.";
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new
        {
            ruleset_id = model.SelectedRulesetId,
            version = model.SelectedVersion
        };

        var response = await client.PostAsJsonAsync($"api/v1/sessions/{sessionId}/ruleset/activate", payload, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            model.ErrorMessage = error?.Message ?? $"Gagal aktivasi ruleset. Status: {(int)response.StatusCode}";
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { sessionId });
    }

    private async Task<(SessionDetailViewModel Model, IActionResult? Result)> BuildSessionDetailViewModel(
        Guid sessionId,
        CancellationToken ct,
        string? overrideErrorMessage = null)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return (new SessionDetailViewModel
            {
                SessionId = sessionId,
                IsDevelopment = _environment.IsDevelopment()
            }, unauthorized);
        }

        var sessionStatus = await GetSessionStatusAsync(client, sessionId, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return (new SessionDetailViewModel
            {
                SessionId = sessionId,
                SessionStatus = sessionStatus,
                IsDevelopment = _environment.IsDevelopment(),
                ErrorMessage = overrideErrorMessage ?? error?.Message ?? $"Gagal memuat detail sesi. Status: {(int)response.StatusCode}"
            }, null);
        }

        var analytics = await response.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        var (timeline, timelineError) = await LoadTimelineAsync(client, sessionId, ct);
        if (!HttpContext.Session.IsInstructor() && analytics is not null && analytics.ByPlayer.Count > 0)
        {
            var scopedPlayerIds = analytics.ByPlayer.Select(item => item.PlayerId).ToHashSet();
            timeline = timeline
                .Where(item => !item.PlayerId.HasValue || scopedPlayerIds.Contains(item.PlayerId.Value))
                .ToList();
        }

        return (new SessionDetailViewModel
        {
            SessionId = sessionId,
            SessionStatus = sessionStatus,
            IsDevelopment = _environment.IsDevelopment(),
            Analytics = analytics,
            Timeline = timeline,
            TimelineErrorMessage = timelineError,
            ErrorMessage = overrideErrorMessage
        }, null);
    }

    private static async Task<string?> GetSessionStatusAsync(HttpClient client, Guid sessionId, CancellationToken ct)
    {
        var sessionResponse = await client.GetAsync("api/v1/sessions", ct);
        if (!sessionResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var data = await sessionResponse.Content.ReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
        return data?.Items.FirstOrDefault(x => x.SessionId == sessionId)?.Status;
    }

    private static async Task<(List<SessionTimelineEventViewModel> Timeline, string? ErrorMessage)> LoadTimelineAsync(
        HttpClient client,
        Guid sessionId,
        CancellationToken ct)
    {
        var response = await client.GetAsync($"api/v1/sessions/{sessionId}/events?fromSeq=0&limit=1000", ct);
        if (!response.IsSuccessStatusCode)
        {
            return (new List<SessionTimelineEventViewModel>(), $"Gagal memuat alur event sesi. Status: {(int)response.StatusCode}");
        }

        var data = await response.Content.ReadFromJsonAsync<EventsBySessionResponseDto>(cancellationToken: ct);
        if (data?.Events is null || data.Events.Count == 0)
        {
            return (new List<SessionTimelineEventViewModel>(), null);
        }

        var timeline = data.Events
            .OrderBy(x => x.Timestamp)
            .ThenBy(x => x.SequenceNumber)
            .Select(x =>
            {
                var flowLabel = ResolveFlowLabel(x.ActionType);
                var flowDescription = BuildFlowDescription(x.ActionType, x.Payload);
                return new SessionTimelineEventViewModel
                {
                    Timestamp = x.Timestamp,
                    SequenceNumber = x.SequenceNumber,
                    DayIndex = x.DayIndex,
                    Weekday = x.Weekday,
                    TurnNumber = x.TurnNumber,
                    ActorType = x.ActorType,
                    PlayerId = x.PlayerId,
                    ActionType = x.ActionType,
                    FlowLabel = flowLabel,
                    FlowDescription = flowDescription
                };
            })
            .ToList();

        return (timeline, null);
    }

    private static string ResolveFlowLabel(string actionType)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            return "Aktivitas";
        }

        var lower = actionType.ToLowerInvariant();
        if (lower.StartsWith("setup."))
        {
            return "Setup";
        }

        if (lower.StartsWith("day."))
        {
            return "Event Harian";
        }

        if (lower.StartsWith("risk."))
        {
            return "Risiko";
        }

        if (lower.StartsWith("loan."))
        {
            return "Pembiayaan";
        }

        if (lower.StartsWith("saving."))
        {
            return "Tabungan";
        }

        if (lower.StartsWith("mission."))
        {
            return "Misi";
        }

        if (lower.Contains("order"))
        {
            return "Order";
        }

        if (lower.Contains("purchased"))
        {
            return "Belanja";
        }

        return "Aktivitas";
    }

    private static string BuildFlowDescription(string actionType, JsonElement payload)
    {
        var text = actionType switch
        {
            "transaction.recorded" => "Transaksi kas pemain dicatat.",
            "day.friday.donation" => "Pemain melakukan donasi khusus hari Jumat.",
            "day.saturday.gold_trade" => "Pemain melakukan jual/beli emas saat event Sabtu.",
            "ingredient.purchased" => "Pemain membeli bahan untuk kebutuhan/order.",
            "order.claimed" => "Pemain menyelesaikan order dan menerima hasil.",
            "work.freelance.completed" => "Pemain menyelesaikan kerja freelance dan mendapat pemasukan.",
            "need.primary.purchased" => "Pemain membeli kebutuhan primer.",
            "need.secondary.purchased" => "Pemain membeli kebutuhan sekunder.",
            "need.tertiary.purchased" => "Pemain membeli kebutuhan tersier.",
            "saving.deposit.created" => "Pemain menaruh dana ke tabungan tujuan.",
            "saving.deposit.withdrawn" => "Pemain menarik dana dari tabungan tujuan.",
            "saving.goal.achieved" => "Pemain mencapai target tabungan.",
            "loan.syariah.taken" => "Pemain mengambil pinjaman syariah.",
            "loan.syariah.repaid" => "Pemain melakukan cicilan/pelunasan pinjaman.",
            "risk.life.drawn" => "Pemain terkena efek kartu risiko.",
            "risk.emergency.used" => "Pemain menggunakan opsi mitigasi risiko darurat.",
            "insurance.multirisk.purchased" => "Pemain membeli proteksi asuransi.",
            "insurance.multirisk.used" => "Pemain mengaktifkan proteksi asuransi.",
            _ => $"Aksi `{actionType}` dieksekusi pada sesi."
        };

        var payloadSummary = BuildPayloadSummary(payload);
        return string.IsNullOrWhiteSpace(payloadSummary) ? text : $"{text} Detail: {payloadSummary}";
    }

    private static string BuildPayloadSummary(JsonElement payload)
    {
        if (payload.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        var keyOrder = new[]
        {
            "amount",
            "direction",
            "category",
            "trade_type",
            "qty",
            "unit_price",
            "card_id",
            "goal_id",
            "loan_id",
            "risk_id",
            "points"
        };

        var parts = new List<string>();
        foreach (var key in keyOrder)
        {
            if (!payload.TryGetProperty(key, out var value))
            {
                continue;
            }

            parts.Add($"{key}: {JsonElementToInlineText(value)}");
            if (parts.Count == 4)
            {
                break;
            }
        }

        return string.Join(", ", parts);
    }

    private static string JsonElementToInlineText(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.ToString(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Array => $"[{value.GetArrayLength()} item]",
            JsonValueKind.Object => "{...}",
            _ => string.Empty
        };
    }
}

