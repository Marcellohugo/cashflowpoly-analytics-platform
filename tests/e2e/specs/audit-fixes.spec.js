// Fungsi file: Memverifikasi halaman galat, batas input, dan polling timeline setelah perbaikan audit.
const { test, expect } = require('@playwright/test');
const password = process.env.E2E_PASSWORD || 'SeedLocal!2026';
const sessionId = '91000000-0000-0000-0000-000000000016';
async function login(page, username) {
  await page.goto('/auth/login');
  await page.getByLabel(/nama pengguna|username/i).fill(username);
  await page.getByLabel(/kata sandi|password/i).fill(password);
  await page.getByRole('button', { name: /masuk|login/i }).click();
  await expect(page).toHaveURL(/\/$/);
}
test.beforeEach(async ({ page }) => page.emulateMedia({ reducedMotion: 'reduce' }));

for (const [username, status] of [['pratama', 404], ['marco', 403]]) {
  test(`galat statistik ${username}: penjelasan dan navigasi tetap tersedia`, async ({ page }) => {
    await login(page, username);
    const response = await page.goto('/statistics?playerId=00000000-0000-0000-0000-000000000001');
    expect(response.status()).toBe(status);
    await expect(page.locator('main h1')).toBeVisible();
    await expect(page.locator('main a[href="/sessions"]')).toBeVisible();
    await expect(page.locator('main')).not.toContainText(/Development Mode|Development environment/);
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1)).toBeTruthy();
    await page.locator('main a[href="/sessions"]').click();
    await expect(page).toHaveURL(/\/sessions$/);
  });
}

test('batas formulir sesuai database dan petunjuk kata sandi terbaca', async ({ page }) => {
  await page.goto('/auth/register');
  await expect(page.locator('#register-password')).toHaveAttribute('minlength', '12');
  await expect(page.locator('#register-password')).toHaveAttribute('aria-describedby', 'register-password-hint');
  await expect(page.locator('#register-password-hint')).toContainText('72');
  await expect(page.locator('#register-display-name')).toHaveAttribute('maxlength', '80');
  await expect(page.locator('#register-username')).toHaveAttribute('minlength', '3');
  await expect(page.locator('#register-username')).toHaveAttribute('maxlength', '80');
  await page.locator('#register-username').fill('ab');
  expect(await page.locator('#register-username').evaluate(input => input.validity.tooShort)).toBe(true);
  await login(page, 'pratama');
  await page.goto('/rulesets/create');
  await expect(page.locator('#ruleset-name')).toHaveAttribute('maxlength', '120');
  await page.locator('#ruleset-name').fill('a'.repeat(121));
  expect((await page.locator('#ruleset-name').inputValue()).length).toBe(120);
  await page.locator('#cfg-cash').fill('1e2');
  expect(JSON.parse(await page.locator('#DefinitionJson').inputValue()).starting_cash).toBe(100);
  await page.locator('#cfg-cash').fill('1.5');
  expect(await page.locator('#cfg-cash').evaluate(input => input.validity.stepMismatch)).toBe(true);
  expect(JSON.parse(await page.locator('#DefinitionJson').inputValue()).starting_cash).toBe(100);
});

