// Fungsi file: Mengaudit struktur UI desktop dan ponsel serta mencocokkan nilai grafik dengan snapshot API.
const { test, expect } = require('@playwright/test');
const apiUrl = process.env.E2E_API_URL || 'http://localhost:5041';
const password = process.env.E2E_PASSWORD || 'SeedLocal!2026';
// Capture final layout while other browser suites exercise the normal entrance animation.
test.beforeEach(async ({ page }) => { await page.emulateMedia({ reducedMotion: 'reduce' }); });

async function inspectStructure(page) {
  await expect(page.locator('main')).toHaveCount(1);
  await expect(page.locator('h1')).toHaveCount(1);
  const issues = await page.evaluate(() => {
    const ids = [...document.querySelectorAll('[id]')].map(e => e.id);
    return {
      duplicateIds: ids.filter((id, i) => ids.indexOf(id) !== i),
      overflow: document.documentElement.scrollWidth > innerWidth + 1,
      brokenImages: [...document.images].filter(i => i.getClientRects().length && i.complete && !i.naturalWidth).map(i => i.getAttribute('src')),
      brokenSections: [...document.querySelectorAll('a[href^="#"]')].map(a => a.getAttribute('href').slice(1)).filter(id => id && !document.getElementById(id))
    };
  });
  expect(issues, page.url()).toEqual({ duplicateIds: [], overflow: false, brokenImages: [], brokenSections: [] });
}

