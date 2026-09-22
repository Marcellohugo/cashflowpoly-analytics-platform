// Fungsi file: Menguji nama pemain ambigu dan format waktu browser tanpa mengubah data akun.
const { test, expect } = require('@playwright/test');
const path = require('node:path');
const scripts = path.resolve(__dirname, '../../../src/Cashflowpoly.Ui/wwwroot/js');

test('filter dan paginasi set aturan tidak menyisakan kartu tersembunyi', async ({ page }) => {
  await page.emulateMedia({ reducedMotion: 'reduce' });
  await page.goto('/auth/login');
  await page.getByLabel(/nama pengguna|username/i).fill('hadziq');
  await page.getByLabel(/kata sandi|password/i).fill(process.env.E2E_PASSWORD || 'SeedLocal!2026');
  await page.getByRole('button', { name: /masuk|login/i }).click();
  await expect(page).toHaveURL(/\/$/);
  await page.goto('/rulesets');
  await page.locator('[data-ruleset-page-size]').selectOption('5');
  const visibleRows = page.locator('[data-ruleset-row]:visible');
  await expect(visibleRows).toHaveCount(5);
  const firstPage = await visibleRows.first().getAttribute('data-ruleset-id');
  await page.locator('[data-ruleset-next]').click();
  await expect(visibleRows).toHaveCount(5);
  await expect(visibleRows.first()).not.toHaveAttribute('data-ruleset-id', firstPage);
  await page.locator('[data-ruleset-search]').fill('tidak-ada-set-aturan-ini');
  await expect(visibleRows).toHaveCount(0);
  await expect(page.locator('[data-ruleset-tbody] > tr:visible')).toHaveCount(1);
  expect(await page.locator('[data-ruleset-tbody]').evaluate(body => body.getBoundingClientRect().height)).toBeLessThan(120);
  await page.locator('[data-ruleset-reset]').click();
  await expect(visibleRows).toHaveCount(10);
});

test('titik berdekatan pada histori panjang membuka popup masing-masing', async ({ page }) => {
  const labels = Array.from({ length: 40 }, (_, i) => `Sesi ${i + 1}`);
  const payload = { labels, series: [{ name: 'Nilai', values: labels.map(() => 10) }] };
  await page.setContent('<article class="chart-card"><svg class="js-metric-line-chart" viewBox="0 0 300 240" style="width:300px;height:240px"></svg></article>');
  await page.locator('svg').evaluate((svg, payload) => { svg.dataset.chart = JSON.stringify(payload); }, payload);
  await page.addScriptTag({ path: path.join(scripts, 'player-detail-charts.js') });
  const points = page.locator('svg [tabindex="0"]');
  await expect(points).toHaveCount(40);
  for (let i = 0; i < 40; i++) {
    await points.nth(i).click();
    await expect(page.locator('.js-chart-bar-insight-metric')).toHaveText(`Sesi ${i + 1}`);
    await page.locator(".chart-detail-close").click();
  }
  await page.mouse.move(310, 10);
  await expect(page.locator('.js-chart-bar-insight')).toBeHidden();
});

test('nama yang mirip tidak diam-diam memilih identitas pertama', async ({ page }) => {
  await page.setContent(`<form>
    <input id="statistics-player" list="statistics-players" data-invalid-player="Pilih pemain yang sesuai">
    <input name="playerId" value="second">
    <datalist id="statistics-players">
      <option value="Marco" data-player-id="first"></option>
      <option value="marco" data-player-id="second"></option>
      <option value="Hugo (third)" data-player-id="third"></option>
      <option value="Hugo (fourth)" data-player-id="fourth"></option>
    </datalist>
  </form>`);
  await page.addScriptTag({ path: path.join(scripts, 'player-statistics.js') });
  const name = page.locator('#statistics-player');
  const identity = page.locator('input[name=playerId]');
  await expect(identity).toHaveValue('second');
  await name.fill('MARCO');
  await expect(identity).toHaveValue('');
  expect(await name.evaluate(input => input.checkValidity())).toBe(false);
  await name.fill('marco');
  await expect(identity).toHaveValue('second');
  await name.fill('Hugo (fourth)');
  await expect(identity).toHaveValue('fourth');
  await name.fill('Hugo');
  await expect(identity).toHaveValue('');
});

for (const [timezoneId, localHour] of [['Asia/Jakarta', 10], ['America/New_York', 23]]) {
  test.describe(timezoneId, () => {
    test.use({ timezoneId });
    test('waktu statis dan aktivitas memakai zona browser dengan label zona', async ({ page }) => {
      await page.setContent('<html lang="id"><time data-local-time datetime="2026-09-10T03:24:28Z">10 Sep 2026 03:24 UTC</time></html>');
      await page.addScriptTag({ path: path.join(scripts, 'site.js') });
      const result = await page.locator('time').evaluate(time => ({
        text: time.textContent,
        live: window.cashflowpolyFormatDateTime(time.dateTime),
        expected: new Date(time.dateTime).toLocaleString('id-ID', {
          day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit',
          second: '2-digit', timeZoneName: 'short'
        }),
        hour: new Date(time.dateTime).getHours(),
        invalid: window.cashflowpolyFormatDateTime('not-a-date')
      }));
      expect(result.hour).toBe(localHour);
      expect(result.text).toBe(result.expected);
      expect(result.live).toBe(result.expected);
      expect(result.invalid).toBe('-');
    });
  });
}
