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

test('grafik ponsel membatasi lima sesi dengan sumbu tetap saat digeser dan diubah ukuran', async ({ page }) => {
  const errors = [];
  page.on('pageerror', error => errors.push(error.message));
  await page.setViewportSize({ width: 412, height: 915 });
  const chart = id => `<article id="${id}" class="chart-card statistics-chart">
    <h3>${id}</h3><div class="statistics-chart-frame">
      <div class="statistics-chart-plot"><svg class="js-metric-line-chart" data-chart-responsive viewBox="0 0 840 340"></svg></div>
      <div class="statistics-chart-axis" aria-hidden="true"></div>
      <p class="statistics-chart-scroll-hint">Geser untuk melihat sesi lain.</p>
    </div></article>`;
  await page.setContent(`<meta name="viewport" content="width=device-width, initial-scale=1">
    <button data-statistics-panel="history">Histori</button><button data-statistics-panel="other">Lainnya</button>
    <section id="history" class="statistics-chapter" data-statistics-section>${chart('long')}${chart('single')}${chart('five')}</section>
    <section id="other" class="statistics-chapter" data-statistics-section hidden>${chart('hidden-chart')}</section>`);
  await page.addStyleTag({ path: path.join(scripts, '../css/player-statistics.css') });
  await page.locator('svg').evaluateAll(nodes => nodes.forEach(svg => {
    const id = svg.closest('article').id;
    const values = id === 'single' ? [10] : id === 'five' ? [10, 20, 30, 40, 50] : [10, 20, 30, 40, 50, -100, 1000, 60];
    svg.dataset.chart = JSON.stringify({ labels: values.map((_, index) => `Sesi ${index + 1}`), series: [{ name: 'Nilai', values }] });
  }));
  await page.addScriptTag({ path: path.join(scripts, 'player-detail-charts.js') });
  await page.addScriptTag({ path: path.join(scripts, 'player-statistics.js') });
  const state = id => page.locator('#' + id).evaluate(card => {
    const plot = card.querySelector('.statistics-chart-plot');
    const svg = card.querySelector('svg');
    const axis = card.querySelector('.statistics-chart-axis');
    const bounds = plot.getBoundingClientRect();
    const axisBounds = axis.getBoundingClientRect();
    const points = [...svg.querySelectorAll('[tabindex="0"]')];
    return {
      visible: points.filter(point => {
        const box = point.getBoundingClientRect();
        const center = box.x + box.width / 2;
        return center >= Math.max(bounds.left, axisBounds.right) && center <= bounds.right;
      }).length,
      pointY: points.map(point => point.getAttribute('cy')),
      ticks: [...axis.querySelectorAll('span')].map(label => label.textContent),
      axisLeft: axisBounds.left,
      scrollWidth: plot.scrollWidth,
      viewportWidth: plot.clientWidth,
      svgWidth: svg.getBoundingClientRect().width,
      viewBoxWidth: svg.viewBox.baseVal.width,
      bodyOverflow: document.documentElement.scrollWidth > innerWidth + 1
    };
  });
  const checkScrollable = async id => {
    const card = page.locator('#' + id);
    const plot = card.locator('.statistics-chart-plot');
    await expect(card.locator('.statistics-chart-axis')).toBeVisible();
    await expect(card.locator('.statistics-chart-scroll-hint')).toBeVisible();
    await expect.poll(async () => Math.abs((await state(id)).viewBoxWidth - (await state(id)).svgWidth)).toBeLessThan(2);
    const before = await state(id);
    expect(before.scrollWidth).toBeGreaterThan(before.viewportWidth);
    expect(before.ticks).toEqual(['1000', '725', '450', '175', '-100']);
    for (const fraction of [0, .25, .5, .75, 1]) {
      await plot.evaluate((element, fraction) => { element.scrollLeft = (element.scrollWidth - element.clientWidth) * fraction; }, fraction);
      const after = await state(id);
      expect(after.visible).toBeGreaterThan(0);
      expect(after.visible).toBeLessThanOrEqual(5);
      expect(after.pointY).toEqual(before.pointY);
      expect(after.ticks).toEqual(before.ticks);
      expect(after.axisLeft).toBe(before.axisLeft);
      expect(after.bodyOverflow).toBe(false);
    }
    await card.locator('svg [tabindex="0"]').last().click();
    await expect(card.locator('.chart-detail-dialog')).toBeVisible();
    await expect(card.locator('.js-chart-bar-insight-metric')).toHaveText('Sesi 8');
    await expect(card.locator('.js-chart-bar-insight-points')).toHaveText('60');
    await card.locator('.chart-detail-close').click();
  };
  await checkScrollable('long');
  for (const [id, count] of [['single', 1], ['five', 5]]) {
    await expect(page.locator('#' + id + ' svg [tabindex="0"]')).toHaveCount(count);
    await expect(page.locator('#' + id + ' .statistics-chart-axis')).toBeHidden();
    await expect(page.locator('#' + id + ' .statistics-chart-scroll-hint')).toBeHidden();
    const actual = await state(id);
    expect(actual.visible).toBe(count);
    expect(actual.scrollWidth).toBeLessThanOrEqual(actual.viewportWidth + 1);
  }
  // A never-opened panel must draw at its current width; a hidden old panel must redraw too.
  await page.setViewportSize({ width: 320, height: 740 });
  await page.locator('[data-statistics-panel="other"]').click();
  await checkScrollable('hidden-chart');
  await page.setViewportSize({ width: 956, height: 440 });
  await page.locator('[data-statistics-panel="history"]').click();
  await checkScrollable('long');
  await page.setViewportSize({ width: 1280, height: 800 });
  await expect(page.locator('#long .statistics-chart-axis')).toBeHidden();
  await expect(page.locator('#long .statistics-chart-scroll-hint')).toBeHidden();
  await expect.poll(async () => (await state('long')).visible).toBe(8);
  const desktop = await state('long');
  expect(desktop.scrollWidth).toBeLessThanOrEqual(desktop.viewportWidth + 1);
  expect(desktop.bodyOverflow).toBe(false);
  expect(errors).toEqual([]);
});

