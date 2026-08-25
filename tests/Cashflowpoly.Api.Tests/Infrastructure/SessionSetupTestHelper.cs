// Fungsi file: Menyiapkan payload pembagian awal deterministik untuk pengujian integrasi.
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Tests.Infrastructure;

internal static class SessionSetupTestHelper
{
    public static async Task<HttpResponseMessage> SaveAsync(
        HttpClient client,
        string accessToken,
        Guid sessionId,
        RulesetDefinitionDto definition,
        CancellationToken ct)
    {
        using var listRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/sessions/{sessionId}/players");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var listResponse = await client.SendAsync(listRequest, ct);
        listResponse.EnsureSuccessStatusCode();
        var playerList = await listResponse.Content.ReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Daftar peserta sesi tidak dapat dibaca oleh test.");
        var players = playerList.Items.OrderBy(item => item.PlayerOrder).ToList();

        var tieBreakers = definition.TieBreakers.OrderBy(item => item.TieNumber).Take(players.Count).ToList();
        var ingredients = Expand(
            definition.Ingredients,
            item => item.CardQty ?? 5,
            players.Count);
        var missions = definition.CollectionMissions.Take(players.Count).ToList();
        var loans = definition.Settings.LoanEnabled
            ? Expand(
                definition.ShariaLoans,
                item => item.CardQty ?? 1,
                players.Count,
                $"pinjaman (catalog={definition.ShariaLoans.Count}; quantities={string.Join(',', definition.ShariaLoans.Select(item => item.CardQty))})")
            : [];
        var insurance = definition.Settings.InsuranceEnabled
            ? Enumerable.Repeat(
                    definition.InsuranceProducts.FirstOrDefault()
                        ?? throw new InvalidOperationException("Ruleset test tidak memiliki produk asuransi awal."),
                    players.Count)
                .ToList()
            : [];

        if (tieBreakers.Count != players.Count || missions.Count != players.Count)
        {
            throw new InvalidOperationException("Ruleset test tidak memiliki cukup kartu pembagian awal.");
        }

        var assignments = players.Select((player, index) => new SessionPlayerSetupRequest(
            player.SessionPlayerId,
            tieBreakers[index].TieBreakerCode,
            ingredients[index].Id,
            1,
            missions[index].Id,
            definition.Settings.LoanEnabled ? loans[index].LoanCode : null,
            definition.Settings.InsuranceEnabled ? insurance[index].ProductCode : null)).ToList();
        var setup = new SessionSetupRequest($"test-setup-{sessionId:N}", assignments);

        var saveRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/sessions/{sessionId}/setup");
        saveRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        saveRequest.Content = JsonContent.Create(setup);
        return await client.SendAsync(saveRequest, ct);
    }

    private static List<T> Expand<T>(
        IEnumerable<T> source,
        Func<T, int> getQuantity,
        int required,
        string cardName = "kartu")
    {
        var cards = source
            .SelectMany(item => Enumerable.Repeat(item, Math.Max(0, getQuantity(item))))
            .Take(required)
            .ToList();
        if (cards.Count != required)
        {
            throw new InvalidOperationException(
                $"Ruleset test hanya memiliki {cards.Count} {cardName} pembagian awal; dibutuhkan {required}.");
        }

        return cards;
    }
}
