// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk PlayersController.
using System.Net.Http.Json;
using System.Security.Claims;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("sessions/{sessionId:guid}/players")]
public sealed class PlayersController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public PlayersController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("{playerId:guid}")]
    public async Task<IActionResult> Details(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        if (!HttpContext.IsInstructor())
        {
            var currentUserIdRaw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserIdRaw, out var currentUserId) || currentUserId != playerId)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

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
            var error = await analyticsResponse.Content.TryReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);
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

        var analytics = await analyticsResponse.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>(cancellationToken: ct);
        var summary = analytics?.ByPlayer.FirstOrDefault(p => p.UserId == playerId);

        var txResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/transactions?userId={playerId}&limit=100", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(txResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!txResponse.IsSuccessStatusCode)
        {
            var error = await txResponse.Content.TryReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);
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

        var tx = await txResponse.Content.TryReadFromJsonAsync<TransactionHistoryResponse>(cancellationToken: ct);
        var transactions = tx?.Items ?? new List<TransactionHistoryItem>();
        while (tx?.HasMore == true && !string.IsNullOrWhiteSpace(tx.NextCursor))
        {
            txResponse = await client.GetAsync(
                $"api/v1/analytics/sessions/{sessionId}/transactions?userId={playerId}&limit=100&cursor={Uri.EscapeDataString(tx.NextCursor)}",
                ct);
            if (!txResponse.IsSuccessStatusCode)
            {
                break;
            }

            tx = await txResponse.Content.TryReadFromJsonAsync<TransactionHistoryResponse>(cancellationToken: ct);
            if (tx is not null)
            {
                transactions.AddRange(tx.Items);
            }
        }
        string? gameplayError = null;
        GameplayMetricsResponse? gameplay = null;
        var gameplayResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(gameplayResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (gameplayResponse.IsSuccessStatusCode)
        {
            gameplay = await gameplayResponse.Content.TryReadFromJsonAsync<GameplayMetricsResponse>(cancellationToken: ct);
        }
        else
        {
            var error = await gameplayResponse.Content.TryReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);
            gameplayError = error?.Message ?? HttpContext
                .T("players.error.load_gameplay_failed")
                .Replace("{status}", ((int)gameplayResponse.StatusCode).ToString());
        }

        var fallbackStartingCash = InferDefaultStartingCash(analytics?.RulesetName);
        var startingCash = gameplay?.Economy.StartingCash ?? fallbackStartingCash;
        var cashflowJourney = BuildCashflowJourneyStats(transactions, startingCash);
        var statSummary = PlayerStatSummaryBuilder.Build(gameplay, summary, cashflowJourney, HttpContext.T);

        return View(new PlayerDetailViewModel
        {
            SessionId = sessionId,
            PlayerId = playerId,
            PlayerDisplayName = playerDisplayName,
            Summary = summary,
            StatSummary = statSummary,
            CashflowJourney = cashflowJourney,
            GameplayRaw = gameplay?.RawJson,
            GameplayDerived = gameplay?.DerivedJson,
            GameplayComputedAt = gameplay?.ComputedAt,
            GameplayErrorMessage = gameplayError
        });
    }

    private static async Task<string?> ResolvePlayerDisplayNameAsync(HttpClient client, Guid playerId, CancellationToken ct)
    {
        var response = await client.GetAsync("api/v1/players", ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var players = await response.Content.TryReadFromJsonAsync<PlayerListResponse>(cancellationToken: ct);
        return players?.Items.FirstOrDefault(item => item.UserId == playerId)?.DisplayName;
    }

    private static PlayerCashflowJourneyStatsViewModel BuildCashflowJourneyStats(List<TransactionHistoryItem> transactions, double startingCash)
    {
        var orderedTransactions = transactions
            .OrderBy(item => item.Timestamp)
            .ToList();

        var labels = new List<string>(orderedTransactions.Count + 1) { "START" };
        var runningBalanceSeries = new List<double>(orderedTransactions.Count + 1) { startingCash };
        var transactionDetails = new List<string>(orderedTransactions.Count + 1) { $"START - OPENING_CASH ({startingCash:N0})" };

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
