// Fungsi file: Memeriksa histori pribadi, sumber angka, akses direktori, dan pendaftaran instruktur pada browser desktop serta ponsel.
const { test, expect } = require("@playwright/test");
const password = process.env.E2E_PASSWORD || "SeedLocal!2026";
const apiUrl = process.env.E2E_API_URL || "http://localhost:5041";
const playerId = "90000000-0000-0000-0000-000000000011";

async function login(page, username = "marco") {
  await page.goto("/auth/login");
  await page.getByLabel(/nama pengguna|username/i).fill(username);
  await page.getByLabel(/kata sandi|password/i).fill(password);
  await page.getByRole("button", { name: /masuk|login/i }).click();
  await expect(page).toHaveURL(/\/$/);
}

test("statistik satu mode memakai angka analitika, kategori tunggal, dan akses sendiri", async ({ page, request }, testInfo) => {
  test.setTimeout(120000);
  const errors = [];
  page.on("pageerror", error => errors.push(error.message));
  await login(page);
  await expect(page.locator('a[href="/players"]')).toHaveCount(0);
  const removed = await page.goto("/players");
  expect(removed.status()).toBe(404);
  await page.goto("/statistics?mode=MAHIR");
  await expect(page.locator("h1")).toHaveText("Statistik Pemain");
  await expect(page.locator(".statistics-chart")).toHaveCount(19);
  await expect(page.locator(".statistics-session")).toHaveCount(8);
  await expect(page.locator(".statistics-chart svg circle").first()).toBeVisible();
  await expect(page.getByText("Direktori Pemain", { exact: true })).toHaveCount(0);
  await expect(page.locator('.statistics-reading')).not.toHaveAttribute('open');
  await expect(page.locator('#statistics-mode option')).toHaveCount(2);
  await expect(page.locator('#statistics-player')).toHaveCount(0);
  await expect(page.locator('.player-statistics > .page-intro .page-intro-actions')).toHaveCount(0);
  await expect(page.locator('.statistics-selected-player')).toHaveCount(0);
  await expect(page.locator('.statistics-jump a')).toHaveCount(0);
  await expect(page.locator('.statistics-jump button')).toHaveCount(5);
  await page.locator('.statistics-reading summary').click();
  await expect(page.locator('.statistics-reading')).toHaveAttribute('open');
  await page.reload();
  await expect(page.locator('.statistics-reading')).not.toHaveAttribute('open');
  for (const button of await page.locator('.statistics-jump button').all()) {
    await button.click();
    await expect(page.locator('[data-statistics-section]:visible')).toHaveCount(1);
    const target = await button.getAttribute('aria-controls');
    await expect(page.locator('#' + target)).toBeVisible();
    await expect(button).toHaveAttribute('aria-expanded', 'true');
    await expect(page.locator('#statistics-sessions')).toBeVisible();
    const widths = await page.locator('#' + target + ' .statistics-chart').evaluateAll(cards => cards.map(card => ({ left: card.getBoundingClientRect().left, width: card.getBoundingClientRect().width })));
    expect(new Set(widths.map(card => Math.round(card.left))).size).toBe(1);
    for (const card of await page.locator('#' + target + ' .statistics-chart').all()) {
      const plot = card.locator('.statistics-chart-plot');
      await expect(plot.locator('svg')).toHaveAttribute('data-chart-responsive');
      await expect.poll(async () => plot.locator('svg').evaluate(svg => Math.abs(svg.viewBox.baseVal.width - svg.getBoundingClientRect().width))).toBeLessThan(2);
      const headingBox = await card.locator('h3').boundingBox();
      expect((await plot.boundingBox()).width).toBeCloseTo(headingBox.width, 0);
      const labels = await plot.locator('svg > text').evaluateAll(nodes => nodes.filter(node => /^Sesi \d+$/.test(node.textContent)).map(node => ({ left: node.getBoundingClientRect().left, right: node.getBoundingClientRect().right })));
      expect(labels.every((label, index) => index === 0 || label.left >= labels[index - 1].right + 4)).toBeTruthy();
      await card.locator('.statistics-chart-detail summary').click();
      await expect(card.locator('tbody tr:visible')).toHaveCount(5);
      await expect(card.locator('[data-page-back]')).toBeDisabled();
      await card.locator('[data-page-next]').click();
      await expect(card.locator('tbody tr:visible')).toHaveCount(3);
      await expect(card.locator('[data-page-status-text]')).toHaveText('Baris 6–8 dari 8');
      await expect(card.locator('[data-page-next]')).toBeDisabled();
      await card.locator('[data-page-back]').click();
      await expect(card.locator('tbody tr:visible')).toHaveCount(5);
      await expect(card.locator('tbody tr:visible').first().locator('th')).toContainText('1.');
      await card.locator('.statistics-chart-detail summary').click();
    }
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1)).toBeTruthy();
  }
  await page.locator('[data-statistics-panel="statistics-overview"]').click();
  const auth = await request.post(`${apiUrl}/api/v1/auth/login`, { data: { username: "marco", password } });
  expect(auth.ok()).toBeTruthy();
  const headers = { Authorization: `Bearer ${(await auth.json()).access_token}` };
  const sessionLinks = await page.locator('.statistics-session a[href*="/players/"]').evaluateAll(a => a.map(a => a.getAttribute("href")));
  for (const link of sessionLinks) {
    expect(link).toContain(`/players/${playerId}`);
    const sessionId = link.split("/")[2];
    const gameplay = await (await request.get(`${apiUrl}/api/v1/analytics/sessions/${sessionId}/players/${playerId}/gameplay`, { headers })).json();
    const index = Number((await page.locator(`.statistics-session:has(a[href="${link}"]) h3`).innerText()).split(".")[0]) - 1;
    for (const [key, expected] of Object.entries({
      total_happiness_points: gameplay.score.happiness_points_total,
      coins_net_end_game: gameplay.economy.starting_cash + gameplay.economy.cashflow_net_total,
      cash_growth_percent: gameplay.derived_json.cash_growth_percent - 100,
      income_action_focus_percent: gameplay.derived_json.income_action_focus_percent,
      happiness_source_diversity_percent: gameplay.derived_json.happiness_source_diversity_percent
    })) {
      const chart = JSON.parse(await page.locator(`[data-statistics-metric="${key}"] svg`).getAttribute("data-chart"));
      expect(chart.series[0].values[index]).toBeCloseTo(expected, 6);
    }
  }
  await expect(page.locator('.statistics-chart-latest')).toHaveCount(0);
  for (const [key, value] of Object.entries({ cash_growth_percent: '+20', financial_goal_completion_percent: '100', sharia_loans_taken: '1', sharia_loans_repaid: '1', donation_commitment_score: '9.76' })) {
    await expect(page.locator(`[data-statistics-metric="${key}"] tbody tr`).first().locator('td').first()).toContainText(value);
  }
  await page.screenshot({ path: testInfo.outputPath("statistics-top.png"), fullPage: false });
  await page.locator("#statistics-overview").scrollIntoViewIfNeeded();
  await page.screenshot({ path: testInfo.outputPath("statistics-charts.png"), fullPage: false });
  const overflow = await page.evaluate(() => document.documentElement.scrollWidth > window.innerWidth + 1);
  expect(overflow).toBeFalsy();
  await page.locator('[data-statistics-panel="statistics-money"]').click();
  await page.locator('[data-statistics-metric="cash_growth_percent"] summary').click();
  await expect(page.locator('[data-statistics-metric="cash_growth_percent"] table tbody tr')).toHaveCount(8);
  await page.locator('[data-statistics-metric="cash_growth_percent"] svg [tabindex="0"]').first().hover();
  await expect(page.locator('[data-statistics-metric="cash_growth_percent"] .js-chart-bar-insight')).toContainText("+20");
  await page.goto('/sessions');
  await expect(page.locator('.players-session-card')).toHaveCount(16);
  await expect(page.locator('.session-stats-grid .stat-card')).toHaveCount(4);
  await expect(page.locator('.players-session-detail')).toHaveCount(16);
  const ownLinks = await page.locator('.players-session-table a').evaluateAll(links => links.map(a => a.getAttribute('href')));
  expect(ownLinks).toHaveLength(16);
  expect(ownLinks.every(link => link.endsWith('/players/' + playerId))).toBeTruthy();
  await expect(page.locator('.session-stats-grid .stat-card').last()).toContainText('6');
  for (const table of await page.locator('.players-session-card .table-wrap').all()) {
    await table.scrollIntoViewIfNeeded();
    await expect(table).toHaveCSS('opacity', '1');
  }
  await page.screenshot({ path: testInfo.outputPath('session-participants.png'), fullPage: true });
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1)).toBeTruthy();
  const foreign = await page.goto('/statistics?playerId=90000000-0000-0000-0000-000000000012');
  expect(foreign.status()).toBe(403);
  await page.goto(sessionLinks[0].split('/players/')[0]);
  const detailLinks = await page.locator('a[href*="/players/"]').evaluateAll(links => links.map(a => a.getAttribute('href')));
  expect(detailLinks.every(link => link.endsWith('/players/' + playerId))).toBeTruthy();
  await page.goto(sessionLinks[0]);
  await expect(page.locator('.player-detail-statistics-link')).toBeVisible();
  await expect(page.locator('a[href="/players"]')).toHaveCount(0);
  await page.goto("/statistics");
  await page.locator("#statistics-mode").selectOption("PEMULA");
  await page.getByRole("button", { name: "Tampilkan", exact: true }).click();
  await expect(page.locator(".statistics-session")).toHaveCount(8);
  await expect(page.locator(".statistics-chart")).toHaveCount(14);
  await expect(page.locator("#statistics-future")).toHaveCount(0);
  await expect(page.locator('.statistics-chart svg [tabindex="0"]').first()).toBeVisible();
  await page.locator("#statistics-status").selectOption("CREATED");
  await page.getByRole("button", { name: "Tampilkan", exact: true }).click();
  await expect(page.getByRole("heading", { name: "Tidak ada sesi yang cocok" })).toBeVisible();
  await page.goto("/statistics");
  await page.locator(".nav-dropdown-lang summary").click();
  await page.getByRole("button", { name: "Bahasa Inggris (EN)", exact: true }).click();
  await expect(page.locator("h1")).toHaveText("Player Statistics");
  await expect(page.locator('[data-statistics-metric="total_happiness_points"] h3')).toHaveText("Total Happiness Points");
  await expect(page.locator('[data-statistics-metric="meal_orders_claimed"] h3')).toHaveText("Completed Orders");
  expect(errors).toEqual([]);
});

