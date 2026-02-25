// Fungsi file: Mengelola alur halaman UI untuk domain PlayersController termasuk komunikasi ke API backend.
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller UI untuk detail pemain pada sesi.
/// </summary>
[Route("sessions/{sessionId:guid}/players")]
public sealed class PlayersController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Menjalankan fungsi PlayersController sebagai bagian dari alur file ini.
    /// </summary>
    public PlayersController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("{playerId:guid}")]
    /// <summary>
    /// Menjalankan fungsi Details sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> Details(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var playerDisplayName = await ResolvePlayerDisplayNameAsync(client, playerId, ct);
        var analyticsResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(analyticsResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!analyticsResponse.IsSuccessStatusCode)
        {
            var error = await analyticsResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return View(new PlayerDetailViewModel
            {
                SessionId = sessionId,
                PlayerId = playerId,
                PlayerDisplayName = playerDisplayName,
                ErrorMessage = error?.Message ?? HttpContext
                    .T("players.error.load_session_analytics_failed")
                    .Replace("{status}", ((int)analyticsResponse.StatusCode).ToString())
            });
        }

        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        var summary = analytics?.ByPlayer.FirstOrDefault(p => p.PlayerId == playerId);

        var txResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/transactions?playerId={playerId}", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(txResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!txResponse.IsSuccessStatusCode)
        {
            var error = await txResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return View(new PlayerDetailViewModel
            {
                SessionId = sessionId,
                PlayerId = playerId,
                PlayerDisplayName = playerDisplayName,
                Summary = summary,
                ErrorMessage = error?.Message ?? HttpContext
                    .T("players.error.load_transactions_failed")
                    .Replace("{status}", ((int)txResponse.StatusCode).ToString())
            });
        }

        var tx = await txResponse.Content.ReadFromJsonAsync<TransactionHistoryResponseDto>(cancellationToken: ct);
        var transactions = tx?.Items ?? new List<TransactionHistoryItemDto>();
        string? gameplayError = null;
        GameplayMetricsResponseDto? gameplay = null;
        var gameplayResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(gameplayResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (gameplayResponse.IsSuccessStatusCode)
        {
            gameplay = await gameplayResponse.Content.ReadFromJsonAsync<GameplayMetricsResponseDto>(cancellationToken: ct);
        }
        else
        {
            var error = await gameplayResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            gameplayError = error?.Message ?? HttpContext
                .T("players.error.load_gameplay_failed")
                .Replace("{status}", ((int)gameplayResponse.StatusCode).ToString());
        }

        var fallbackStartingCash = InferDefaultStartingCash(analytics?.RulesetName);
        var startingCash = TryReadStartingCashFromGameplayRaw(gameplay?.Raw, out var parsedStartingCash)
            ? parsedStartingCash
            : fallbackStartingCash;
        var cashflowJourney = BuildCashflowJourneyStats(transactions, startingCash);

        return View(new PlayerDetailViewModel
        {
            SessionId = sessionId,
            PlayerId = playerId,
            PlayerDisplayName = playerDisplayName,
            Summary = summary,
            Transactions = transactions,
            CashflowJourney = cashflowJourney,
            GameplayRaw = gameplay?.Raw,
            GameplayDerived = gameplay?.Derived,
            GameplayComputedAt = gameplay?.ComputedAt,
            GameplayErrorMessage = gameplayError
        });
    }

    /// <summary>
    /// Menjalankan fungsi ResolvePlayerDisplayNameAsync sebagai bagian dari alur file ini.
    /// </summary>
    private static async Task<string?> ResolvePlayerDisplayNameAsync(HttpClient client, Guid playerId, CancellationToken ct)
    {
        var response = await client.GetAsync("api/v1/players", ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var players = await response.Content.ReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
        return players?.Items.FirstOrDefault(item => item.PlayerId == playerId)?.DisplayName;
    }

    /// <summary>
    /// Menjalankan fungsi BuildCashflowJourneyStats sebagai bagian dari alur file ini.
    /// </summary>
    private static PlayerCashflowJourneyStatsViewModel BuildCashflowJourneyStats(List<TransactionHistoryItemDto> transactions, double startingCash)
    {
        var orderedTransactions = transactions
            .OrderBy(item => item.Timestamp)
            .ToList();

        var labels = new List<string>(orderedTransactions.Count + 1) { "START" };
        var runningBalanceSeries = new List<double>(orderedTransactions.Count + 1) { startingCash };
        var transactionDetails = new List<string>(orderedTransactions.Count + 1) { $"START - OPENING CASH ({startingCash:N0})" };

        var totalCashIn = 0d;
        var totalCashOut = 0d;
        var cashInCount = 0;
        var cashOutCount = 0;
        var runningBalance = startingCash;
        var peakRunningBalance = startingCash;
        var lowestRunningBalance = startingCash;

        foreach (var item in orderedTransactions)
        {
            var direction = item.Direction?.Trim().ToUpperInvariant() ?? string.Empty;
            var category = string.IsNullOrWhiteSpace(item.Category) ? "TRANSACTION" : item.Category.Trim();

            if (string.Equals(item.Direction, "IN", StringComparison.OrdinalIgnoreCase))
            {
                totalCashIn += item.Amount;
                cashInCount += 1;
                runningBalance += item.Amount;
            }
            else if (string.Equals(item.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                totalCashOut += item.Amount;
                cashOutCount += 1;
                runningBalance -= item.Amount;
            }

            labels.Add(item.Timestamp.ToString("dd/MM HH:mm"));
            runningBalanceSeries.Add(runningBalance);
            peakRunningBalance = Math.Max(peakRunningBalance, runningBalance);
            lowestRunningBalance = Math.Min(lowestRunningBalance, runningBalance);
            transactionDetails.Add($"{direction} - {category} ({item.Amount:N0})");
        }

        return new PlayerCashflowJourneyStatsViewModel
        {
            StartingCash = startingCash,
            EndingCash = runningBalance,
            TransactionCount = orderedTransactions.Count,
            CashInCount = cashInCount,
            CashOutCount = cashOutCount,
            TotalCashIn = totalCashIn,
            TotalCashOut = totalCashOut,
            NetCashflow = totalCashIn - totalCashOut,
            PeakRunningNet = peakRunningBalance,
            LowestRunningNet = lowestRunningBalance,
            FirstTransactionAt = orderedTransactions.Count > 0 ? orderedTransactions.First().Timestamp : null,
            LastTransactionAt = orderedTransactions.Count > 0 ? orderedTransactions.Last().Timestamp : null,
            TimelineLabels = labels,
            RunningNetSeries = runningBalanceSeries,
            TransactionDetails = transactionDetails
        };
    }

    /// <summary>
    /// Menjalankan fungsi TryReadStartingCashFromGameplayRaw sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadStartingCashFromGameplayRaw(JsonElement? raw, out double startingCash)
    {
        startingCash = 0;
        if (!raw.HasValue || raw.Value.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        if (!raw.Value.TryGetProperty("coins", out var coins) || coins.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        if (!coins.TryGetProperty("starting_coins", out var startingProp))
        {
            return false;
        }

        return startingProp.ValueKind switch
        {
            JsonValueKind.Number => startingProp.TryGetDouble(out startingCash),
            JsonValueKind.String => double.TryParse(startingProp.GetString(), out startingCash),
            _ => false
        };
    }

    /// <summary>
    /// Menjalankan fungsi InferDefaultStartingCash sebagai bagian dari alur file ini.
    /// </summary>
    private static double InferDefaultStartingCash(string? rulesetName)
    {
        if (string.IsNullOrWhiteSpace(rulesetName))
        {
            return 20d;
        }

        var normalized = rulesetName.Trim().ToLowerInvariant();
        if (normalized.Contains("mahir", StringComparison.Ordinal) ||
            normalized.Contains("advanced", StringComparison.Ordinal))
        {
            return 10d;
        }

        return 20d;
    }
}

