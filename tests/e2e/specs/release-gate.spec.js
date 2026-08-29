// Fungsi file: Memverifikasi login, dashboard, accordion, mode, aksesibilitas, dan responsivitas UI rilis.
const { test: base, expect } = require("@playwright/test");

const username = process.env.E2E_USERNAME || "rina.kartika";
const password = process.env.E2E_PASSWORD || "SeedLocal!2026";
const baseUrl = process.env.E2E_BASE_URL || "http://localhost:5203";
const playerId = "90000000-0000-0000-0000-000000000011";
const beginnerSessionId = "91000000-0000-0000-0000-000000000001";
const advancedSessionId = "91000000-0000-0000-0000-000000000002";

async function login(page) {
  await page.goto(`${baseUrl}/auth/login`);
  await page.getByLabel(/nama pengguna|username/i).fill(username);
  await page.getByLabel(/kata sandi|password/i).fill(password);
  await page.getByRole("button", { name: /masuk|login/i }).click();
  await expect(page).toHaveURL(/\/$/);
}

const test = base.extend({
  workerStorageState: [async ({ browser }, use) => {
    const loginPage = await browser.newPage({ storageState: undefined });
    await login(loginPage);
    const storageState = await loginPage.context().storageState();
    await loginPage.close();
    await use(storageState);
  }, { scope: "worker" }],
  storageState: async ({ workerStorageState }, use) => {
    await use(workerStorageState);
  }
});

async function expectNoHorizontalOverflow(page) {
  const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
  expect(overflow).toBeLessThanOrEqual(1);
}

