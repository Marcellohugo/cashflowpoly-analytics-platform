// Fungsi file: Memverifikasi login, dashboard, accordion, mode, aksesibilitas, dan responsivitas UI rilis.
const { test, expect } = require("@playwright/test");

const username = process.env.E2E_USERNAME || "rina.kartika";
const password = process.env.E2E_PASSWORD || "SeedLocal!2026";
const playerId = "90000000-0000-0000-0000-000000000011";
const beginnerSessionId = "91000000-0000-0000-0000-000000000001";
const advancedSessionId = "91000000-0000-0000-0000-000000000002";

async function login(page) {
  await page.goto("/auth/login");
  await page.getByLabel(/nama pengguna|username/i).fill(username);
  await page.getByLabel(/kata sandi|password/i).fill(password);
  await page.getByRole("button", { name: /masuk|login/i }).click();
  await expect(page).toHaveURL(/\/$/);
}

async function expectNoHorizontalOverflow(page) {
  const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
  expect(overflow).toBeLessThanOrEqual(1);
}

test.beforeEach(async ({ page }) => {
  await login(page);
});

test("beranda hanya menampilkan angka data, bukan watermark atau persentase redundan", async ({ page }) => {
  await expect(page.locator(".home-stat-mark")).toHaveCount(0);
  await expect(page.locator("#home-active-percentage")).toHaveCount(0);
  await expect(page.locator(".home-live-summary")).toHaveCount(0);
  await expect(page.locator("#home-active-sessions")).toHaveText(/^\d+$/);
  await expectNoHorizontalOverflow(page);
});

test("tiga bagian pemain adalah accordion konsisten dan menyimpan pilihan", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  const summary = page.locator("#player-statistics-summary");
  const analysis = page.locator("#player-analysis-atlas");
  const evidence = page.locator("#player-evidence-library");
  await expect(summary).toHaveAttribute("open", "");
  await expect(analysis).not.toHaveAttribute("open", "");
  await expect(evidence).not.toHaveAttribute("open", "");

  const summaries = page.locator("[data-player-section-accordion] > summary");
  await expect(summaries).toHaveCount(3);
  for (let index = 0; index < 3; index += 1) {
    await expect(summaries.nth(index)).toHaveCSS("cursor", "pointer");
  }

  await analysis.locator(":scope > summary").focus();
  await page.keyboard.press("Enter");
  await expect(analysis).toHaveAttribute("open", "");
  await page.reload();
  await expect(analysis).toHaveAttribute("open", "");
  await expectNoHorizontalOverflow(page);
});

test("mode pemula tidak merender kelompok atau metrik khusus mahir", async ({ page }) => {
  await page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`);
  await page.locator("#player-analysis-atlas > summary").click();
  await page.locator("#player-evidence-library > summary").click();

  await expect(page.locator(".player-analysis-card__mode")).toHaveCount(0);
  await expect(page.locator(".player-evidence-domain__mode")).toHaveCount(0);
  await expect(page.locator(".player-metric-series__outside-quota")).toHaveCount(0);
  await expectNoHorizontalOverflow(page);
});

test("rincian sesi tidak menampilkan kartu pelanggaran aturan", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}`);
  await expect(page.getByText(/pelanggaran aturan|rule violations/i)).toHaveCount(0);
  await expect(page.locator('.work-calendar-cell[data-day="25"] .work-calendar-cell__number')).toHaveText("25");
  await expect(page.locator('.work-calendar-cell--finish[data-day="26"]')).toContainText(/selesai|finish/i);
  await expectNoHorizontalOverflow(page);
});

test("daftar pemain tetap terbaca tanpa overflow pada semua ukuran utama", async ({ page }) => {
  const viewports = [
    { width: 320, height: 800, mobile: true },
    { width: 768, height: 1024, mobile: true },
    { width: 1024, height: 768, mobile: false },
    { width: 1440, height: 900, mobile: false }
  ];

  for (const viewport of viewports) {
    await page.setViewportSize({ width: viewport.width, height: viewport.height });
    await page.goto(`/sessions/${advancedSessionId}`);

    const desktopTable = page.locator(".happiness-score-desktop");
    const mobileCards = page.locator(".happiness-score-mobile");
    if (viewport.mobile) {
      await expect(desktopTable).toBeHidden();
      await expect(mobileCards).toBeVisible();
    } else {
      await expect(desktopTable).toBeVisible();
      await expect(mobileCards).toBeHidden();
      const playerColumnColors = await page.locator(".happiness-score-table tbody tr:first-child td").evaluateAll(
        cells => cells.map(cell => getComputedStyle(cell).backgroundColor)
      );
      expect(new Set(playerColumnColors).size).toBe(4);
    }
    await expect(page.locator(".happiness-score-mobile-card")).toHaveCount(4);
    const playerCardColors = await page.locator(".happiness-score-mobile-card").evaluateAll(
      cards => cards.map(card => getComputedStyle(card).backgroundColor)
    );
    expect(new Set(playerCardColors).size).toBe(4);
    await expectNoHorizontalOverflow(page);
  }
});