for (const [role, username] of [['instructor', 'hadziq'], ['player', 'marco']]) {
  test(`audit ${role}: seluruh halaman dan angka statistik cocok dengan sumber API`, async ({ page, request }, testInfo) => {
    test.setTimeout(300000);
    // Audit ini membaca banyak sesi. Tunggu jendela API sebelumnya lalu beri jeda
    // antarsesi agar hasil verifikasi tidak berubah menjadi respons pembatasan 429.
    await new Promise(resolve => setTimeout(resolve, 60000));
    const errors = [];
    page.on('pageerror', error => errors.push(error.message));
    await page.goto('/auth/login');
    await page.getByLabel(/nama pengguna|username/i).fill(username);
    await page.getByLabel(/kata sandi|password/i).fill(password);
    await page.getByRole('button', { name: /masuk|login/i }).click();
    await expect(page).toHaveURL(/\/$/);
    const login = await request.post(`${apiUrl}/api/v1/auth/login`, { data: { username, password } });
    expect(login.ok()).toBeTruthy();
    const user = await login.json();
    const headers = { Authorization: `Bearer ${user.access_token}` };
    async function get(path) {
      const response = await request.get(`${apiUrl}/api/v1/${path}`, { headers });
      expect(response.ok(), path).toBeTruthy();
      return response.json();
    }
    const sessions = (await get('sessions')).items;
    const players = (await get(role === 'instructor' ? 'players?inMySessions=true' : 'players')).items;
    const rulesets = (await get('rulesets')).items;
    await expect(page.locator('#home-total-sessions')).toHaveText(String(sessions.length));
    await expect(page.locator('#home-total-players')).toHaveText(String(new Set(players.map(p => p.user_id)).size));
    await inspectStructure(page);
    await page.screenshot({ path: testInfo.outputPath('home.png') });
    const routes = ['/sessions', '/statistics', '/rulebook', '/privacy', '/terms'];
    if (role === 'instructor') {
      routes.push('/rulesets', '/rulesets/create');
      const ruleset = rulesets.find(r => r.ruleset_id);
      if (ruleset) routes.push(`/rulesets/${ruleset.ruleset_id}`);
    }
    for (const route of routes) {
      expect((await page.goto(route)).status(), route).toBe(200);
      await inspectStructure(page);
      await page.screenshot({ path: testInfo.outputPath(route.replaceAll('/', '_') + '.png') });
    }
    let comparedValues = 0;
    const statisticsByPlayerAndMode = new Map();
    for (const session of sessions.filter(s => s.status === 'ENDED')) {
      await new Promise(resolve => setTimeout(resolve, role === 'instructor' ? 15000 : 6000));
      const roster = (await get(`sessions/${session.session_id}/players`)).items;
      expect((await page.goto(`/sessions/${session.session_id}`)).status()).toBe(200);
      await inspectStructure(page);
      const summary = await get(`analytics/sessions/${session.session_id}`);
      expect(summary.ruleset_version_id).toBeTruthy();
      for (const participant of roster.filter(p => role === 'instructor' || p.user_id === user.user_id)) {
        const gameplay = await get(`analytics/sessions/${session.session_id}/players/${participant.user_id}/gameplay`);
        // Satu halaman statistik sudah memuat seluruh sesi pemain pada mode ini.
        // Hindari memuat ulang grafik identik pada tiap sesi dan membanjiri batas request API.
        const statisticsKey = `${participant.user_id}:${session.mode}`;
        if (!statisticsByPlayerAndMode.has(statisticsKey)) {
          // Periksa struktur dan ringkasan analitika untuk setiap pemain/mode sekali.
          // Seluruh titik dari semua sesi tetap dibandingkan di bawah.
          await page.goto(`/sessions/${session.session_id}/players/${participant.user_id}`);
          await inspectStructure(page);
          const values = page.locator('.player-analysis-scorecard dd');
          expect(Number(await values.nth(0).innerText())).toBeCloseTo(gameplay.economy.starting_cash + gameplay.economy.cashflow_net_total, 2);
          expect(Number(await values.nth(1).innerText())).toBeCloseTo(gameplay.economy.cashflow_net_total, 2);
          expect(Number(await values.nth(3).innerText())).toBeCloseTo(gameplay.score.happiness_points_total, 2);
          await page.goto(`/statistics?mode=${session.mode}&status=ENDED&playerId=${participant.user_id}`);
          await inspectStructure(page);
          await expect(page.locator('.player-statistics > [role="alert"]')).toHaveCount(0);
          const indexes = await page.locator('.statistics-session').evaluateAll(cards => Object.fromEntries(cards.map(card => [
            card.querySelector('a').getAttribute('href').split('/')[2], Number(card.querySelector('h3').textContent.trim().split('.')[0]) - 1
          ])));
          const charts = await page.locator('[data-statistics-metric]').evaluateAll(cards => cards.map(card => ({
            key: card.dataset.statisticsMetric, values: JSON.parse(card.querySelector('svg').getAttribute('data-chart')).series[0].values
          })));
          statisticsByPlayerAndMode.set(statisticsKey, { indexes, charts });
        }
        const statistics = statisticsByPlayerAndMode.get(statisticsKey);
        const index = statistics.indexes[session.session_id];
        expect(index).toBeGreaterThanOrEqual(0);
        const expected = {
          ...gameplay.derived_json,
          total_happiness_points: gameplay.score.happiness_points_total,
          coins_net_end_game: gameplay.economy.starting_cash + gameplay.economy.cashflow_net_total,
          cash_in_total: gameplay.economy.cash_in_total,
          cash_out_total: gameplay.economy.cash_out_total,
          cash_growth_percent: gameplay.derived_json.cash_growth_percent == null ? null : gameplay.derived_json.cash_growth_percent - 100,
          meal_orders_claimed: gameplay.progress.orders_completed_count,
          sharia_loans_taken: gameplay.raw_json.financial_goals?.sharia_loans_taken,
          sharia_loans_repaid: gameplay.raw_json.financial_goals?.sharia_loans_repaid
        };
        for (const { key, values } of statistics.charts) {
          if (expected[key] == null) expect(values[index], key).toBeNull();
          else expect(values[index], `${session.session_id}/${participant.user_id}/${key}`).toBeCloseTo(expected[key], 6);
          comparedValues++;
        }
      }
    }
    testInfo.annotations.push({ type: 'data comparisons', description: `${comparedValues} chart values matched to API snapshots` });
    expect(errors).toEqual([]);
  });
}

test('audit publik: formulir dan halaman informasi terbaca', async ({ page }, testInfo) => {
  for (const route of ['/auth/login', '/auth/register', '/rulebook', '/privacy', '/terms']) {
    expect((await page.goto(route)).status()).toBe(200);
    await inspectStructure(page);
    await page.screenshot({ path: testInfo.outputPath(route.replaceAll('/', '_') + '.png') });
  }
});

test('ringkasan beranda tidak mengubah data null menjadi nol', async ({ page }) => {
  await page.goto('/auth/login');
  await page.getByLabel(/nama pengguna|username/i).fill('marco');
  await page.getByLabel(/kata sandi|password/i).fill(password);
  await page.getByRole('button', { name: /masuk|login/i }).click();
  await expect(page).toHaveURL(/\/$/);
  await page.route('**/Home/RealtimeStats', route => route.fulfill({ json: {
    totalSessions: null, activeSessions: null, totalPlayers: 0, totalRulesets: null, errorMessage: 'Sumber sesi gagal dimuat'
  } }));
  await page.reload();
  await expect(page.locator('#home-total-sessions')).toHaveText('—');
  await expect(page.locator('#home-active-sessions')).toHaveText('—');
  await expect(page.locator('#home-total-players')).toHaveText('0');
  await expect(page.locator('#home-realtime-error')).toContainText('Sumber sesi gagal dimuat');
});