test("beranda hanya menampilkan angka data, bukan watermark atau persentase redundan", async ({ page }) => {
  await page.goto("/");
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

test("ringkasan pemain menyusun enam informasi termasuk status misi koleksi", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  const scorecard = page.locator("#player-statistics-summary .player-analysis-scorecard");
  const mission = scorecard.locator(".player-analysis-scorecard__mission");

  await expect(mission.locator("dt")).toHaveText("Misi Koleksi");
  await expect(mission.locator("dd")).toHaveText("Belum selesai");
  await expect(mission).toBeVisible();
  await expect(scorecard.locator(":scope > div")).toHaveCount(6);
  await expect(scorecard.locator("dt")).toHaveText([
    "Koin Tersisa",
    "Selisih Koin Masuk dan Keluar",
    "Koin Tersisa dibanding Koin Awal",
    "Total Poin Kebahagiaan",
    "Pemerataan Kartu Kebutuhan",
    "Misi Koleksi"
  ]);
  await expect(mission).toHaveCSS("grid-column", "auto");

  // Disable entrance/hover animation before comparing the actual grid layout.
  await page.emulateMedia({ reducedMotion: "reduce" });
  const boxes = await scorecard.locator(":scope > div").evaluateAll(items => items.map(item => {
    const box = item.getBoundingClientRect();
    return { x: box.x, y: box.y, width: box.width, height: box.height };
  }));
  const columns = page.viewportSize().width <= 560 ? 2 : 3;
  for (let index = 0; index < boxes.length; index += 1) {
    const rowStart = Math.floor(index / columns) * columns;
    expect(Math.abs(boxes[index].y - boxes[rowStart].y)).toBeLessThanOrEqual(1);
    expect(Math.abs(boxes[index].width - boxes[0].width)).toBeLessThanOrEqual(1);
    expect(Math.abs(boxes[index].height - boxes[rowStart].height)).toBeLessThanOrEqual(1);
    if (index >= columns) {
      expect(boxes[index].y).toBeGreaterThan(boxes[index - columns].y);
    }
  }
  await expectNoHorizontalOverflow(page);

  await page.locator("#player-analysis-atlas > summary").click();
  await expect(page.locator(".player-analysis-card--action-efficiency")).toBeVisible();

  await page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`);
  await expect(mission).toBeVisible();
  await expect(mission.locator("dt")).toHaveText("Misi Koleksi");
  await expect(scorecard.locator(":scope > div")).toHaveCount(6);
  await expect(scorecard.locator("dt").last()).toHaveText("Misi Koleksi");
  await expectNoHorizontalOverflow(page);
});

test("koin dan keuangan menampilkan satu tabel transaksi komprehensif", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  const evidence = page.locator("#player-evidence-library");
  if ((await evidence.getAttribute("open")) === null) {
    await evidence.locator(":scope > summary").click();
  }

  const coinDomain = page.locator("details.player-evidence-domain").first();
  if ((await coinDomain.getAttribute("open")) === null) {
    await coinDomain.locator(":scope > summary").click();
  }

  await expect(page.getByText("Perjalanan Koin dan Peristiwa", { exact: true })).toHaveCount(0);
  await expect(coinDomain.getByRole("heading", { name: "Riwayat Transaksi" })).toBeVisible();
  await expect(coinDomain.getByRole("columnheader", { name: "Koin Masuk (koin)" })).toBeVisible();
  await expect(coinDomain.getByRole("columnheader", { name: "Koin Keluar (koin)" })).toBeVisible();
  await expect(coinDomain.getByRole("columnheader", { name: "Perubahan Koin (koin)" })).toBeVisible();
  await expect(coinDomain.getByRole("columnheader", { name: "Saldo setelah Kejadian (koin)" })).toBeVisible();

  const headings = await coinDomain.locator(".player-metric-card h5").allTextContents();
  expect(headings).toEqual([
    "Koin Awal",
    "Koin Tersisa",
    "Riwayat Transaksi",
    "Pinjaman Pertama Tercatat",
    "Aktivitas Terakhir Tercatat"
  ]);

  const cards = coinDomain.locator(".player-metric-card");
  await expect(cards).toHaveCount(5);
  const transactionCard = cards.nth(2);
  const firstTransactionDay = transactionCard.locator("tbody tr").first().locator("td").first();
  expect(await transactionCard.evaluate(card => getComputedStyle(card.parentElement).borderTopWidth)).toBe("0px");
  expect(await firstTransactionDay.evaluate(cell => getComputedStyle(cell).whiteSpace)).toBe("nowrap");
  expect(await firstTransactionDay.evaluate(cell => cell.scrollWidth <= cell.clientWidth)).toBeTruthy();
  const layout = await cards.evaluateAll(items => items.map(item => {
    const box = item.getBoundingClientRect();
    return {
      overview: item.classList.contains("player-metric-card--coin-overview"),
      series: item.classList.contains("player-metric-card--series"),
      width: box.width,
      y: box.y
    };
  }));
  const gridWidth = await coinDomain.locator(".player-metric-card-grid").evaluate(
    grid => grid.getBoundingClientRect().width
  );

  expect(layout.slice(0, 2).every(item => item.overview)).toBeTruthy();
  expect(layout[2].series).toBeTruthy();
  expect(layout.slice(3).every(item => !item.series)).toBeTruthy();
  if (page.viewportSize().width >= 768) {
    expect(layout[0].y).toBe(layout[1].y);
    expect(layout.slice(0, 2).every(item => item.width > gridWidth * 0.45)).toBeTruthy();
    expect(layout[2].width).toBeGreaterThan(gridWidth * 0.9);
    expect(layout[3].y).toBe(layout[4].y);
  } else {
    expect(layout[1].y).toBeGreaterThan(layout[0].y);
    expect(layout.every(item => item.width > gridWidth * 0.9)).toBeTruthy();
  }
  expect(layout[2].y).toBeGreaterThan(layout[1].y);
  expect(layout[3].y).toBeGreaterThan(layout[2].y);
  await expectNoHorizontalOverflow(page);
});

test("kartu bahan ringkas dengan jumlah bahan untuk setiap pesanan", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  await page.locator("#player-evidence-library > summary").click();
  const ingredients = page.locator("details.player-evidence-domain").nth(1);
  await ingredients.locator(":scope > summary").click();

  const cards = ingredients.locator(".player-metric-card");
  await expect(cards.locator("h5")).toHaveText([
    "Bahan Terkumpul",
    "Total Bahan Tersisa",
    "Bahan Digunakan per Pesanan",
    "Total Bahan Digunakan",
    "Rata-rata Bahan per Pesanan",
    "Total Biaya Pembelian Bahan"
  ]);
  await expect(cards.nth(1).locator(".player-metric-card__value strong")).toHaveText("2");
  await expect(cards.nth(1)).toContainText("Bahan yang sudah digunakan untuk pesanan atau dibuang tidak termasuk.");
  await expect(cards.nth(2).locator("th")).toHaveText([
    "Pesanan ke-", "Jumlah Kartu Bahan yang Digunakan"
  ]);
  await expect(cards.nth(2).locator("tbody tr")).toHaveText([
    "1 2", "2 3", "3 3", "4 2", "5 3"
  ], { useInnerText: true });
  await expect(cards.nth(2)).toContainText("Angka pesanan bukan nomor pada kartu pesanan.");
  const layout = await cards.evaluateAll(items => items.map(item => {
    const box = item.getBoundingClientRect();
    return { width: box.width, y: box.y };
  }));
  const gridWidth = await ingredients.locator(".player-metric-card-grid").evaluate(
    grid => grid.clientWidth - parseFloat(getComputedStyle(grid).paddingLeft) - parseFloat(getComputedStyle(grid).paddingRight)
  );
  expect(layout[2].width).toBeGreaterThan(gridWidth * 0.9);
  expect(layout[2].y).toBeGreaterThan(layout[1].y);
  expect(layout[3].y).toBeGreaterThan(layout[2].y);
  await expectNoHorizontalOverflow(page);
});

test("rincian risiko tersusun berurutan tanpa kartu mengapit tabel", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  const evidence = page.locator("#player-evidence-library");
  if ((await evidence.getAttribute("open")) === null) {
    await evidence.locator(":scope > summary").click();
  }

  const riskDomain = page.locator("details.player-evidence-domain").nth(7);
  if ((await riskDomain.getAttribute("open")) === null) {
    await riskDomain.locator(":scope > summary").click();
  }

  const cards = riskDomain.locator(".player-metric-card");
  await expect(cards).toHaveCount(6);
  await expect(cards.locator("h5")).toHaveText([
    "Kartu Risiko Kehidupan yang Muncul",
    "Nominal Dampak Koin per Kartu Risiko",
    "Total Nominal Dampak Koin Kartu Risiko",
    "Risiko yang Ditanggung Asuransi",
    "Total Premi Asuransi Dibayar",
    "Penggunaan Tindakan Darurat"
  ]);
  await expect(cards.nth(1).locator("th").first()).toHaveText("No.");

  const layout = await cards.evaluateAll(items => items.map(item => {
    const box = item.getBoundingClientRect();
    return { width: box.width, y: box.y };
  }));
  const gridWidth = await riskDomain.locator(".player-metric-card-grid").evaluate(
    grid => grid.getBoundingClientRect().width
  );

  expect(layout.slice(0, 3).every(item => item.width > gridWidth * 0.9)).toBeTruthy();
  expect(layout[1].y).toBeGreaterThan(layout[0].y);
  expect(layout[2].y).toBeGreaterThan(layout[1].y);
  expect(layout[3].y).toBeGreaterThan(layout[2].y);
  await expectNoHorizontalOverflow(page);
});

test("rincian emas tersusun menjadi empat ringkasan dua harga dan tiga hasil", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  const evidence = page.locator("#player-evidence-library");
  if ((await evidence.getAttribute("open")) === null) {
    await evidence.locator(":scope > summary").click();
  }

  const goldDomain = page.locator("details.player-evidence-domain").nth(5);
  if ((await goldDomain.getAttribute("open")) === null) {
    await goldDomain.locator(":scope > summary").click();
  }

  const cards = goldDomain.locator(".player-metric-card");
  await expect(cards).toHaveCount(9);
  await expect(cards.locator("h5")).toHaveText([
    "Kartu Emas Awal",
    "Kartu Emas Dibeli selama Permainan",
    "Kartu Emas Dijual",
    "Total Kartu Emas Tersisa",
    "Harga Emas Saat Beli",
    "Harga Emas Saat Jual",
    "Total Biaya Pembelian Emas",
    "Pendapatan Penjualan Emas",
    "Selisih Arus Kas Transaksi Emas"
  ]);

  const layout = await cards.evaluateAll(items => items.map(item => {
    const box = item.getBoundingClientRect();
    return { series: item.classList.contains("player-metric-card--series"), width: box.width, y: box.y };
  }));
  const gridContentWidth = await goldDomain.locator(".player-metric-card-grid").evaluate(grid => {
    const style = getComputedStyle(grid);
    return grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
  });

  expect(layout.slice(0, 4).every(item => !item.series)).toBeTruthy();
  expect(layout.slice(4, 6).every(item => item.series)).toBeTruthy();
  expect(layout.slice(6).every(item => !item.series)).toBeTruthy();
  if (page.viewportSize().width >= 768) {
    expect(layout[0].y).toBe(layout[1].y);
    expect(layout[2].y).toBe(layout[3].y);
    expect(layout[4].y).toBe(layout[5].y);
    expect(layout[6].y).toBe(layout[7].y);
    expect(layout[7].y).toBe(layout[8].y);
    expect(layout.slice(0, 6).every(item => item.width > gridContentWidth * 0.45)).toBeTruthy();
  } else {
    expect(
      layout.every(item => item.width > gridContentWidth * 0.98),
      JSON.stringify({ gridContentWidth, widths: layout.map(item => item.width) })
    ).toBeTruthy();
  }
  expect(layout[4].y).toBeGreaterThan(layout[3].y);
  expect(layout[6].y).toBeGreaterThan(layout[5].y);
  await expectNoHorizontalOverflow(page);
});

test("rincian donasi menggabungkan jumlah dan peringkat dalam satu tabel", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  const evidence = page.locator("#player-evidence-library");
  if ((await evidence.getAttribute("open")) === null) {
    await evidence.locator(":scope > summary").click();
  }

  const donationDomain = page.locator("details.player-evidence-domain").nth(4);
  if ((await donationDomain.getAttribute("open")) === null) {
    await donationDomain.locator(":scope > summary").click();
  }

  const cards = donationDomain.locator(".player-metric-card");
  await expect(cards).toHaveCount(4);
  await expect(cards.locator("h5")).toHaveText([
    "Donasi Setiap Jumat",
    "Total Koin Donasi",
    "Kartu Juara Donasi Diperoleh",
    "Poin Kebahagiaan dari Donasi"
  ]);
  const table = donationDomain.locator("table");
  await expect(table).toHaveCount(1);
  await expect(table.locator("th")).toHaveText(["Hari", /Jumlah Donasi\s*\(koin\)/, "Peringkat"]);
  await expect(table.locator("tbody tr")).toHaveCount(3);
  for (const [index, values] of [[0, ["5", "1", "4"]], [1, ["12", "4", "2"]], [2, ["19", "1", "4"]]]) {
    await expect(table.locator("tbody tr").nth(index).locator("td")).toHaveText(values);
  }

  const layout = await cards.evaluateAll(items => items.map(item => {
    const box = item.getBoundingClientRect();
    return { series: item.classList.contains("player-metric-card--series"), width: box.width, y: box.y };
  }));
  const gridContentWidth = await donationDomain.locator(".player-metric-card-grid").evaluate(grid => {
    const style = getComputedStyle(grid);
    return grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
  });

  expect(layout[0].series).toBeTruthy();
  expect(layout[0].width).toBeGreaterThan(gridContentWidth * 0.98);
  expect(layout.slice(1).every(item => !item.series)).toBeTruthy();
  if (page.viewportSize().width >= 768) {
    expect(layout[1].y).toBe(layout[2].y);
    expect(layout[2].y).toBe(layout[3].y);
    expect(layout.slice(1).every(item => item.width > gridContentWidth * 0.3)).toBeTruthy();
  } else {
    expect(layout.every(item => item.width > gridContentWidth * 0.98)).toBeTruthy();
  }
  expect(layout[1].y).toBeGreaterThan(layout[0].y);
  await expectNoHorizontalOverflow(page);
});

test("kartu kebutuhan tetap menjelaskan kepemilikan tanpa indikator 70 persen", async ({ page }) => {
  for (const sessionId of [advancedSessionId, beginnerSessionId]) {
    await page.goto(`/sessions/${sessionId}/players/${playerId}`);
    const evidence = page.locator("#player-evidence-library");
    if ((await evidence.getAttribute("open")) === null) {
      await evidence.locator(":scope > summary").click();
    }
    const needs = evidence.locator("details.player-evidence-domain").nth(3);
    await needs.locator(":scope > summary").click();
    await expect(needs.locator(".player-metric-card h5").filter({ hasText: /70%/ })).toHaveCount(0);
    const owned = needs.locator(".player-metric-card").filter({
      has: page.getByRole("heading", { name: "Kartu Kebutuhan yang Masih Dimiliki", exact: true })
    });
    await expect(owned).toBeVisible();
    await expect(owned.locator(".player-metric-card__explanation")).toContainText("Jumlah kartu kebutuhan yang masih dimiliki setelah aktivitas terakhir.");
    if (sessionId === advancedSessionId) {
      await expect(owned.locator(".player-metric-card__value strong")).toHaveText("1");
      const purchased = needs.locator(".player-metric-card").filter({
        has: page.getByRole("heading", { name: "Kartu Kebutuhan Dibeli", exact: true })
      });
      await expect(purchased.locator(".player-metric-card__value strong")).toHaveText("1");
    } else {
      await expect(owned.locator(".player-metric-card__explanation")).not.toContainText("Tindakan Darurat");
    }
    const analysis = page.locator("#player-analysis-atlas");
    if ((await analysis.getAttribute("open")) === null) {
      await analysis.locator(":scope > summary").click();
    }
    await expect(analysis.locator(".player-analysis-card--fulfillment-diversity")).toContainText("Pemerataan Kartu Kebutuhan");
    await expectNoHorizontalOverflow(page);
  }
});

test("target finansial diringkas menjadi hasil target tabungan dan sisa pinjaman", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  const evidence = page.locator("#player-evidence-library");
  if ((await evidence.getAttribute("open")) === null) {
    await evidence.locator(":scope > summary").click();
  }
  const goals = evidence.locator("details.player-evidence-domain").filter({
    has: page.locator("summary strong", { hasText: /^Target Finansial$/ })
  });
  await goals.locator(":scope > summary").click();
  const cards = goals.locator(".player-metric-card");
  await expect(cards).toHaveCount(3);
  await expect(cards.locator("h5")).toHaveText([
    "Target Finansial yang Selesai", "Koin dalam Tabungan", "Sisa Pinjaman"
  ]);
  await expect(cards.locator(".player-metric-card__value strong")).toHaveText(["1", "0", "0"]);
  await expect(cards.locator(".player-metric-card__value small")).toHaveText(["target", "koin", "koin"]);
  await expect(cards.first().locator(".player-metric-card__explanation")).toHaveText("Dari 1 target yang mulai didanai.");
  await expect(goals.locator("table, .player-evidence-domain__guide")).toHaveCount(0);

  const boxes = await cards.evaluateAll(items => items.map(item => {
    const box = item.getBoundingClientRect();
    return { width: box.width, y: box.y };
  }));
  const width = await goals.locator(".player-metric-card-grid").evaluate(grid => {
    const style = getComputedStyle(grid);
    return grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
  });
  if (page.viewportSize().width >= 768) {
    expect(boxes.every(box => box.y === boxes[0].y && box.width > width * 0.3)).toBeTruthy();
  } else {
    expect(boxes.every(box => box.width > width * 0.98)).toBeTruthy();
    expect(boxes[1].y).toBeGreaterThan(boxes[0].y);
    expect(boxes[2].y).toBeGreaterThan(boxes[1].y);
  }
  await expectNoHorizontalOverflow(page);

  await page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`);
  await expect(page.locator("details.player-evidence-domain > summary strong").filter({ hasText: /^Target Finansial$/ })).toHaveCount(0);
});

