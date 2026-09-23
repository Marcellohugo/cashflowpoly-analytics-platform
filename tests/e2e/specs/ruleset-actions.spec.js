// Fungsi file: Memverifikasi alur aksi ruleset per versi dan penghapusan versi terakhir.
const { test, expect } = require('@playwright/test');

test('aksi per versi dan penghapusan versi terakhir tanpa pilihan massal', async ({ page }, testInfo) => {
  test.setTimeout(90000);
  await page.emulateMedia({ reducedMotion: 'reduce' });
  await page.goto('/auth/login');
  await page.getByLabel(/nama pengguna|username/i).fill('pratama');
  await page.getByLabel(/kata sandi|password/i).fill(process.env.E2E_PASSWORD || 'SeedLocal!2026');
  await page.getByRole('button', { name: /masuk|login/i }).click();
  await expect(page).toHaveURL(/\/$/);
  await page.goto('/rulesets');
  await expect(page.locator('main input[type="checkbox"]')).toHaveCount(0);
  await expect(page.locator('[data-ruleset-selected-count], [data-ruleset-bulk-delete]')).toHaveCount(0);
  await page.screenshot({ path: testInfo.outputPath('rulesets-index.png'), fullPage: true });

  const name = `E2E version actions ${Date.now()} ${testInfo.project.name}`;
  await page.getByRole('link', { name: /buat set aturan|create ruleset/i }).click();
  await page.locator('#ruleset-name').fill(name);
  await page.locator('#cfg-cash').fill('21');
  await page.getByRole('button', { name: /simpan set aturan|save ruleset/i }).click();
  await expect(page).toHaveURL(/\/rulesets$/);
  await page.locator('[data-ruleset-search]').fill(name);
  await page.locator('[data-ruleset-row]:visible').getByRole('link', { name: /lihat|view/i }).click();
  const detailUrl = page.url();
  const rows = page.locator('.ruleset-detail-section').first().locator('tbody tr');
  await expect(page.locator('.ruleset-detail-toolbar')).toHaveCount(0);
  await expect(rows).toHaveCount(1);
  const edit = rows.first().getByRole('link', { name: /edit aturan|edit rules/i });
  await expect(edit).toHaveAttribute('href', /\/edit\?version=1$/);
  await expect(rows.first().locator('td').last().getByRole('link', { name: /edit aturan|edit rules/i })).toBeVisible();
  await edit.click();
  await expect(page.locator('#cfg-cash')).toHaveValue('21');
  await page.locator('#cfg-cash').fill('22');
  await page.getByRole('button', { name: /simpan versi baru|save new version/i }).click();
  await expect(page).toHaveURL(detailUrl);
  await expect(rows).toHaveCount(2);
  const v1 = rows.filter({ has: page.getByRole('cell', { name: 'v1', exact: true }) });
  const v2 = rows.filter({ has: page.getByRole('cell', { name: 'v2', exact: true }) });
  await expect(v2.getByRole('button', { name: /hapus versi|delete version/i })).toHaveCount(0);
  page.once('dialog', dialog => dialog.accept());
  await v1.getByRole('button', { name: /hapus versi|delete version/i }).click();
  await expect(page).toHaveURL(detailUrl);
  await expect(rows).toHaveCount(1);
  await page.screenshot({ path: testInfo.outputPath('ruleset-last-version.png'), fullPage: true });

  page.once('dialog', async dialog => {
    expect(dialog.message()).toMatch(/versi terakhir|last version/i);
    await dialog.accept();
  });
  await rows.first().getByRole('button', { name: /hapus versi|delete version/i }).click();
  await expect(page).toHaveURL(/\/rulesets$/);
  await expect(page.locator('main')).toContainText(/Set aturan berhasil dihapus|Ruleset deleted successfully/);
  await page.locator('[data-ruleset-search]').fill(name);
  await expect(page.locator('[data-ruleset-row]:visible')).toHaveCount(0);
});