test('tujuan SYSTEM terlihat pada filter pemain dan fokus kalender bertahan saat polling', async ({ page }) => {
  await login(page, 'pratama');
  await page.clock.install();
  let goal;
  let polls = 0;
  await page.route(`**/sessions/${sessionId}`, async route => {
    const response = await route.fetch();
    let body = await response.text();
    const raw = body.match(/const initialTimelineRaw = (.+);/);
    const events = JSON.parse(raw[1]);
    const player = events.find(item => item.ActorType === 'PLAYER' && item.PlayerId);
    expect(player).toBeTruthy();
    const action = { ...player, DayIndex: 1, SequenceNumber: 1 };
    goal = { ...action, ActorType: 'SYSTEM', ActionType: 'TujuanFinansial', ActionSlot: 0,
      ActionSlotRole: 'system', ActionSlotLabel: 'Sistem', SequenceNumber: 2, FlowDescription: 'TUJUAN_TERCATAT' };
    body = body.replace(raw[0], `const initialTimelineRaw = ${JSON.stringify([action, goal])};`);
    await route.fulfill({ response, body });
  });
  await page.route(`**/sessions/${sessionId}/timeline?*`, async route => {
    polls++;
    await route.fulfill({ json: {
      timeline: polls === 2 ? [{ ...goal, DayIndex: 2, SequenceNumber: 3, FlowDescription: 'TUJUAN_KEDUA' }] : [],
      refreshedTimeline: [], nextCursor: 'focus-probe', hasMore: false, errorMessage: null,
      lastSyncedAt: new Date().toISOString()
    } });
  });
  await page.goto(`/sessions/${sessionId}`);
  await expect.poll(() => polls).toBe(1);
  await page.locator('[data-day="1"]').click();
  await expect(page.locator('#session-journey-feed')).toContainText('TUJUAN_TERCATAT');
  await page.locator(`[data-timeline-filter="player:${goal.PlayerId.toLowerCase()}"]`).click();
  await expect(page.locator('#session-journey-feed')).toContainText('TUJUAN_TERCATAT');
  const day = page.locator('[data-day="1"]');
  await day.focus();
  await day.press('Enter');
  await expect(day).toBeFocused();
  await page.clock.runFor(10050);
  await expect.poll(() => polls).toBeGreaterThanOrEqual(2);
  await expect(page.locator('[data-day="2"]')).toHaveAttribute('aria-disabled', 'false');
  await expect(page.locator('#session-journey-feed')).toContainText('TUJUAN_TERCATAT');
  await expect(day).toBeFocused();
  await page.clock.runFor(10050);
  await expect.poll(() => polls).toBeGreaterThanOrEqual(3);
  await expect(day).toBeFocused();
});

test('timeline melanjutkan cursor awal dan membuka serta menyegel ulang donasi tanpa reload', async ({ page }) => {
  await login(page, 'pratama');
  await page.clock.install();
  let donation;
  let originalCursor;
  const polls = [];
  await page.route(`**/sessions/${sessionId}`, async route => {
    const response = await route.fetch();
    let body = await response.text();
    const raw = body.match(/const initialTimelineRaw = (.+);/);
    expect(raw).toBeTruthy();
    const initial = JSON.parse(raw[1]);
    donation = initial.find(e => e.ActionType === 'JumatBerkah');
    expect(donation).toBeTruthy();
    donation.IsSealed = true;
    donation.FlowDescription = 'DONASI_MASIH_RAHASIA';
    body = body.replace(raw[0], `const initialTimelineRaw = ${JSON.stringify(initial)};`);
    originalCursor = JSON.parse(body.match(/let timelineCursor = (.+);/)[1]);
    expect(originalCursor).toBeTruthy();
    await route.fulfill({ response, body });
  });
  await page.route(`**/sessions/${sessionId}/timeline?*`, async route => {
    const url = new URL(route.request().url());
    polls.push(url);
    await route.fulfill({ json: {
      timeline: [], refreshedTimeline: [{ ...donation, IsSealed: polls.length > 1,
        FlowDescription: polls.length > 1 ? 'DONASI_DISEGEL_UNDO' : 'DONASI_SUDAH_TERBUKA' }],
      nextCursor: originalCursor, hasMore: false, errorMessage: null, lastSyncedAt: new Date().toISOString()
    } });
  });
  await page.goto(`/sessions/${sessionId}`);
  await expect.poll(() => polls.length).toBeGreaterThan(0);
  expect(polls[0].searchParams.get('cursor')).toBe(originalCursor);
  expect(polls[0].searchParams.get('refreshSequences').split(',')).toContain(String(donation.SequenceNumber));
  await page.locator('[data-day="5"]').click();
  await expect(page.locator('#session-journey-feed')).toContainText('DONASI_SUDAH_TERBUKA');
  await page.clock.runFor(10050);
  await expect.poll(() => polls.length).toBeGreaterThan(1);
  expect(polls[1].searchParams.get('refreshSequences').split(',')).toContain(String(donation.SequenceNumber));
  await expect(page.locator('#session-journey-feed')).toContainText('DONASI_DISEGEL_UNDO');
});
