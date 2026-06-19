using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Tests.Infrastructure;
using Cashflowpoly.Api.Contracts;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class ManualSimulationSeedIntegrationTests
{
    private const string JwtSigningKey = "integration-test-signing-key-with-min-32-char";
    private const string SeedInstructorUsername = "rina.kartika";
    private const string SeedInstructorPassword = "SeedLocal!2026";
    private const string SeedPemulaSessionName = "Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A";
    private const string SeedMahirSessionName = "Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B";
    [Fact]
    public async Task ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions()
    {
        await using var database = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_manual_seed_boot")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();

        await database.StartAsync();

        await using (var setupConnection = new NpgsqlConnection(database.GetConnectionString()))
        {
            await setupConnection.OpenAsync();
            await setupConnection.ExecuteAsync(await File.ReadAllTextAsync(Path.Combine(RepoRoot, "database", "00_create_schema.sql")));
            await setupConnection.ExecuteAsync(await File.ReadAllTextAsync(Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql")));
            await setupConnection.ExecuteAsync(await File.ReadAllTextAsync(Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql")));
        }

        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        {
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var healthResponse = await client.GetAsync("/health/ready");
            Assert.Equal(HttpStatusCode.OK, healthResponse.StatusCode);

            var loginResponse = await client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new LoginRequest(SeedInstructorUsername, SeedInstructorPassword));
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            Assert.NotNull(loginBody);
            Assert.Equal("INSTRUCTOR", loginBody.Role);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginBody.AccessToken);

            var seedMahirSessionId = Guid.Parse("91000000-0000-0000-0000-000000000002");
            var seedPlayer1UserId = Guid.Parse("90000000-0000-0000-0000-000000000011");
            var txResponse = await client.GetAsync($"/api/v1/analytics/sessions/{seedMahirSessionId}/transactions?userId={seedPlayer1UserId}");
            Assert.Equal(HttpStatusCode.OK, txResponse.StatusCode);

            var txBody = await txResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
            Assert.NotNull(txBody);
            Assert.NotEmpty(txBody.Items);

            var gameplayResponse = await client.GetAsync($"/api/v1/analytics/sessions/{seedMahirSessionId}/players/{seedPlayer1UserId}/gameplay");
            Assert.Equal(HttpStatusCode.OK, gameplayResponse.StatusCode);

            var gameplayBody = await gameplayResponse.Content.ReadFromJsonAsync<GameplayMetricsResponse>();
            Assert.NotNull(gameplayBody);
            Assert.True(gameplayBody.Economy.StartingCash > 0);
            Assert.True(gameplayBody.Economy.CashInTotal >= 0);
            Assert.True(gameplayBody.Progress.ActionsUsedTotal >= 0);
        });

        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();

        var sessions = (await connection.QueryAsync<SeedSessionRow>(
            """
            select session_id, session_name, status, mode
            from sessions
            where session_name in (@pemulaSessionName, @mahirSessionName)
            order by session_name asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToList();

        Assert.Equal(2, sessions.Count);
        Assert.All(sessions, session => Assert.Equal("ENDED", session.Status));

        var playerCounts = (await connection.QueryAsync<SessionPlayerCountRow>(
            """
            select s.session_name, count(*)::int as player_count
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToDictionary(row => row.SessionName, row => row.PlayerCount);

        Assert.Equal(4, playerCounts[SeedPemulaSessionName]);
        Assert.Equal(4, playerCounts[SeedMahirSessionName]);

        var sequenceChecks = (await connection.QueryAsync<SequenceCheckRow>(
            """
            select
                s.session_name,
                min(e.sequence_number)::bigint as min_sequence,
                max(e.sequence_number)::bigint as max_sequence,
                count(*)::int as event_count
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToList();

        Assert.All(sequenceChecks, row =>
        {
            Assert.Equal(0, row.MinSequence);
            Assert.Equal(row.EventCount - 1, row.MaxSequence);
        });

        var rulesetChecks = (await connection.QueryAsync<RulesetCheckRow>(
            """
            select
                s.session_name,
                max((s.ruleset_version_id)::text) as activated_ruleset_version_id,
                min((e.ruleset_version_id)::text) as event_ruleset_version_id,
                count(distinct e.ruleset_version_id)::int as event_ruleset_versions
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToDictionary(row => row.SessionName);

        Assert.Equal("f5b4c67b-0825-4970-9f07-3b68e8fcb524", rulesetChecks[SeedPemulaSessionName].ActivatedRulesetVersionId);
        Assert.Equal("f5b4c67b-0825-4970-9f07-3b68e8fcb524", rulesetChecks[SeedPemulaSessionName].EventRulesetVersionId);
        Assert.Equal(1, rulesetChecks[SeedPemulaSessionName].EventRulesetVersions);
        Assert.Equal("7c3bfd8a-27d7-4468-b8d7-cf90131bc61d", rulesetChecks[SeedMahirSessionName].ActivatedRulesetVersionId);
        Assert.Equal("7c3bfd8a-27d7-4468-b8d7-cf90131bc61d", rulesetChecks[SeedMahirSessionName].EventRulesetVersionId);
        Assert.Equal(1, rulesetChecks[SeedMahirSessionName].EventRulesetVersions);

        var calendarChecks = (await connection.QueryAsync<CalendarCheckRow>(
            """
            select
                s.session_name,
                count(distinct e.day_index)::int as day_count,
                min(e.day_index)::int as min_day_index,
                max(e.day_index)::int as max_day_index,
                count(*) filter (
                    where e.weekday <> case (((e.day_index - 1) % 7 + 7) % 7)
                        when 0 then 'MON'
                        when 1 then 'TUE'
                        when 2 then 'WED'
                        when 3 then 'THU'
                        when 4 then 'FRI'
                        when 5 then 'SAT'
                        else 'SUN'
                    end
                )::int as weekday_mismatch_count
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToDictionary(row => row.SessionName);

        Assert.All(calendarChecks.Values, row =>
        {
            Assert.Equal(25, row.DayCount);
            Assert.Equal(1, row.MinDayIndex);
            Assert.Equal(25, row.MaxDayIndex);
            Assert.Equal(0, row.WeekdayMismatchCount);
        });

        var internalTurnActionCount = await connection.ExecuteScalarAsync<int>(
            """
            select count(*)
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
              and e.action_type = 'turn.action.used'
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            });
        Assert.Equal(0, internalTurnActionCount);

        var pemulaForbiddenCount = await connection.ExecuteScalarAsync<int>(
            """
            select count(*)
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @pemulaSessionName
              and (
                e.action_type in (
                    'PinjamanSyariah',
                    'BayarPinjaman',
                    'Asuransi',
                    'RisikoKehidupan',
                    'GunakanOpsiDarurat',
                    'Menabung',
                    'TarikTabungan',
                    'TujuanFinansial'
                )
              )
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName
            });
        Assert.Equal(0, pemulaForbiddenCount);

        var setupRows = (await connection.QueryAsync<PlayerSetupProjectionRow>(
            """
            select
                s.session_name,
                sp.session_participant_id,
                count(*) filter (where e.action_type = 'BahanMasakan' and e.day_index = 1)::int as setup_ingredient_count,
                count(*) filter (where e.action_type = 'BagikanEmasAwal' and e.day_index = 1)::int as setup_gold_count,
                count(*) filter (where e.action_type = 'BagikanMisiKoleksi' and e.day_index = 1)::int as setup_mission_count,
                count(*) filter (where e.action_type = 'BagikanTieBreaker' and e.day_index = 1)::int as setup_tie_breaker_count,
                count(*) filter (
                    where e.action_type = 'PinjamanSyariah'
                      and e.day_index = 1
                      and e.payload->>'setup' = 'INITIAL'
                )::int as setup_loan_count,
                count(*) filter (
                    where e.action_type = 'Asuransi'
                      and e.day_index = 1
                      and coalesce((e.payload->>'premium')::int, -1) = 0
                      and e.payload->>'setup' = 'INITIAL'
                )::int as setup_insurance_count
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            left join events e
              on e.session_id = sp.session_id
             and e.session_player_id = sp.session_participant_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name, sp.session_participant_id
            order by s.session_name asc, sp.session_participant_id asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToList();

        Assert.Equal(8, setupRows.Count);
        foreach (var row in setupRows)
        {
            Assert.Equal(2, row.SetupIngredientCount);
            Assert.Equal(0, row.SetupGoldCount);
            Assert.Equal(1, row.SetupMissionCount);
            Assert.Equal(1, row.SetupTieBreakerCount);
            Assert.Equal(0, row.SetupLoanCount);
            Assert.Equal(0, row.SetupInsuranceCount);
        }

        var mahirLoanCount = await connection.ExecuteScalarAsync<int>(
            """
            select count(*)::int
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @mahirSessionName
              and e.action_type = 'PinjamanSyariah'
            """,
            new { mahirSessionName = SeedMahirSessionName });
        Assert.Equal(4, mahirLoanCount);

        var scenarioAlignmentRows = (await connection.QueryAsync<ScenarioAlignmentRow>(
            """
            select
                s.mode,
                sp.player_name,
                e.day_index,
                e.action_slot,
                e.action_type,
                coalesce(
                    e.payload->>'card_id',
                    e.payload->>'order_card_id',
                    e.payload->>'trade_type',
                    e.payload->>'status',
                    e.payload->>'note',
                    e.payload->>'loan_id',
                    ''
                ) as payload_key
            from sessions s
            join events e on e.session_id = s.session_id
            left join session_participants sp on sp.session_participant_id = e.session_player_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
              and e.actor_type = 'PLAYER'
              and (
                (s.mode = 'PEMULA' and e.day_index in (1, 2, 6))
                or (s.mode = 'MAHIR' and e.day_index in (1, 2, 8, 9, 10, 13, 15, 16, 20, 23, 25))
              )
            order by s.mode, e.day_index, sp.player_order_no, e.sequence_number
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToList();

        AssertScenarioEvent(
            scenarioAlignmentRows,
            "PEMULA",
            "Marco",
            1,
            1,
            "BahanMasakan",
            "nasi_putih");
        AssertScenarioEvent(
            scenarioAlignmentRows,
            "PEMULA",
            "Marco",
            1,
            2,
            "BahanMasakan",
            "telur");
        AssertScenarioEvent(
            scenarioAlignmentRows,
            "PEMULA",
            "Marco",
            2,
            1,
            "JualMasakan",
            "nasi_goreng");
        AssertScenarioEvent(
            scenarioAlignmentRows,
            "PEMULA",
            "Marcello",
            6,
            1,
            "InvestasiEmas",
            "BUY");

        foreach (var player in new[] { "Marco", "Marcello", "Hugo", "Manalu" })
        {
            AssertScenarioEvent(
                scenarioAlignmentRows,
                "MAHIR",
                player,
                1,
                1,
                "BahanMasakan",
                player is "Marcello" ? "daging" : player is "Manalu" ? "sayur" : "nasi_putih");
        }

        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marcello", 13, 1, "InvestasiEmas", "BUY");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Hugo", 13, 1, "InvestasiEmas", "BUY");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marco", 20, 1, "InvestasiEmas", "BUY");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Hugo", 20, 1, "InvestasiEmas", "BUY");

        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marcello", 15, 1, "Kebutuhan", "gameboy");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Manalu", 15, 1, "Kebutuhan", "boneka");

        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marco", 2, 1, "BahanMasakan", "telur");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marco", 2, 2, "Asuransi", "");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marco", 8, 1, "JualMasakan", "tahu_campur");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marcello", 9, 1, "BahanMasakan", "tahu_tempe");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marcello", 9, 2, "JualMasakan", "resep-sayur-bumbu");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Hugo", 10, 2, "RisikoKehidupan", "");
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Manalu", 10, 2, "PinjamanSyariah", "");

        var mahirActions = (await connection.QueryAsync<string>(
            """
            select distinct e.action_type
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @mahirSessionName
            """,
            new
            {
                mahirSessionName = SeedMahirSessionName
            })).ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("PinjamanSyariah", mahirActions);
        Assert.Contains("Asuransi", mahirActions);
        Assert.Contains("Menabung", mahirActions);
        Assert.Contains("JualMasakan", mahirActions);
        Assert.Contains("RisikoKehidupan", mahirActions);
        Assert.Contains("Asuransi", mahirActions);
        Assert.Contains("AmbilKartuDariDeck", mahirActions);
        Assert.Contains("KartuDiambilDariPasar", mahirActions);
        Assert.Contains("KartuMasukDiscard", mahirActions);
        Assert.Contains("IsiUlangPasar", mahirActions);

        var relationalReadModelCounts = await connection.QuerySingleAsync<RelationalReadModelCountRow>(
            """
            select
              count(*) filter (where exists (
                select 1 from session_participant_inventory spi
                where spi.session_id = s.session_id
              ))::int as ingredient_count,
              count(*) filter (where exists (
                select 1 from session_participant_need_purchases spnp
                where spnp.session_id = s.session_id
              ))::int as need_purchase_count,
              count(*) filter (where exists (
                select 1 from session_participant_financial_goals spfg
                where spfg.session_id = s.session_id
              ))::int as financial_goal_count,
              count(*) filter (where exists (
                select 1 from session_donation_events sde
                where sde.session_id = s.session_id
              ))::int as donation_event_count,
              count(*) filter (where exists (
                select 1 from events e
                where e.session_id = s.session_id
                  and e.action_type = 'PoinPeringkatDonasi'
              ))::int as donation_ranking_count,
              count(*) filter (where exists (
                select 1 from session_participant_action_counters spac
                where spac.session_participant_id in (
                  select sp.session_participant_id
                  from session_participants sp
                  where sp.session_id = s.session_id
                )
              ))::int as action_counter_count
            from sessions s
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            });

        Assert.True(relationalReadModelCounts.IngredientCount > 0);
        Assert.True(relationalReadModelCounts.NeedPurchaseCount > 0);
        Assert.True(relationalReadModelCounts.FinancialGoalCount > 0);
        Assert.True(relationalReadModelCounts.DonationEventCount > 0);
        Assert.True(relationalReadModelCounts.DonationRankingCount > 0);
        Assert.True(relationalReadModelCounts.ActionCounterCount > 0);

        var riskPayloadWithHardcodedValueCount = await connection.ExecuteScalarAsync<int>(
            """
            select count(*)::int
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @mahirSessionName
              and e.action_type = 'RisikoKehidupan'
              and (e.payload ? 'direction' or e.payload ? 'amount')
            """,
            new { mahirSessionName = SeedMahirSessionName });
        Assert.Equal(0, riskPayloadWithHardcodedValueCount);

        var unmatchedInsuranceUseCount = await connection.ExecuteScalarAsync<int>(
            """
            with insurance_usage as (
              select
                e.session_id,
                e.user_id,
                (e.payload::jsonb->>'risk_event_id')::uuid as risk_event_id
              from sessions s
              join events e on e.session_id = s.session_id
              where s.session_name = @mahirSessionName
                and e.action_type = 'Asuransi'
                and e.payload ? 'risk_event_id'
            )
            select count(*)
            from insurance_usage iu
            left join events risk_evt
              on risk_evt.session_id = iu.session_id
             and risk_evt.event_id = iu.risk_event_id
             and risk_evt.user_id = iu.user_id
             and risk_evt.action_type = 'RisikoKehidupan'
            left join ruleset_life_risks risk_catalog
              on risk_catalog.ruleset_version_id = risk_evt.ruleset_version_id
             and lower(risk_catalog.risk_code) = lower(risk_evt.payload::jsonb->>'risk_id')
             and risk_catalog.direction = 'OUT'
            where risk_evt.event_id is null
               or risk_catalog.ruleset_life_risk_id is null
            """,
            new
            {
                mahirSessionName = SeedMahirSessionName
            });
        Assert.Equal(0, unmatchedInsuranceUseCount);

        var insuredRiskNetCostCount = await connection.ExecuteScalarAsync<int>(
            """
            with insured_risks as (
              select
                risk_evt.event_pk as risk_event_pk,
                insurance_evt.event_pk as insurance_event_pk
              from sessions s
              join events insurance_evt
                on insurance_evt.session_id = s.session_id
               and insurance_evt.action_type = 'Asuransi'
               and insurance_evt.payload ? 'risk_event_id'
              join events risk_evt
                on risk_evt.session_id = insurance_evt.session_id
               and risk_evt.event_id = (insurance_evt.payload->>'risk_event_id')::uuid
              join ruleset_life_risks risk_catalog
                on risk_catalog.ruleset_version_id = risk_evt.ruleset_version_id
               and lower(risk_catalog.risk_code) = lower(risk_evt.payload->>'risk_id')
               and risk_catalog.direction = 'OUT'
              where s.session_name = @mahirSessionName
            ),
            net_cost as (
              select
                ir.risk_event_pk,
                coalesce(sum(case when ecp.event_pk = ir.risk_event_pk and ecp.direction = 'OUT' then ecp.amount else 0 end), 0)
                - coalesce(sum(case when ecp.event_pk = ir.insurance_event_pk and ecp.direction = 'IN' then ecp.amount else 0 end), 0) as net_amount
              from insured_risks ir
              left join event_cashflow_projections ecp
                on ecp.event_pk in (ir.risk_event_pk, ir.insurance_event_pk)
              group by ir.risk_event_pk
            )
            select count(*)::int
            from net_cost
            where net_amount <> 0
            """,
            new { mahirSessionName = SeedMahirSessionName });
        Assert.Equal(0, insuredRiskNetCostCount);

        var marketSlotRows = (await connection.QueryAsync<MarketSlotRow>(
            """
            select
                s.session_name,
                scp.slot_group,
                count(*)::int as slot_count
            from sessions s
            join session_card_positions scp on scp.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
              and scp.zone = 'MARKET'
              and scp.status = 'ACTIVE'
            group by s.session_name, scp.slot_group
            order by s.session_name, scp.slot_group
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToList();

        foreach (var sessionName in new[] { SeedPemulaSessionName, SeedMahirSessionName })
        {
            Assert.Equal(5, Assert.Single(marketSlotRows, row => row.SessionName == sessionName && row.SlotGroup == "INGREDIENT_MARKET").SlotCount);
            Assert.Equal(5, Assert.Single(marketSlotRows, row => row.SessionName == sessionName && row.SlotGroup == "ORDER_MARKET").SlotCount);
            Assert.Equal(5, Assert.Single(marketSlotRows, row => row.SessionName == sessionName && row.SlotGroup == "NEED_MARKET").SlotCount);
        }

        var missionAndDonationChecks = (await connection.QueryAsync<ScenarioSystemEventCountRow>(
            """
            select
                s.session_name,
                count(*) filter (where e.action_type = 'BagikanMisiKoleksi')::int as mission_assigned_count,
                count(*) filter (where e.action_type = 'PoinPeringkatDonasi')::int as donation_rank_awarded_count,
                count(*) filter (where e.action_type = 'UmumkanJuaraDonasi')::int as donation_winners_announced_count,
                count(*) filter (where e.action_type = 'BagikanTieBreaker')::int as tie_breaker_assigned_count
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToDictionary(row => row.SessionName);

        Assert.Equal(4, missionAndDonationChecks[SeedPemulaSessionName].MissionAssignedCount);
        Assert.Equal(4, missionAndDonationChecks[SeedMahirSessionName].MissionAssignedCount);
        Assert.Equal(9, missionAndDonationChecks[SeedPemulaSessionName].DonationRankAwardedCount);
        Assert.Equal(9, missionAndDonationChecks[SeedMahirSessionName].DonationRankAwardedCount);
        Assert.Equal(3, missionAndDonationChecks[SeedPemulaSessionName].DonationWinnersAnnouncedCount);
        Assert.Equal(3, missionAndDonationChecks[SeedMahirSessionName].DonationWinnersAnnouncedCount);
        Assert.Equal(4, missionAndDonationChecks[SeedPemulaSessionName].TieBreakerAssignedCount);
        Assert.Equal(4, missionAndDonationChecks[SeedMahirSessionName].TieBreakerAssignedCount);

        var winnerAnnouncementSummaries = (await connection.QueryAsync<string>(
            """
            select e.payload->>'summary'
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @pemulaSessionName
              and e.action_type = 'UmumkanJuaraDonasi'
            order by e.day_index asc
            """,
            new { pemulaSessionName = SeedPemulaSessionName })).ToList();

        Assert.Equal(
            new[]
            {
                "Manalu Juara 1, Marcello Juara 2, Marco Juara 3",
                "Marco Juara 1, Hugo Juara 2, Manalu Juara 3",
                "Marcello Juara 1, Manalu Juara 2, Marco Juara 3"
            },
            winnerAnnouncementSummaries);

        var projectionCoverage = (await connection.QueryAsync<PlayerProjectionCoverageRow>(
            """
            select
                s.session_name,
                sp.user_id,
                count(distinct e.event_pk) filter (where e.actor_type = 'PLAYER')::int as player_event_count,
                count(distinct ecp.projection_id)::int as projection_count
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            left join events e
              on e.session_id = sp.session_id
             and e.user_id = sp.user_id
            left join event_cashflow_projections ecp
              on ecp.session_id = sp.session_id
             and ecp.user_id = sp.user_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name, sp.user_id
            order by s.session_name asc, sp.user_id asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToList();

        Assert.Equal(8, projectionCoverage.Count);
        Assert.All(projectionCoverage, row =>
        {
            Assert.True(row.PlayerEventCount > 0, $"{row.SessionName}/{row.UserId} tidak punya event pemain.");
            Assert.True(row.ProjectionCount > 0, $"{row.SessionName}/{row.UserId} tidak punya transaksi cashflow.");
        });

        var gameplaySnapshotCoverage = (await connection.QueryAsync<PlayerGameplaySnapshotCoverageRow>(
            """
            select
                s.session_name,
                sp.user_id,
                count(*) filter (where ms.metric_name = 'gameplay.raw.variables')::int as raw_snapshot_count,
                count(*) filter (where ms.metric_name = 'gameplay.derived.metrics')::int as derived_snapshot_count,
                count(*) filter (
                    where ms.metric_name in ('gameplay.raw.variables', 'gameplay.derived.metrics')
                      and ms.metric_payload_json is null
                )::int as empty_payload_count
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            left join metric_snapshots ms
              on ms.session_id = sp.session_id
             and ms.user_id = sp.user_id
             and ms.session_player_id = sp.session_participant_id
             and ms.metric_name in ('gameplay.raw.variables', 'gameplay.derived.metrics')
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name, sp.user_id
            order by s.session_name asc, sp.user_id asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToList();

        Assert.Equal(8, gameplaySnapshotCoverage.Count);
        Assert.All(gameplaySnapshotCoverage, row =>
        {
            Assert.True(row.RawSnapshotCount > 0, $"{row.SessionName}/{row.UserId} tidak punya snapshot gameplay raw.");
            Assert.True(row.DerivedSnapshotCount > 0, $"{row.SessionName}/{row.UserId} tidak punya snapshot gameplay derived.");
            Assert.Equal(0, row.EmptyPayloadCount);
        });

        await AssertScenarioReplayIsValidAsync(connection);
    }

    private static async Task AssertScenarioReplayIsValidAsync(NpgsqlConnection connection)
    {
        var events = (await connection.QueryAsync<ReplayEventRow>(
            """
            select
                s.session_name,
                s.mode,
                e.session_id,
                e.event_id,
                e.user_id,
                sp.player_name,
                e.actor_type,
                e.day_index,
                e.weekday,
                e.action_slot,
                e.sequence_number,
                e.action_type,
                e.payload
            from sessions s
            join events e on e.session_id = s.session_id
            left join session_participants sp on sp.session_participant_id = e.session_player_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            order by s.session_name asc, e.sequence_number asc
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            })).ToList();

        Assert.NotEmpty(events);

        var lifeRiskCatalog = (await connection.QueryAsync<ReplayLifeRiskRow>(
            """
            select distinct
                risk.risk_code,
                risk.direction,
                risk.amount
            from sessions s
            join ruleset_life_risks risk
              on risk.ruleset_version_id = s.ruleset_version_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            """,
            new
            {
                pemulaSessionName = SeedPemulaSessionName,
                mahirSessionName = SeedMahirSessionName
            }))
            .ToDictionary(
                row => row.RiskCode,
                row => (row.Direction, row.Amount),
                StringComparer.OrdinalIgnoreCase);

        var sessionStates = events
            .GroupBy(e => e.SessionId)
            .ToDictionary(
                group => group.Key,
                group => new ReplaySessionState(
                    group.First().SessionName,
                    group.First().Mode,
                    group.First().Mode.Equals("MAHIR", StringComparison.OrdinalIgnoreCase) ? 10 : 20));

        var failures = new List<string>();
        var eventById = events.ToDictionary(evt => evt.EventId);
        var insuredRiskEventIds = new HashSet<Guid>();
        foreach (var insuranceEvent in events.Where(evt => evt.ActionType.Equals("Asuransi", StringComparison.OrdinalIgnoreCase)))
        {
            using var document = JsonDocument.Parse(insuranceEvent.Payload);
            if (document.RootElement.TryGetProperty("risk_event_id", out var riskEventIdElement) &&
                riskEventIdElement.ValueKind == JsonValueKind.String &&
                Guid.TryParse(riskEventIdElement.GetString(), out var riskEventId))
            {
                insuredRiskEventIds.Add(riskEventId);
            }
        }

        foreach (var evt in events)
        {
            using var payloadDocument = JsonDocument.Parse(evt.Payload);
            var payload = payloadDocument.RootElement;
            var session = sessionStates[evt.SessionId];
            try
            {
                ReplayEvent(session, evt, payload, eventById, insuredRiskEventIds, lifeRiskCatalog);
            }
            catch (InvalidOperationException ex)
            {
                failures.Add($"{evt.SessionName} seq={evt.SequenceNumber} day={evt.DayIndex} player={evt.PlayerName ?? "-"} action={evt.ActionType}: {ex.Message}");
            }
        }

        foreach (var session in sessionStates.Values)
        {
            ValidateDonationRanks(session, failures);
            ValidateFinalPlayerState(session, failures);
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    private static void AssertScenarioEvent(
        IReadOnlyCollection<ScenarioAlignmentRow> rows,
        string mode,
        string playerName,
        int dayIndex,
        int actionSlot,
        string actionType,
        string payloadKey)
    {
        Assert.Contains(
            rows,
            row => row.Mode == mode
                   && row.PlayerName == playerName
                   && row.DayIndex == dayIndex
                   && row.ActionSlot == actionSlot
                   && row.ActionType == actionType
                   && (string.IsNullOrEmpty(payloadKey) ||
                       string.Equals(row.PayloadKey, payloadKey, StringComparison.OrdinalIgnoreCase)));
    }

    private static void ReplayEvent(
        ReplaySessionState session,
        ReplayEventRow evt,
        JsonElement payload,
        IReadOnlyDictionary<Guid, ReplayEventRow> eventById,
        IReadOnlySet<Guid> insuredRiskEventIds,
        IReadOnlyDictionary<string, (string Direction, int Amount)> lifeRiskCatalog)
    {
        ValidateCalendarAction(evt);

        if (evt.ActorType.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
        {
            ReplaySystemEvent(session, evt, payload);
            return;
        }

        var player = session.GetPlayer(evt.UserId, evt.PlayerName);
        session.RecordActionToken(evt, CountsAsActionToken(evt.ActionType, payload));

        if (session.Mode.Equals("PEMULA", StringComparison.OrdinalIgnoreCase) && IsMahirOnlyAction(evt.ActionType))
        {
            throw new InvalidOperationException("Mode PEMULA tidak boleh berisi aksi khusus MAHIR.");
        }

        switch (evt.ActionType)
        {
            case "BahanMasakan":
                ApplyCashOut(player, ReadInt(payload, "amount"), evt, "biaya bahan");
                AddInventory(player.Ingredients, ReadString(payload, "card_id"), 1);
                ValidateIngredientLimits(player, evt);
                break;

            case "BuangBahanMasakan":
                RemoveInventory(player.Ingredients, ReadString(payload, "card_id"), ReadQuantity(payload), evt);
                break;

            case "JualMasakan":
                foreach (var cardId in ReadStringArray(payload, "required_ingredient_card_ids"))
                {
                    RemoveInventoryIfAvailable(player.Ingredients, cardId, 1);
                }

                ApplyCashIn(player, ReadInt(payload, "income"));
                break;

            case "KerjaLepas":
                ApplyCashIn(player, ReadInt(payload, "amount"));
                break;

            case "Kebutuhan":
                var needCardId = ReadString(payload, "card_id");
                var needTier = ResolveNeedTier(payload, needCardId);
                if (needTier.Equals("primer", StringComparison.OrdinalIgnoreCase))
                {
                    ValidatePrimaryNeedLimit(session, evt);
                    ApplyCashOut(player, ReadInt(payload, "amount"), evt, "kebutuhan primer");
                    player.PrimaryNeeds.Add(needCardId);
                    break;
                }

                RequirePrimaryBeforeOtherNeeds(player);
                if (needTier.Equals("sekunder", StringComparison.OrdinalIgnoreCase))
                {
                    ApplyCashOut(player, ReadInt(payload, "amount"), evt, "kebutuhan sekunder");
                    player.SecondaryNeeds.Add(needCardId);
                    break;
                }

                ApplyCashOut(player, ReadInt(payload, "amount"), evt, "kebutuhan tersier");
                player.TertiaryNeeds.Add(needCardId);
                break;

            case "JumatBerkah":
                var donationAmount = ReadInt(payload, "amount");
                ApplyCashOut(player, donationAmount, evt, "donasi Jumat");
                session.RecordDonation(evt.DayIndex, evt.UserId!.Value, donationAmount);
                break;

            case "InvestasiEmas":
            case "JualEmas":
                ApplyGoldTrade(player, evt, payload);
                break;

            case "LewatiTransaksiEmas":
                break;

            case "RisikoKehidupan":
                if (!session.Mode.Equals("MAHIR", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Risk life hanya valid pada mode MAHIR.");
                }

                ApplyRiskLife(player, evt, payload, insuredRiskEventIds, lifeRiskCatalog);
                break;

            case "Asuransi":
                if (payload.TryGetProperty("risk_event_id", out _) || payload.TryGetProperty("risk_event_ref", out _))
                {
                    ApplyInsuranceUsage(player, evt, payload, eventById);
                    break;
                }

                RequireFeature(session.Mode, enabled: true, evt, "asuransi");
                var policyId = ReadString(payload, "policy_id");
                if (!player.InsurancePolicies.Add(policyId))
                {
                    throw new InvalidOperationException($"Policy asuransi duplikat: {policyId}.");
                }

                ApplyCashOut(player, ReadInt(payload, "premium"), evt, "premi asuransi");
                break;

            case "PinjamanSyariah":
                RequireFeature(session.Mode, enabled: true, evt, "pinjaman syariah");
                var loanId = ReadString(payload, "loan_id");
                if (player.Loans.ContainsKey(loanId))
                {
                    throw new InvalidOperationException($"Loan ID duplikat: {loanId}.");
                }

                var principal = ReadInt(payload, "principal");
                if (principal != 10 || ReadInt(payload, "penalty_points") != 15)
                {
                    throw new InvalidOperationException("Pinjaman syariah harus principal 10 dan penalty 15.");
                }

                player.Loans[loanId] = principal;
                ApplyCashIn(player, principal);
                break;

            case "BayarPinjaman":
                var repayLoanId = ReadString(payload, "loan_id");
                var repayAmount = ReadInt(payload, "amount");
                if (!player.Loans.TryGetValue(repayLoanId, out var outstanding))
                {
                    throw new InvalidOperationException($"Loan ID tidak ditemukan: {repayLoanId}.");
                }

                if (repayAmount > outstanding)
                {
                    throw new InvalidOperationException($"Pembayaran {repayAmount} melebihi sisa pinjaman {outstanding}.");
                }

                ApplyCashOut(player, repayAmount, evt, "bayar pinjaman");
                player.Loans[repayLoanId] = outstanding - repayAmount;
                break;

            case "Menabung":
                RequireFeature(session.Mode, enabled: true, evt, "tabungan tujuan");
                var depositGoalId = ReadString(payload, "goal_id");
                var depositAmount = ReadInt(payload, "amount");
                if (depositAmount > 15)
                {
                    throw new InvalidOperationException("Maksimal tabungan per aksi adalah 15 koin.");
                }

                ApplyCashOut(player, depositAmount, evt, "setoran tabungan");
                AddInventory(player.Savings, depositGoalId, depositAmount);
                break;

            case "TarikTabungan":
                RequireFeature(session.Mode, enabled: true, evt, "tabungan tujuan");
                var withdrawGoalId = ReadString(payload, "goal_id");
                var withdrawAmount = ReadInt(payload, "amount");
                RemoveInventory(player.Savings, withdrawGoalId, withdrawAmount, evt);
                ApplyCashIn(player, withdrawAmount);
                break;

            case "TujuanFinansial":
                RequireFeature(session.Mode, enabled: true, evt, "tabungan tujuan");
                var achievedGoalId = ReadString(payload, "goal_id");
                var cost = ReadInt(payload, "cost");
                RemoveInventory(player.Savings, achievedGoalId, cost, evt);
                player.AchievedSavingGoals.Add(achievedGoalId);
                break;

            default:
                throw new InvalidOperationException($"Action type belum ditangani replay validator: {evt.ActionType}.");
        }
    }

    private static async Task RunWithConnectionStringAsync(string connectionString, Func<Task> action)
    {
        var previousConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        var previousJwtSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        var previousJwtSectionSigningKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");

        Environment.SetEnvironmentVariable("ConnectionStrings__Default", connectionString);
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);
        Environment.SetEnvironmentVariable("Jwt__SigningKey", JwtSigningKey);

        try
        {
            await action();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", previousConnectionString);
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", previousJwtSigningKey);
            Environment.SetEnvironmentVariable("Jwt__SigningKey", previousJwtSectionSigningKey);
        }
    }

    private static string RepoRoot => ResolveRepositoryRoot();

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    }

    private static void ReplaySystemEvent(ReplaySessionState session, ReplayEventRow evt, JsonElement payload)
    {
        switch (evt.ActionType)
        {
            case "MulaiSesi":
                Assert.Equal(1, evt.DayIndex);
                break;

            case "BagikanMisiKoleksi":
                _ = ReadString(payload, "mission_id");
                _ = ReadString(payload, "target_tertiary_card_id");
                _ = ReadInt(payload, "penalty_points");
                break;

            case "BagikanEmasAwal":
                if (!evt.UserId.HasValue)
                {
                    throw new InvalidOperationException("Emas awal wajib memiliki user_id pemain.");
                }

                session.GetPlayer(evt.UserId, evt.PlayerName).GoldQty += ReadInt(payload, "qty");
                break;

            case "Asuransi":
                if (!evt.UserId.HasValue)
                {
                    throw new InvalidOperationException("Asuransi setup wajib memiliki user_id pemain.");
                }

                var setupPolicyId = ReadString(payload, "policy_id");
                var setupPremium = ReadInt(payload, "premium");
                if (setupPremium != 0)
                {
                    throw new InvalidOperationException("Asuransi setup wajib gratis.");
                }

                if (!session.GetPlayer(evt.UserId, evt.PlayerName).InsurancePolicies.Add(setupPolicyId))
                {
                    throw new InvalidOperationException($"Policy asuransi duplikat: {setupPolicyId}.");
                }

                break;

            case "AmbilKartuDariDeck":
            case "KartuDiambilDariPasar":
            case "KartuMasukDiscard":
            case "IsiUlangPasar":
                _ = ReadString(payload, "asset_type");
                _ = ReadString(payload, "asset_code");
                break;

            case "PoinPeringkatDonasi":
                var rank = ReadInt(payload, "rank");
                var points = ReadInt(payload, "points");
                if (!evt.UserId.HasValue)
                {
                    throw new InvalidOperationException("Donation rank wajib memiliki user_id pemain.");
                }

                session.RecordDonationAward(evt.DayIndex, evt.UserId.Value, rank, points);
                break;

            case "UmumkanJuaraDonasi":
                _ = ReadString(payload, "summary");
                if (!payload.TryGetProperty("winners", out var winners) ||
                    winners.ValueKind != JsonValueKind.Array ||
                    winners.GetArrayLength() != 3)
                {
                    throw new InvalidOperationException("Pengumuman juara donasi wajib berisi 3 winner.");
                }

                var expectedRank = 1;
                foreach (var winner in winners.EnumerateArray())
                {
                    var winnerRank = ReadInt(winner, "rank");
                    _ = ReadString(winner, "player_name");
                    _ = ReadInt(winner, "points");
                    if (winnerRank != expectedRank)
                    {
                        throw new InvalidOperationException("Urutan winner juara donasi tidak sesuai rank.");
                    }

                    expectedRank++;
                }

                break;

            case "BagikanTieBreaker":
                var tieNumber = ReadInt(payload, "number");
                if (!evt.UserId.HasValue)
                {
                    throw new InvalidOperationException("Tie breaker wajib memiliki user_id pemain.");
                }

                if (tieNumber is < 1 or > 4)
                {
                    throw new InvalidOperationException($"Nomor tie breaker tidak valid: {tieNumber}.");
                }

                session.RecordTieBreaker(evt.UserId.Value, tieNumber);
                break;

            case "HariMingguLibur":
                if (!evt.Weekday.Equals("SUN", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Event libur Minggu harus jatuh pada weekday SUN.");
                }

                break;

            case "AkhirGiliran":
                var fromDay = ReadInt(payload, "from_day");
                var toDay = ReadInt(payload, "to_day");
                var completedPlayers = ReadInt(payload, "completed_players");
                var used = ReadInt(payload, "used");
                var remaining = ReadInt(payload, "remaining");

                if (evt.DayIndex is < 1 or >= 25)
                {
                    throw new InvalidOperationException("AkhirGiliran hanya valid untuk transisi hari 1 sampai 24.");
                }

                if (fromDay != evt.DayIndex || toDay != evt.DayIndex + 1)
                {
                    throw new InvalidOperationException("Payload AkhirGiliran tidak sesuai transisi day_index event.");
                }

                if (completedPlayers != 4)
                {
                    throw new InvalidOperationException("AkhirGiliran wajib menutup giliran 4 pemain.");
                }

                var replayedUsed = session.ActionTokensByDay
                    .Where(pair => pair.Key.DayIndex == evt.DayIndex)
                    .Sum(pair => pair.Value);
                if (used != replayedUsed)
                {
                    throw new InvalidOperationException($"AkhirGiliran used={used}, expected {replayedUsed} dari replay action token.");
                }

                if (remaining != 0)
                {
                    throw new InvalidOperationException("AkhirGiliran seed wajib menyisakan 0 action setelah transisi hari.");
                }

                break;

            case "AkhiriSesi":
                Assert.Equal(25, evt.DayIndex);
                break;

            default:
                throw new InvalidOperationException($"System action belum ditangani replay validator: {evt.ActionType}.");
        }
    }

    private static void ValidateCalendarAction(ReplayEventRow evt)
    {
        if (evt.ActionType is "JumatBerkah" or "PoinPeringkatDonasi" or "UmumkanJuaraDonasi")
        {
            if (!evt.Weekday.Equals("FRI", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"{evt.ActionType} harus jatuh pada FRI.");
            }
        }

        if (evt.ActionType is "InvestasiEmas" or "LewatiTransaksiEmas")
        {
            if (!evt.Weekday.Equals("SAT", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"{evt.ActionType} harus jatuh pada SAT.");
            }
        }

        if (evt.ActorType.Equals("PLAYER", StringComparison.OrdinalIgnoreCase) &&
            evt.Weekday.Equals("SUN", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Hari Minggu tidak boleh memiliki event pemain.");
        }
    }

    private static void ValidateIngredientLimits(ReplayPlayerState player, ReplayEventRow evt)
    {
        var total = player.Ingredients.Values.Sum();
        if (total > 6)
        {
            throw new InvalidOperationException($"Total bahan {total} melebihi batas ruleset 6.");
        }

        var maxSame = player.Ingredients.Values.DefaultIfEmpty(0).Max();
        if (maxSame > 3)
        {
            throw new InvalidOperationException($"Jumlah bahan sejenis {maxSame} melebihi batas ruleset 3.");
        }
    }

    private static void ValidatePrimaryNeedLimit(ReplaySessionState session, ReplayEventRow evt)
    {
        var key = (evt.DayIndex, evt.UserId!.Value);
        var current = session.PrimaryNeedPurchasesByDay.TryGetValue(key, out var count) ? count : 0;
        if (current >= 1)
        {
            throw new InvalidOperationException("Pembelian kebutuhan primer melebihi batas harian.");
        }

        session.PrimaryNeedPurchasesByDay[key] = current + 1;
    }

    private static void RequirePrimaryBeforeOtherNeeds(ReplayPlayerState player)
    {
        if (player.PrimaryNeeds.Count == 0)
        {
            throw new InvalidOperationException("Kebutuhan primer harus dibeli terlebih dahulu sebelum kebutuhan lain.");
        }
    }

    private static string ResolveNeedTier(JsonElement payload, string cardId)
    {
        foreach (var propertyName in new[] { "need_tier", "tier", "need_type" })
        {
            if (payload.TryGetProperty(propertyName, out var property) &&
                property.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(property.GetString()))
            {
                return property.GetString()!;
            }
        }

        return cardId.ToLowerInvariant() switch
        {
            "buku" => "primer",
            "sepatu" => "sekunder",
            _ => "tersier"
        };
    }

    private static void ApplyGoldTrade(ReplayPlayerState player, ReplayEventRow evt, JsonElement payload)
    {
        var tradeType = ReadString(payload, "trade_type");
        var qty = ReadInt(payload, "qty");
        var unitPrice = ReadInt(payload, "unit_price");
        var amount = ReadInt(payload, "amount");

        if (qty <= 0 || unitPrice <= 0 || amount != qty * unitPrice)
        {
            throw new InvalidOperationException("Payload gold trade tidak sesuai qty * unit_price.");
        }

        if (tradeType.Equals("BUY", StringComparison.OrdinalIgnoreCase))
        {
            ApplyCashOut(player, amount, evt, "beli emas");
            player.GoldQty += qty;
            player.GoldNetAmount += amount;
            return;
        }

        if (tradeType.Equals("SELL", StringComparison.OrdinalIgnoreCase))
        {
            if (player.GoldQty < qty)
            {
                throw new InvalidOperationException("Kepemilikan emas tidak mencukupi untuk SELL.");
            }

            player.GoldQty -= qty;
            player.GoldNetAmount -= amount;
            ApplyCashIn(player, amount);
            return;
        }

        throw new InvalidOperationException($"Trade type emas tidak valid: {tradeType}.");
    }

    private static void ApplyRiskLife(
        ReplayPlayerState player,
        ReplayEventRow evt,
        JsonElement payload,
        IReadOnlySet<Guid> insuredRiskEventIds,
        IReadOnlyDictionary<string, (string Direction, int Amount)> lifeRiskCatalog)
    {
        var riskId = ReadString(payload, "risk_id");
        if (!lifeRiskCatalog.TryGetValue(riskId, out var risk))
        {
            throw new InvalidOperationException($"Risk tidak ditemukan di katalog test: {riskId}.");
        }

        var direction = risk.Direction;
        var amount = risk.Amount;
        if (amount <= 0)
        {
            throw new InvalidOperationException("Amount risiko harus > 0.");
        }

        if (direction.Equals("IN", StringComparison.OrdinalIgnoreCase))
        {
            ApplyCashIn(player, amount);
            return;
        }

        if (direction.Equals("OUT", StringComparison.OrdinalIgnoreCase))
        {
            player.RiskOutByEventId[evt.EventId] = amount;
            if (!insuredRiskEventIds.Contains(evt.EventId))
            {
                ApplyCashOut(player, amount, evt, "risk life");
            }

            return;
        }

        throw new InvalidOperationException($"Direction risiko tidak valid: {direction}.");
    }

    private static void ApplyInsuranceUsage(
        ReplayPlayerState player,
        ReplayEventRow evt,
        JsonElement payload,
        IReadOnlyDictionary<Guid, ReplayEventRow> eventById)
    {
        if (player.InsurancePolicies.Count == 0)
        {
            throw new InvalidOperationException("Pemain belum membeli asuransi.");
        }

        var riskEventId = ReadGuid(payload, "risk_event_id");
        if (!eventById.TryGetValue(riskEventId, out var riskEvent))
        {
            throw new InvalidOperationException("Risk event tidak ditemukan untuk insurance usage.");
        }

        if (riskEvent.UserId != evt.UserId)
        {
            throw new InvalidOperationException("Risk event insurance usage bukan milik pemain.");
        }

        if (!player.RiskOutByEventId.ContainsKey(riskEventId))
        {
            throw new InvalidOperationException("Asuransi hanya boleh dipakai untuk risiko OUT yang sudah tercatat.");
        }

        if (!player.InsuranceOffsets.Add(riskEventId))
        {
            throw new InvalidOperationException("Risk event sudah ditangkal asuransi.");
        }

    }

    private static void ValidateDonationRanks(ReplaySessionState session, List<string> failures)
    {
        foreach (var day in session.DonationsByDay.Keys.OrderBy(day => day))
        {
            var expectedRanks = session.DonationsByDay[day]
                .OrderByDescending(pair => pair.Value)
                .ThenByDescending(pair => session.TieBreakers.TryGetValue(pair.Key, out var tieNumber) ? tieNumber : 0)
                .ThenBy(pair => pair.Key)
                .Take(3)
                .Select((pair, index) => new
                {
                    UserId = pair.Key,
                    Rank = index + 1,
                    Points = index switch
                    {
                        0 => 7,
                        1 => 5,
                        _ => 2
                    }
                })
                .ToList();

            var actualRanks = session.DonationAwardsByDay.TryGetValue(day, out var awards)
                ? awards.OrderBy(award => award.Rank).ToList()
                : new List<DonationAward>();

            if (actualRanks.Count != expectedRanks.Count)
            {
                failures.Add($"{session.SessionName} day={day}: jumlah donation rank award {actualRanks.Count}, expected {expectedRanks.Count}.");
                continue;
            }

            foreach (var expected in expectedRanks)
            {
                var actual = actualRanks.FirstOrDefault(award => award.Rank == expected.Rank);
                if (actual is null || actual.UserId != expected.UserId || actual.Points != expected.Points)
                {
                    failures.Add($"{session.SessionName} day={day}: rank donasi {expected.Rank} tidak cocok.");
                }
            }
        }
    }

    private static void ValidateFinalPlayerState(ReplaySessionState session, List<string> failures)
    {
        foreach (var player in session.Players.Values)
        {
            if (player.Cash < 0)
            {
                failures.Add($"{session.SessionName}/{player.PlayerName}: saldo akhir negatif {player.Cash}.");
            }

            if (player.Ingredients.Any(pair => pair.Value < 0) ||
                player.Savings.Any(pair => pair.Value < 0) ||
                player.GoldQty < 0)
            {
                failures.Add($"{session.SessionName}/{player.PlayerName}: state akhir memiliki nilai negatif.");
            }
        }
    }

    private static bool CountsAsActionToken(string actionType, JsonElement payload)
    {
        if (actionType.Equals("RisikoKehidupan", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (actionType.Equals("Asuransi", StringComparison.OrdinalIgnoreCase) &&
            (payload.TryGetProperty("risk_event_id", out _) || payload.TryGetProperty("risk_event_ref", out _)))
        {
            return false;
        }

        return true;
    }

    private static bool IsMahirOnlyAction(string actionType)
    {
        return actionType.StartsWith("loan.", StringComparison.OrdinalIgnoreCase) ||
               actionType.StartsWith("insurance.", StringComparison.OrdinalIgnoreCase) ||
               actionType.StartsWith("saving.", StringComparison.OrdinalIgnoreCase) ||
               actionType.Equals("RisikoKehidupan", StringComparison.OrdinalIgnoreCase);
    }

    private static void RequireFeature(string mode, bool enabled, ReplayEventRow evt, string feature)
    {
        if (!mode.Equals("MAHIR", StringComparison.OrdinalIgnoreCase) || !enabled)
        {
            throw new InvalidOperationException($"Fitur {feature} hanya valid pada mode MAHIR.");
        }
    }

    private static void ApplyCashOut(ReplayPlayerState player, int amount, ReplayEventRow evt, string reason)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException($"Amount {reason} harus > 0.");
        }

        player.Cash = Math.Max(player.Cash - amount, 0);
    }

    private static void ApplyCashIn(ReplayPlayerState player, int amount)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Cash IN harus > 0.");
        }

        player.Cash += amount;
    }

    private static void AddInventory(IDictionary<string, int> inventory, string key, int amount)
    {
        inventory[key] = inventory.TryGetValue(key, out var current) ? current + amount : amount;
    }

    private static void RemoveInventory(IDictionary<string, int> inventory, string key, int amount, ReplayEventRow evt)
    {
        if (!inventory.TryGetValue(key, out var current) || current < amount)
        {
            throw new InvalidOperationException($"Stok {key} tidak cukup untuk {evt.ActionType}: stok={current}, amount={amount}.");
        }

        inventory[key] = current - amount;
    }

    private static void RemoveInventoryIfAvailable(IDictionary<string, int> inventory, string key, int amount)
    {
        if (!inventory.TryGetValue(key, out var current) || current <= 0)
        {
            return;
        }

        inventory[key] = Math.Max(current - amount, 0);
    }

    private static string ReadString(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new InvalidOperationException($"Payload wajib memiliki string '{propertyName}'.");
        }

        return property.GetString()!;
    }

    private static int ReadInt(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out var property) || !property.TryGetInt32(out var value))
        {
            throw new InvalidOperationException($"Payload wajib memiliki integer '{propertyName}'.");
        }

        return value;
    }

    private static int ReadQuantity(JsonElement payload)
    {
        return payload.TryGetProperty("quantity", out var quantityProperty) && quantityProperty.TryGetInt32(out var quantity)
            ? quantity
            : 1;
    }

    private static Guid ReadGuid(JsonElement payload, string propertyName)
    {
        var value = ReadString(payload, propertyName);
        return Guid.TryParse(value, out var guid)
            ? guid
            : throw new InvalidOperationException($"Payload '{propertyName}' bukan UUID valid.");
    }

    private static List<string> ReadStringArray(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException($"Payload wajib memiliki array '{propertyName}'.");
        }

        return property.EnumerateArray()
            .Select(item => item.GetString())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToList();
    }

    private sealed class SeedSessionRow
    {
        public Guid SessionId { get; init; }
        public string SessionName { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string Mode { get; init; } = string.Empty;
    }

    private sealed class SessionPlayerCountRow
    {
        public string SessionName { get; init; } = string.Empty;
        public int PlayerCount { get; init; }
    }

    private sealed class SequenceCheckRow
    {
        public string SessionName { get; init; } = string.Empty;
        public long MinSequence { get; init; }
        public long MaxSequence { get; init; }
        public int EventCount { get; init; }
    }

    private sealed class RulesetCheckRow
    {
        public string SessionName { get; init; } = string.Empty;
        public string ActivatedRulesetVersionId { get; init; } = string.Empty;
        public string EventRulesetVersionId { get; init; } = string.Empty;
        public int EventRulesetVersions { get; init; }
    }

    private sealed class CalendarCheckRow
    {
        public string SessionName { get; init; } = string.Empty;
        public int DayCount { get; init; }
        public int MinDayIndex { get; init; }
        public int MaxDayIndex { get; init; }
        public int WeekdayMismatchCount { get; init; }
    }

    private sealed class PlayerProjectionCoverageRow
    {
        public string SessionName { get; init; } = string.Empty;
        public Guid UserId { get; init; }
        public int PlayerEventCount { get; init; }
        public int ProjectionCount { get; init; }
    }

    private sealed class PlayerGameplaySnapshotCoverageRow
    {
        public string SessionName { get; init; } = string.Empty;
        public Guid UserId { get; init; }
        public int RawSnapshotCount { get; init; }
        public int DerivedSnapshotCount { get; init; }
        public int EmptyPayloadCount { get; init; }
    }

    private sealed class PlayerSetupProjectionRow
    {
        public string SessionName { get; init; } = string.Empty;
        public Guid SessionParticipantId { get; init; }
        public int SetupIngredientCount { get; init; }
        public int SetupGoldCount { get; init; }
        public int SetupMissionCount { get; init; }
        public int SetupTieBreakerCount { get; init; }
        public int SetupLoanCount { get; init; }
        public int SetupInsuranceCount { get; init; }
    }

    private sealed class MarketSlotRow
    {
        public string SessionName { get; init; } = string.Empty;
        public string SlotGroup { get; init; } = string.Empty;
        public int SlotCount { get; init; }
    }

    private sealed class ScenarioSystemEventCountRow
    {
        public string SessionName { get; init; } = string.Empty;
        public int MissionAssignedCount { get; init; }
        public int DonationRankAwardedCount { get; init; }
        public int DonationWinnersAnnouncedCount { get; init; }
        public int TieBreakerAssignedCount { get; init; }
    }

    private sealed class ScenarioAlignmentRow
    {
        public string Mode { get; init; } = string.Empty;
        public string PlayerName { get; init; } = string.Empty;
        public int DayIndex { get; init; }
        public int ActionSlot { get; init; }
        public string ActionType { get; init; } = string.Empty;
        public string PayloadKey { get; init; } = string.Empty;
    }

    private sealed class RelationalReadModelCountRow
    {
        public int IngredientCount { get; init; }
        public int NeedPurchaseCount { get; init; }
        public int FinancialGoalCount { get; init; }
        public int DonationEventCount { get; init; }
        public int DonationRankingCount { get; init; }
        public int ActionCounterCount { get; init; }
    }

    private sealed class ReplayLifeRiskRow
    {
        public string RiskCode { get; init; } = string.Empty;
        public string Direction { get; init; } = string.Empty;
        public int Amount { get; init; }
    }

    private sealed class ReplayEventRow
    {
        public string SessionName { get; init; } = string.Empty;
        public string Mode { get; init; } = string.Empty;
        public Guid SessionId { get; init; }
        public Guid EventId { get; init; }
        public Guid? UserId { get; init; }
        public string? PlayerName { get; init; }
        public string ActorType { get; init; } = string.Empty;
        public int DayIndex { get; init; }
        public string Weekday { get; init; } = string.Empty;
        public int ActionSlot { get; init; }
        public long SequenceNumber { get; init; }
        public string ActionType { get; init; } = string.Empty;
        public string Payload { get; init; } = "{}";
    }

    private sealed class ReplaySessionState
    {
        private readonly int _startingCash;

        public ReplaySessionState(string sessionName, string mode, int startingCash)
        {
            SessionName = sessionName;
            Mode = mode;
            _startingCash = startingCash;
        }

        public string SessionName { get; }
        public string Mode { get; }
        public Dictionary<Guid, ReplayPlayerState> Players { get; } = new();
        public Dictionary<(int DayIndex, Guid UserId), int> PrimaryNeedPurchasesByDay { get; } = new();
        public Dictionary<(int DayIndex, Guid UserId), int> ActionTokensByDay { get; } = new();
        public Dictionary<int, Dictionary<Guid, int>> DonationsByDay { get; } = new();
        public Dictionary<int, List<DonationAward>> DonationAwardsByDay { get; } = new();
        public Dictionary<Guid, int> TieBreakers { get; } = new();

        public ReplayPlayerState GetPlayer(Guid? userId, string? playerName)
        {
            if (!userId.HasValue)
            {
                throw new InvalidOperationException("Event pemain wajib memiliki user_id.");
            }

            if (!Players.TryGetValue(userId.Value, out var player))
            {
                player = new ReplayPlayerState(userId.Value, playerName ?? userId.Value.ToString(), _startingCash);
                Players[userId.Value] = player;
            }

            return player;
        }

        public void RecordActionToken(ReplayEventRow evt, bool counts)
        {
            if (!counts || !evt.UserId.HasValue)
            {
                return;
            }

            var key = (evt.DayIndex, evt.UserId.Value);
            ActionTokensByDay[key] = ActionTokensByDay.TryGetValue(key, out var current) ? current + 1 : 1;
        }

        public void RecordDonation(int dayIndex, Guid userId, int amount)
        {
            if (!DonationsByDay.TryGetValue(dayIndex, out var donations))
            {
                donations = new Dictionary<Guid, int>();
                DonationsByDay[dayIndex] = donations;
            }

            donations[userId] = donations.TryGetValue(userId, out var current) ? current + amount : amount;
        }

        public void RecordDonationAward(int dayIndex, Guid userId, int rank, int points)
        {
            if (!DonationAwardsByDay.TryGetValue(dayIndex, out var awards))
            {
                awards = new List<DonationAward>();
                DonationAwardsByDay[dayIndex] = awards;
            }

            awards.Add(new DonationAward(userId, rank, points));
        }

        public void RecordTieBreaker(Guid userId, int tieNumber)
        {
            TieBreakers[userId] = tieNumber;
        }
    }

    private sealed class ReplayPlayerState
    {
        public ReplayPlayerState(Guid userId, string playerName, int startingCash)
        {
            UserId = userId;
            PlayerName = playerName;
            Cash = startingCash;
        }

        public Guid UserId { get; }
        public string PlayerName { get; }
        public int Cash { get; set; }
        public int GoldQty { get; set; }
        public int GoldNetAmount { get; set; }
        public Dictionary<string, int> Ingredients { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> Savings { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> Loans { get; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> PrimaryNeeds { get; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> SecondaryNeeds { get; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> TertiaryNeeds { get; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> AchievedSavingGoals { get; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> InsurancePolicies { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<Guid, int> RiskOutByEventId { get; } = new();
        public HashSet<Guid> InsuranceOffsets { get; } = new();
    }

    private sealed record DonationAward(Guid UserId, int Rank, int Points);
}