test("daftar umum menawarkan akun instruktur dan memberikan akses instruktur", async ({ page }, testInfo) => {
  const username = `e2e_instructor_${Date.now()}`;
  await page.goto("/auth/register");
  await page.locator("#register-role").selectOption("INSTRUCTOR");
  await page.locator("#register-display-name").fill("Instruktur Uji Publik");
  await page.locator("#register-username").fill(username);
  await page.locator("#register-password").fill(password);
  await page.locator("#register-confirm-password").fill(password);
  await page.screenshot({ path: testInfo.outputPath("register-instructor.png") });
  await page.getByRole("button", { name: "Buat Akun", exact: true }).click();
  await expect(page).toHaveURL(/\/$/);
  await page.goto("/rulesets/create");
  await expect(page.locator("#ruleset-name")).toBeVisible();
  await expect(page.locator('a[href="/players"]')).toHaveCount(0);
  await expect(page.locator('a[href="/statistics"]').first()).toBeAttached();
  await page.locator('input[name="cfg-mode"][value="MAHIR"]').check();
  await expect(page.locator('input[type="checkbox"]')).toHaveCount(8);
  for (const box of await page.locator('input[type="checkbox"]').all()) {
    await expect(box).toBeChecked();
    await expect(box).toBeDisabled();
  }
  await expect(page.locator(".ruleset-coming-soon")).toHaveCount(8);
  await page.locator("#cfg-adv-loan").scrollIntoViewIfNeeded();
  await page.screenshot({ path: testInfo.outputPath("ruleset-coming-soon.png") });
});