test('label sesi ke-11 mempertahankan tanda hubung dan tidak terpotong saat digulir ke ujung kanan', async ({ page }) => {
  const labels = Array.from({ length: 11 }, (_, i) => `Sesi ke-${i + 1}`);
  const values = [10, 20, 15, 30, 25, 35, 40, 30, 45, 50, 55];
  const payload = { labels, series: [{ name: 'Nilai', values }] };
  await page.setViewportSize({ width: 412, height: 915 });
  await page.setContent(`<div class="statistics-chart-frame">
    <div class="statistics-chart-plot"><svg class="js-metric-line-chart" data-chart-responsive viewBox="0 0 840 340"></svg></div>
    <div class="statistics-chart-axis"></div>
  </div>`);
  await page.addStyleTag({ path: path.join(scripts, '../css/player-statistics.css') });
  await page.locator('svg').evaluate((svg, payload) => { svg.dataset.chart = JSON.stringify(payload); }, payload);
  await page.addScriptTag({ path: path.join(scripts, 'player-detail-charts.js') });

  const plot = page.locator('.statistics-chart-plot');
  await plot.evaluate(el => { el.scrollLeft = el.scrollWidth - el.clientWidth; });

  const labelTexts = await page.locator('svg text').allTextContents();
  expect(labelTexts).toContain('Sesi ke-11');
  expect(labelTexts).not.toContain('Sesi ke 11');

  const isClipped = await page.evaluate(() => {
    const texts = Array.from(document.querySelectorAll('svg text'));
    const lastText = texts.find(t => t.textContent === 'Sesi ke-11');
    if (!lastText) return true;
    const textRect = lastText.getBoundingClientRect();
    const plotRect = document.querySelector('.statistics-chart-plot').getBoundingClientRect();
    return textRect.right > plotRect.right + 1;
  });
  expect(isClipped).toBe(false);
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
