// Fungsi file: Menguji pembaruan harga untuk setiap kartu emas melalui API dan validasi database.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class GoldRiskPriceRefreshIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Fact]
    public async Task TwoGoldRiskCardsOnMonday_RefreshPriceAndLetEveryPlayerTradeAtNewPrice()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest($"goldrisk_{suffix}", "IntegrationGold!2026", "INSTRUCTOR", "Gold instructor"));
        registration.EnsureSuccessStatusCode();
        var account = (await registration.Content.ReadFromJsonAsync<RegisterResponse>())!;
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR");
        definition.LifeRisks.Clear();
        definition.LifeRisks.Add(new RulesetLifeRiskDto
        {
            RiskCode = "risk_investasi_emas", ItemName = "Investasi Emas", EffectType = "GOLD_TRADE",
            TargetScope = "ALL_PLAYERS", DurationDays = 1, Amount = 0, CardQty = 2
        });
        definition.LifeRisks.Add(new RulesetLifeRiskDto
        {
            RiskCode = "risk_emergency_cost", ItemName = "Biaya darurat", EffectType = "COIN_EFFECT",
            Direction = "OUT", TargetScope = "SELF", DurationDays = 1, Amount = 100, CardQty = 1
        });
        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, suffix, definition);
        await using var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await db.OpenAsync();
        var players = (await db.QueryAsync<(Guid ParticipantId, Guid UserId, int TurnNumber)>("""
            select session_participant_id, user_id, player_order_no
            from session_participants where session_id=@SessionId order by player_order_no
            """, new { setup.SessionId })).ToList();
        var sequence = setup.NextSequenceNumber;
        var firstOrder = await Send("JualMasakan", new { order_card_id = "nasi_goreng" }, 0, slot: 1);
        var firstRisk = await Send("RisikoKehidupan",
            new { risk_id = "risk_investasi_emas", source_order_event_id = firstOrder }, 0);
        var firstPrice = await Send("BukaHargaEmas", new { gold_price = 5 });
        var secondOrder = await Send("JualMasakan", new { order_card_id = "nasi_goreng" }, 0, slot: 2);
        var secondRisk = await Send("RisikoKehidupan",
            new { risk_id = "risk_investasi_emas", source_order_event_id = secondOrder }, 0);

        // Neither the latest card nor a still-active older card permits stale-price trades.
        foreach (var riskId in new[] { firstRisk, secondRisk })
        {
            await Send("InvestasiEmas", new { qty = 1, unit_price = 5, amount = 5, risk_event_id = riskId }, 1,
                expected: HttpStatusCode.UnprocessableEntity);
            await Send("JualEmas", new { qty = 1, unit_price = 5, amount = 5, risk_event_id = riskId }, 1,
                expected: HttpStatusCode.UnprocessableEntity);
        }
        await AssertDatabaseRejects("InvestasiEmas",
            new { qty = 1, unit_price = 5, amount = 5, risk_event_id = secondRisk }, 1,
            "Gold trade requires BukaHargaEmas");

        // Emergency gold sales must also wait for the newly drawn card's price refresh.
        var emergencyOrder = await Send("JualMasakan", new { order_card_id = "nasi_goreng" }, 1, slot: 1);
        var emergencyRisk = await Send("RisikoKehidupan",
            new { risk_id = "risk_emergency_cost", source_order_event_id = emergencyOrder }, 1);
        await Send("GunakanOpsiDarurat",
            new { option_type = "SELL_GOLD", qty = 1, risk_event_id = emergencyRisk, gold_price_event_id = firstPrice }, 1,
            expected: HttpStatusCode.UnprocessableEntity, errorText: "Harga emas tidak berasal");
        await AssertDatabaseRejects("GunakanOpsiDarurat", new
        {
            option_type = "SELL_GOLD", qty = 1, unit_price = 5, amount = 5,
            risk_event_id = emergencyRisk, gold_price_event_id = firstPrice
        }, 1, "Gold trade requires BukaHargaEmas");

        var secondPrice = await Send("BukaHargaEmas", new { gold_price = 6 });
        await Send("BukaHargaEmas", new { gold_price = 5 }, expected: HttpStatusCode.UnprocessableEntity);
        await AssertDatabaseRejects("BukaHargaEmas", new { gold_price = 5 }, null, "Gold price is already opened");
        var emergencyBefore = await db.ExecuteScalarAsync<int>(
            "select coins from session_participant_balances where session_participant_id=@ParticipantId",
            new { players[1].ParticipantId });
        var emergencySale = await Send("GunakanOpsiDarurat",
            new { option_type = "SELL_GOLD", qty = 1, risk_event_id = emergencyRisk, gold_price_event_id = secondPrice }, 1);
        Assert.Equal(emergencyBefore + 6, await db.ExecuteScalarAsync<int>(
            "select coins from session_participant_balances where session_participant_id=@ParticipantId",
            new { players[1].ParticipantId }));
        Assert.Equal(6, await db.ExecuteScalarAsync<int>(
            "select amount from event_cashflow_projections where session_id=@SessionId and event_id=@emergencySale and direction='IN'",
            new { setup.SessionId, emergencySale }));
        await Send("Asuransi", new { risk_event_id = emergencyRisk }, 1);
        await Send("InvestasiEmas", new { qty = 1, unit_price = 5, amount = 5, risk_event_id = secondRisk }, 1,
            expected: HttpStatusCode.UnprocessableEntity);

        var balancesBefore = (await db.QueryAsync<int>("""
            select b.coins from session_participant_balances b
            join session_participants p using(session_id, session_participant_id)
            where p.session_id=@SessionId order by p.player_order_no
            """, new { setup.SessionId })).ToArray();
        for (var index = 0; index < players.Count; index++)
        {
            await Send("InvestasiEmas", new { qty = 1, unit_price = 6, amount = 6, risk_event_id = secondRisk }, index);
            await Send("JualEmas", new { qty = 1, unit_price = 6, amount = 6, risk_event_id = secondRisk }, index);
        }
        Assert.Equal(balancesBefore, (await db.QueryAsync<int>("""
            select b.coins from session_participant_balances b
            join session_participants p using(session_id, session_participant_id)
            where p.session_id=@SessionId order by p.player_order_no
            """, new { setup.SessionId })).ToArray());
        Assert.Equal(2, await db.ExecuteScalarAsync<int>("""
            select count(*)::int from events where session_id=@SessionId and action_type='BukaHargaEmas'
            """, new { setup.SessionId }));

        async Task<Guid> Send(string action, object payload, int? player = null, int slot = 0,
            HttpStatusCode expected = HttpStatusCode.Created, string? errorText = null)
        {
            var eventId = Guid.NewGuid();
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/events");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken);
            request.Content = JsonContent.Create(new
            {
                event_id = eventId, session_id = setup.SessionId,
                user_id = player.HasValue ? players[player.Value].UserId : (Guid?)null,
                actor_type = player.HasValue ? "PLAYER" : "SYSTEM", timestamp = DateTimeOffset.UtcNow,
                day_index = 1, weekday = "MON", turn_number = player.HasValue ? players[player.Value].TurnNumber : 0,
                action_slot = slot, sequence_number = sequence, action_type = action,
                ruleset_version_id = setup.RulesetVersionId, payload
            });
            using var response = await fixture.Client.SendAsync(request, TestContext.Current.CancellationToken);
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == expected, $"{action}: {content}");
            if (errorText is not null) Assert.Contains(errorText, content);
            if (response.IsSuccessStatusCode) sequence++;
            return eventId;
        }

        async Task AssertDatabaseRejects(string action, object payload, int? player, string message)
        {
            Assert.Equal(1, await db.ExecuteScalarAsync<int>("select count(*) from ruleset_actions where ruleset_version_id=@RulesetVersionId and action_id=@action",
                new { setup.RulesetVersionId, action }));
            var error = await Assert.ThrowsAsync<PostgresException>(async () => await db.ExecuteAsync("""
                insert into events (event_id, session_id, session_player_id, user_id, actor_type,
                    timestamp, day_index, weekday, turn_number, action_slot, sequence_number,
                    ruleset_action_id, action_type, ruleset_version_id, payload)
                select @EventId, @SessionId, @ParticipantId, @UserId, @ActorType,
                    now(), 1, 'MON', @TurnNumber, 0, @SequenceNumber,
                    ra.ruleset_action_id, @ActionType, @RulesetVersionId, @Payload::jsonb
                from ruleset_actions ra
                where ra.ruleset_version_id=@RulesetVersionId and ra.action_id=@ActionType
                """, new
            {
                EventId = Guid.NewGuid(), setup.SessionId, setup.RulesetVersionId,
                ParticipantId = player.HasValue ? players[player.Value].ParticipantId : (Guid?)null,
                UserId = player.HasValue ? players[player.Value].UserId : (Guid?)null,
                ActorType = player.HasValue ? "PLAYER" : "SYSTEM",
                TurnNumber = player.HasValue ? players[player.Value].TurnNumber : 0,
                SequenceNumber = sequence, ActionType = action, Payload = JsonSerializer.Serialize(payload)
            }));
            Assert.Equal(PostgresErrorCodes.CheckViolation, error.SqlState);
            Assert.Contains(message, error.MessageText);
        }
    }
}