test("istilah Poin Kebahagiaan konsisten pada label satuan rumus dan panduan", async ({ page }) => {
  for (const sessionId of [advancedSessionId, beginnerSessionId]) {
    await page.goto(`/sessions/${sessionId}/players/${playerId}`);
    const analysis = page.locator("#player-analysis-atlas");
    if ((await analysis.getAttribute("open")) === null) {
      await analysis.locator(":scope > summary").click();
    }
    const happiness = analysis.locator(".player-analysis-card--happiness-portfolio");
    await happiness.locator(".player-analysis-card__method > summary").click();
    await expect(happiness).toContainText("Poin Kebahagiaan Kartu Kebutuhan");
    await expect(happiness).toContainText("Poin Kebahagiaan Dana Pensiun");
    const units = happiness.locator(".player-analysis-card__metric small");
    expect(await units.count()).toBeGreaterThan(0);
    for (const unit of await units.all()) {
      await expect(unit).toHaveText("poin kebahagiaan");
    }

    const evidence = page.locator("#player-evidence-library");
    if ((await evidence.getAttribute("open")) === null) {
      await evidence.locator(":scope > summary").click();
    }
    const pension = evidence.locator("details.player-evidence-domain").nth(6);
    await pension.locator(":scope > summary").click();
    await expect(pension).toContainText("Poin Kebahagiaan Dana Pensiun");
    const content = await page.locator("#content").innerText();
    expect(content.match(/\bpoin\b(?!\s+kebahagiaan\b)|\bpoin\s+kebahagiaan\s+kebahagiaan\b/gi) || []).toEqual([]);
    await expectNoHorizontalOverflow(page);
  }

  for (const path of ["/", `/sessions/${advancedSessionId}`, "/rulebook"]) {
    await page.goto(path);
    const content = await page.locator("#content").innerText();
    expect(content.match(/\bpoin\b(?!\s+kebahagiaan\b)/gi) || []).toEqual([]);
    await expectNoHorizontalOverflow(page);
  }
});

