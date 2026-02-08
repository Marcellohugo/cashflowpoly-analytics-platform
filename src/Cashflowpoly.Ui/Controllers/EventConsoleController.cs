using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Controllers;

[Route("events")]
public sealed class EventConsoleController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public EventConsoleController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View(new EventConsoleViewModel
        {
            EventJson = EventConsoleViewModel.BuildDefaultSample()
        });
    }

    [HttpPost("")]
    public async Task<IActionResult> Index(EventConsoleViewModel model, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(model.EventJson))
        {
            model.ErrorMessage = "JSON event wajib diisi.";
            return View(model);
        }

        Dictionary<string, object?> payload;
        try
        {
            payload = JsonSerializer.Deserialize<Dictionary<string, object?>>(model.EventJson) ??
                      new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            model.ErrorMessage = "JSON tidak valid. Pastikan format sudah benar.";
            return View(model);
        }

        var normalized = await NormalizeEventAsync(payload, model, ct);
        if (!string.IsNullOrWhiteSpace(model.ErrorMessage))
        {
            return View(model);
        }

        model.EventJson = normalized;
        var client = _clientFactory.CreateClient("Api");
        using var content = new StringContent(normalized, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/events", content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            model.ErrorMessage = $"Gagal menyimpan event. Status: {(int)response.StatusCode}";
            model.ResponseJson = responseBody;
            return View(model);
        }

        model.SuccessMessage = "Event berhasil disimpan.";
        model.ResponseJson = responseBody;
        return View(model);
    }

    private async Task<string> NormalizeEventAsync(Dictionary<string, object?> data, EventConsoleViewModel model, CancellationToken ct)
    {
        if (!TryReadGuid(data, "session_id", out var sessionId) || sessionId == Guid.Empty)
        {
            model.ErrorMessage = "session_id wajib diisi.";
            return model.EventJson;
        }

        if (!TryReadGuid(data, "event_id", out var eventId) || eventId == Guid.Empty)
        {
            data["event_id"] = Guid.NewGuid();
        }

        if (!TryReadDateTimeOffset(data, "timestamp", out _))
        {
            data["timestamp"] = DateTimeOffset.UtcNow;
        }

        var client = _clientFactory.CreateClient("Api");
        var lastEvent = await GetLastEventAsync(client, sessionId, ct);

        if (!TryReadGuid(data, "ruleset_version_id", out var rulesetVersionId) || rulesetVersionId == Guid.Empty)
        {
            if (lastEvent is null)
            {
                model.ErrorMessage = "ruleset_version_id wajib diisi (belum ada event sebelumnya di sesi ini).";
                return model.EventJson;
            }

            data["ruleset_version_id"] = lastEvent.RulesetVersionId;
        }

        if (!TryReadLong(data, "sequence_number", out var sequenceNumber) || sequenceNumber <= 0)
        {
            var nextSequence = lastEvent is null ? 1 : lastEvent.SequenceNumber + 1;
            data["sequence_number"] = nextSequence;
        }

        return JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private static bool TryReadGuid(Dictionary<string, object?> data, string key, out Guid value)
    {
        value = Guid.Empty;
        if (!data.TryGetValue(key, out var raw) || raw is null)
        {
            return false;
        }

        if (raw is Guid guid)
        {
            value = guid;
            return true;
        }

        if (raw is string text && Guid.TryParse(text, out guid))
        {
            value = guid;
            return true;
        }

        if (raw is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String &&
                Guid.TryParse(element.GetString(), out guid))
            {
                value = guid;
                return true;
            }
        }

        return false;
    }

    private static bool TryReadLong(Dictionary<string, object?> data, string key, out long value)
    {
        value = 0;
        if (!data.TryGetValue(key, out var raw) || raw is null)
        {
            return false;
        }

        if (raw is long longValue)
        {
            value = longValue;
            return true;
        }

        if (raw is int intValue)
        {
            value = intValue;
            return true;
        }

        if (raw is string text && long.TryParse(text, out var parsed))
        {
            value = parsed;
            return true;
        }

        if (raw is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var number))
            {
                value = number;
                return true;
            }

            if (element.ValueKind == JsonValueKind.String && long.TryParse(element.GetString(), out var numberText))
            {
                value = numberText;
                return true;
            }
        }

        return false;
    }

    private static bool TryReadDateTimeOffset(Dictionary<string, object?> data, string key, out DateTimeOffset value)
    {
        value = default;
        if (!data.TryGetValue(key, out var raw) || raw is null)
        {
            return false;
        }

        if (raw is DateTimeOffset dto)
        {
            value = dto;
            return true;
        }

        if (raw is string text && DateTimeOffset.TryParse(text, out var parsed))
        {
            value = parsed;
            return true;
        }

        if (raw is JsonElement element && element.ValueKind == JsonValueKind.String &&
            DateTimeOffset.TryParse(element.GetString(), out parsed))
        {
            value = parsed;
            return true;
        }

        return false;
    }

    private static async Task<EventRequestDto?> GetLastEventAsync(HttpClient client, Guid sessionId, CancellationToken ct)
    {
        const int pageSize = 200;
        long fromSeq = 0;
        EventRequestDto? last = null;

        while (true)
        {
            var response = await client.GetAsync($"api/sessions/{sessionId}/events?fromSeq={fromSeq}&limit={pageSize}", ct);
            if (!response.IsSuccessStatusCode)
            {
                return last;
            }

            var payload = await response.Content.ReadFromJsonAsync<EventsBySessionResponseDto>(cancellationToken: ct);
            var events = payload?.Events ?? new List<EventRequestDto>();
            if (events.Count == 0)
            {
                return last;
            }

            last = events[^1];
            fromSeq = last.SequenceNumber + 1;

            if (events.Count < pageSize)
            {
                return last;
            }
        }
    }
}
