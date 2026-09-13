// Fungsi file: Menguji nama pemain ambigu dan format waktu browser tanpa mengubah data akun.
const { test, expect } = require('@playwright/test');
const path = require('node:path');
const scripts = path.resolve(__dirname, '../../../src/Cashflowpoly.Ui/wwwroot/js');

test('titik berdekatan pada histori panjang tetap dapat dihover sendiri', async ({ page }) => {
  const labels = Array.from({ length: 40 }, (_, i) => `Sesi ${i + 1}`);
  const payload = { labels, series: [{ name: 'Nilai', values: labels.map(() => 10) }] };
  await page.setContent('<article class="chart-card"><svg class="js-metric-line-chart" viewBox="0 0 300 240" style="width:300px;height:240px"></svg></article>');
  await page.locator('svg').evaluate((svg, payload) => { svg.dataset.chart = JSON.stringify(payload); }, payload);
  await page.addScriptTag({ path: path.join(scripts, 'player-detail-charts.js') });
  const points = page.locator('svg [tabindex="0"]');
  await expect(points).toHaveCount(40);
  for (let i = 0; i < 40; i++) {
    await points.nth(i).hover();
    await expect(page.locator('.js-chart-bar-insight-metric')).toHaveText(`Sesi ${i + 1}`);
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