test("pemain baru memiliki histori kosong tanpa direktori", async ({ page }) => {
  await page.goto("/auth/register");
  await page.locator("#register-role").selectOption("PLAYER");
  await page.locator("#register-display-name").fill("Pemain Uji Histori");
  await page.locator("#register-username").fill(`e2e_history_${Date.now()}`);
  await page.locator("#register-password").fill(password);
  await page.locator("#register-confirm-password").fill(password);
  await page.getByRole("button", { name: "Buat Akun", exact: true }).click();
  await expect(page).toHaveURL(/\/$/);
  await expect(page.locator('#home-total-sessions')).toHaveText('0');
  await expect(page.locator('#home-total-players')).toHaveText('0');
  await page.goto("/statistics");
  await expect(page.getByRole("heading", { name: "Belum ada statistik pemain" })).toBeVisible();
  await expect(page.locator(".statistics-chart")).toHaveCount(0);
  await expect(page.getByText("Direktori Pemain", { exact: true })).toHaveCount(0);
});

for (const [instructor, selectedPlayer] of [
  ['hadziq', '90000000-0000-0000-0000-000000000012'],
  ['pratama', '90000000-0000-0000-0000-000000000021']
]) {
test(`instruktur ${instructor} memilih peserta sesinya dan dapat membuka statistik per mode`, async ({ page }, testInfo) => {
  await login(page, instructor);
  await page.goto('/sessions');
  await expect(page.locator('.players-session-card')).toHaveCount(8);
  await expect(page.locator('.players-session-table a')).toHaveCount(32);
  await expect(page.locator('a[href="/players"]')).toHaveCount(0);
  await page.goto('/statistics?mode=MAHIR&playerId=' + playerId);
  await expect(page.locator('#statistics-players option')).toHaveCount(4);
  await expect(page.locator('#statistics-player')).toHaveValue('Marco');
  await expect(page.locator('.player-statistics > .page-intro .page-intro-actions')).toHaveCount(0);
  await expect(page.locator('.statistics-selected-player-name > strong')).toHaveText('Marco');
  await expect(page.locator('.statistics-selected-player .hint-chip')).toContainText('Mahir');
  await expect(page.locator('.statistics-session')).toHaveCount(4);
  await expect(page.locator('#statistics-mode option')).toHaveCount(2);
  await page.screenshot({ path: testInfo.outputPath('instructor-statistics.png'), fullPage: false });
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1)).toBeTruthy();
  const selectedName = await page.locator('#statistics-players option[data-player-id="' + selectedPlayer + '"]').getAttribute('value');
  await page.locator('#statistics-player').fill('Nama tidak tersedia');
  expect(await page.locator('#statistics-player').evaluate(input => input.checkValidity())).toBe(false);
  await page.locator('#statistics-player').fill(selectedName);
  await page.getByRole('button', { name: 'Tampilkan', exact: true }).click();
  await expect(page.locator('.statistics-selected-player-name > strong')).toHaveText(selectedName);
  const links = await page.locator('.statistics-session a').evaluateAll(links => links.map(a => a.getAttribute('href')));
  expect(links.every(link => link.endsWith('/players/' + selectedPlayer))).toBeTruthy();
  for (const mode of ['MAHIR', 'PEMULA']) {
    await page.locator('#statistics-mode').selectOption(mode);
    await page.getByRole('button', { name: 'Tampilkan', exact: true }).click();
    await expect(page.locator('.statistics-session')).toHaveCount(4);
    await expect(page.locator('.statistics-chart-latest')).toHaveCount(0);
    const card = page.locator('[data-statistics-metric="total_happiness_points"]');
    await card.locator('summary').click();
    await expect(card.locator('tbody tr:visible')).toHaveCount(4);
    await expect(card.locator('[data-page-back]')).toBeDisabled();
    await expect(card.locator('[data-page-next]')).toBeDisabled();
    await expect(card.locator('[data-page-status-text]')).toHaveText('Baris 1–4 dari 4');
  }
  const foreignPlayer = instructor === 'hadziq' ? '90000000-0000-0000-0000-000000000021' : '90000000-0000-0000-0000-000000000012';
  expect((await page.goto('/statistics?playerId=' + foreignPlayer)).status()).toBe(404);
  const inaccessible = await page.goto('/statistics?playerId=99999999-0000-0000-0000-000000000099');
  expect(inaccessible.status()).toBe(404);
});
}
