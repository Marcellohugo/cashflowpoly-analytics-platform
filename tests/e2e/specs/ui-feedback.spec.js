// Fungsi file: Memeriksa detail titik grafik, filter, navigasi, dan mekanik ruleset pada kedua peran.
const { test, expect } = require('@playwright/test');

for (const username of ['pratama', 'marco']) {
  test(`catatan UI diterapkan untuk ${username}`, async ({ page }, testInfo) => {
    test.setTimeout(120000);
    const errors = [];
    page.on('pageerror', error => errors.push(error.message));
    await page.goto('/auth/login');
    await page.getByLabel(/nama pengguna|username/i).fill(username);
    await page.getByLabel(/kata sandi|password/i).fill('SeedLocal!2026');
    await page.getByRole('button', { name: /masuk|login/i }).click();
    await expect(page).toHaveURL(/\/$/);
    await page.goto('/sessions');
    const contentGap = () => page.evaluate(() => document.querySelector('#content > section').getBoundingClientRect().top - document.querySelector('[data-quickstart]').getBoundingClientRect().bottom);
    const sessionsGap = await contentGap();
    await page.goto('/statistics?mode=MAHIR&playerId=90000000-0000-0000-0000-000000000011');
    expect(await contentGap()).toBeCloseTo(sessionsGap, 0);
    expect(await page.locator('#statistics-status').evaluate(select => getComputedStyle(select).backgroundPosition)).toContain('16px');
    expect(await page.locator('.statistics-jump').evaluate(nav => getComputedStyle(nav).justifyContent)).toBe('center');
    await expect(page.locator('.statistics-tips-section .data-toggle')).not.toHaveAttribute('open');
    await expect(page.locator('.statistics-chart-meaning')).toHaveCount(0);
    if (username === 'pratama') {
      await page.locator('#statistics-player').fill('Hugo');
      await page.getByRole('button', { name: 'Tampilkan', exact: true }).click();
      await expect(page.locator('.statistics-selected-player-name strong')).toHaveText('Hugo');
      await expect(page.locator('input[name=playerId]')).toHaveValue('90000000-0000-0000-0000-000000000013');
    }
    await page.screenshot({ path: testInfo.outputPath(`${username}-filters.png`) });
    for (const button of await page.locator('.statistics-jump button').all()) {
      await button.click();
      for (const card of await page.locator('[data-statistics-section]:visible .chart-card').all()) {
        const point = card.locator('svg [tabindex="0"]').first();
        if (!await point.count()) continue;
        const panel = card.locator('.js-chart-bar-insight');
        await expect(panel).toBeHidden();
        await point.hover();
        await expect(panel).toBeHidden();
        await point.click();
        const dialog = card.locator('.chart-detail-dialog');
        await expect(dialog).toBeVisible();
        await expect(panel.locator('.js-chart-bar-insight-formula')).not.toHaveText('-');
        await dialog.locator('.chart-detail-close').click();
        await expect(dialog).toBeHidden();
        const payload = JSON.parse(await card.locator('svg').getAttribute('data-chart'));
        const points = card.locator('svg [tabindex="0"]');
        await points.last().click();
        const expected = payload.pointDetails[payload.series[0].values.findLastIndex(value => value !== null)];
        await expect(panel.locator('.js-chart-bar-insight-session')).toHaveText(expected.sessionName);
        await expect(panel.locator('.js-chart-bar-insight-points')).toHaveText(expected.displayValue);
        await expect(panel.locator('.js-chart-bar-insight-unit')).toHaveText(expected.unit);
        await expect(panel.locator('.js-chart-bar-insight-formula')).toHaveText(expected.guidance);
        await page.mouse.click(2, 2);
        await expect(dialog).toBeHidden();
        await point.focus();
        await point.press('Enter');
        await expect(dialog).toBeVisible();
        await page.keyboard.press('Escape');
        await expect(dialog).toBeHidden();
        if (testInfo.project.use.isMobile) {
          await point.tap();
          await expect(dialog).toBeVisible();
          await dialog.locator('.chart-detail-close').click();
        }

      }
    }
    await page.locator('.statistics-jump button').first().click();
    const first = page.locator('[data-statistics-section]:visible .chart-card').first();
    // Bring the chart into view before opening its modal.
    await first.evaluate(card => window.scrollTo({
      top: card.getBoundingClientRect().top + window.scrollY - 110, behavior: 'instant'
    }));
    await first.locator('svg [tabindex="0"]').first().click();
    const detail = first.locator('.statistics-point-detail');
    await expect(detail).toBeVisible();
    await page.screenshot({ path: testInfo.outputPath(`${username}-point-detail.png`), fullPage: false });
    await expect(detail).toBeVisible();
    await page.goto('/sessions/91000000-0000-0000-0000-000000000016/players/90000000-0000-0000-0000-000000000011');
    const toolbar = page.locator('.player-detail-toolbar');
    await expect(toolbar.locator('a[href="/sessions"]')).toContainText('Kembali ke daftar sesi');
    await expect(toolbar.locator('a[href="/sessions/91000000-0000-0000-0000-000000000016"]')).toHaveText('<- Analitika Sesi');
    await expect(toolbar.locator('.player-detail-statistics-link')).toContainText('Statistik Pemain →');
    if (username === 'pratama') {
      await page.goto('/rulesets/create');
      await expect(page.locator('.action-toolbar a[href="/rulesets"]')).toContainText('Kembali ke daftar ruleset');
      for (const id of ['cfg-friday', 'cfg-saturday', 'cfg-sunday', 'cfg-gold-buy', 'cfg-gold-sell']) {
        const input = page.locator('#' + id);
        await expect(input).toBeChecked();
        await expect(input).toBeDisabled();
        const row = input.locator('..');
        const [key, checkbox, badge] = await row.evaluate(row => ['.metric-key', 'input', '.ruleset-coming-soon'].map(selector => row.querySelector(selector).getBoundingClientRect().toJSON()));
        expect(Math.abs((key.y + key.height / 2) - (checkbox.y + checkbox.height / 2))).toBeLessThan(2);
        expect(badge.y).toBeGreaterThan(key.y + key.height);
      }
      await page.locator('#cfg-gold-buy').locator('..').screenshot({ path: testInfo.outputPath('gold-toggle.png') });
    }
    expect(errors).toEqual([]);
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1)).toBe(true);
  });
}