test("peringkat dana pensiun ditampilkan sebagai posisi ke-4", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  const evidence = page.locator("#player-evidence-library");
  if ((await evidence.getAttribute("open")) === null) {
    await evidence.locator(":scope > summary").click();
  }

  const pension = evidence.locator("details.player-evidence-domain").nth(6);
  await pension.locator(":scope > summary").click();
  const rankCard = pension.locator(".player-metric-card").filter({
    has: page.getByRole("heading", { name: "Peringkat Dana Pensiun", exact: true })
  });
  await expect(rankCard.locator(".player-metric-card__value")).toHaveText("ke-4");
  await expect(rankCard.locator(".player-metric-card__value small")).toHaveCount(0);
  await expectNoHorizontalOverflow(page);
});

test("rincian pesanan tersusun menjadi hasil tabel dan dua ringkasan", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  const evidence = page.locator("#player-evidence-library");
  if ((await evidence.getAttribute("open")) === null) {
    await evidence.locator(":scope > summary").click();
  }

  const orderDomain = page.locator("details.player-evidence-domain").nth(2);
  if ((await orderDomain.getAttribute("open")) === null) {
    await orderDomain.locator(":scope > summary").click();
  }

  const cards = orderDomain.locator(".player-metric-card");
  await expect(cards).toHaveCount(4);
  await expect(cards.locator("h5")).toHaveText([
    "Pesanan Selesai",
    "Pendapatan per Pesanan Makanan",
    "Total Pendapatan Pesanan",
    "Rata-rata Pesanan per Hari dengan Aksi Utama"
  ]);
  await expect(cards.nth(1).locator("th")).toHaveText([
    "Pesanan ke-", "Pendapatan yang Dihasilkan (koin)"
  ]);
  await expect(cards.nth(1).locator("tbody tr")).toHaveText([
    "1 13", "2 22", "3 22", "4 14", "5 20"
  ], { useInnerText: true });
  await expect(cards.nth(1)).toContainText("sebelum dikurangi biaya bahan");
  await expect(cards.nth(1)).toContainText("pesanan pertama yang diselesaikan pemain, bukan nomor kartu pesanan");

  const layout = await cards.evaluateAll(items => items.map(item => {
    const box = item.getBoundingClientRect();
    return { width: box.width, y: box.y };
  }));
  const gridContentWidth = await orderDomain.locator(".player-metric-card-grid").evaluate(grid => {
    const style = getComputedStyle(grid);
    return grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
  });

  expect(layout[1].y).toBeGreaterThan(layout[0].y);
  expect(layout[2].y).toBeGreaterThan(layout[1].y);
  if (page.viewportSize().width >= 768) {
    expect(layout[0].width).toBeGreaterThan(gridContentWidth * 0.98);
    expect(layout[1].width).toBeGreaterThan(gridContentWidth * 0.98);
    expect(layout[2].y).toBe(layout[3].y);
    expect(layout.slice(2).every(item => item.width > gridContentWidth * 0.45)).toBeTruthy();
  } else {
    expect(layout.every(item => item.width > gridContentWidth * 0.98)).toBeTruthy();
    expect(layout[3].y).toBeGreaterThan(layout[2].y);
  }
  await expectNoHorizontalOverflow(page);
});

test("halaman pemain tidak menampilkan riwayat aktivitas keuangan terpisah", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  await expect(page.locator("#player-transaction-history")).toHaveCount(0);
  await expect(page.getByText("Riwayat Aktivitas Keuangan", { exact: true })).toHaveCount(0);
  await expect(page.locator("#player-statistics-dashboard")).toBeVisible();
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
