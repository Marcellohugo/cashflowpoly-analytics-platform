// Fungsi file: Memverifikasi login, dashboard, accordion, mode, aksesibilitas, dan responsivitas UI rilis.
// Penjelasan: Melakukan operasi dengan memuat modul CommonJS `"@playwright/test"` agar fungsi atau konfigurasi modul dapat digunakan.
const { test: base, expect } = require("@playwright/test");

// Penjelasan: Menyimpan `username` dengan membaca nilai `process.env.E2E_USERNAME || "rina.kartika"` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
const username = process.env.E2E_USERNAME || "rina.kartika";
// Penjelasan: Menyimpan `password` dengan membaca nilai `process.env.E2E_PASSWORD || "SeedLocal!2026"` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
const password = process.env.E2E_PASSWORD || "SeedLocal!2026";
// Penjelasan: Menyimpan `baseUrl` dengan menghitung ekspresi `process.env.E2E_BASE_URL || "http://localhost:5203"` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
const baseUrl = process.env.E2E_BASE_URL || "http://localhost:5203";
// Penjelasan: Menyimpan `playerId` dengan menggunakan literal `"90000000-0000-0000-0000-000000000011"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
const playerId = "90000000-0000-0000-0000-000000000011";
// Penjelasan: Menyimpan `beginnerSessionId` dengan menggunakan literal `"91000000-0000-0000-0000-000000000001"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
const beginnerSessionId = "91000000-0000-0000-0000-000000000001";
// Penjelasan: Menyimpan `advancedSessionId` dengan menggunakan literal `"91000000-0000-0000-0000-000000000002"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
const advancedSessionId = "91000000-0000-0000-0000-000000000002";


// Penjelasan: Melakukan operasi dengan memanggil `async function login(page) {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
async function login(page) {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`${baseUrl}/auth/login`)` selesai sebelum memakai hasilnya.
  await page.goto(`${baseUrl}/auth/login`);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.getByLabel(/nama pengguna|username/i).fill(username)` selesai sebelum memakai hasilnya.
  await page.getByLabel(/nama pengguna|username/i).fill(username);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.getByLabel(/kata sandi|password/i).fill(password)` selesai sebelum memakai hasilnya.
  await page.getByLabel(/kata sandi|password/i).fill(password);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.getByRole("button", { name: /masuk|login/i }).click()` selesai sebelum memakai hasilnya.
  await page.getByRole("button", { name: /masuk|login/i }).click();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page).toHaveURL(/\/$/)` selesai sebelum memakai hasilnya.
  await expect(page).toHaveURL(/\/$/);
// Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
}

// Penjelasan: Menyimpan `test` dengan memanggil `base.extend({` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
const test = base.extend({
  // Penjelasan: Mengisi properti `workerStorageState` pada objek atau konfigurasi dengan membaca nilai `[async ({ browser }, use) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
  workerStorageState: [async ({ browser }, use) => {
    // Penjelasan: Menyimpan `loginPage` dengan menunggu operasi asinkron `browser.newPage({ storageState: undefined })` selesai sebelum memakai hasilnya.
    const loginPage = await browser.newPage({ storageState: undefined });
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `login(loginPage)` selesai sebelum memakai hasilnya.
    await login(loginPage);
    // Penjelasan: Menyimpan `storageState` dengan menunggu operasi asinkron `loginPage.context().storageState()` selesai sebelum memakai hasilnya.
    const storageState = await loginPage.context().storageState();
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `loginPage.close()` selesai sebelum memakai hasilnya.
    await loginPage.close();
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `use(storageState)` selesai sebelum memakai hasilnya.
    await use(storageState);
  // Penjelasan: Melakukan operasi dengan membaca nilai `}, { scope: "worker" }]` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
  }, { scope: "worker" }],
  // Penjelasan: Mengisi properti `storageState` pada objek atau konfigurasi dengan membaca nilai `async ({ workerStorageState }, use) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
  storageState: async ({ workerStorageState }, use) => {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `use(workerStorageState)` selesai sebelum memakai hasilnya.
    await use(workerStorageState);
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

for (const mode of ["PEMULA", "MAHIR"]) {
test(`edit ruleset ${mode} menyimpan nama tanpa versi duplikat dan menampilkan konfigurasi terbaru`, async ({ page, request }) => {
  const apiUrl = process.env.E2E_API_URL || "http://localhost:5041";
  const auth = await request.post(`${apiUrl}/api/v1/auth/login`, { data: { username, password } });
  expect(auth.ok()).toBeTruthy();
  const headers = { Authorization: `Bearer ${(await auth.json()).access_token}` };
  const name = `E2E ruleset ${mode} ${Date.now()}`;
  let rulesetId;
  try {
    await page.goto("/rulesets/create");
    await page.locator(`input[name="cfg-mode"][value="${mode}"]`).check();
    await page.locator("#ruleset-name").fill(name);
    await page.locator("#cfg-cash").fill("23");
    await page.locator("form[data-ruleset-create-form] button[type=submit]").click();
    await expect(page).toHaveURL(/\/rulesets$/);
    const list = await request.get(`${apiUrl}/api/v1/rulesets`, { headers });
    const created = (await list.json()).items.find(item => item.name === name);
    expect(created).toBeTruthy();
    rulesetId = created.ruleset_id;
    const original = await (await request.get(`${apiUrl}/api/v1/rulesets/${rulesetId}`, { headers })).json();

    for (let save = 0; save < 2; save += 1) {
      await page.goto(`/rulesets/${rulesetId}/edit`);
      await expect(page.locator("#cfg-cash")).toHaveValue("23");
      await page.locator("#ruleset-name").fill(`${name} renamed`);
      await page.locator("#ruleset-description").fill("Description updated without changing configuration");
      await page.locator("form[data-ruleset-create-form] button[type=submit]").click();
      await expect(page).toHaveURL(new RegExp(`/rulesets/${rulesetId}$`));
      await expect(page.locator(".ruleset-title-block")).toContainText(`${name} renamed`);
      const saved = await (await request.get(`${apiUrl}/api/v1/rulesets/${rulesetId}`, { headers })).json();
      expect(saved.definition).toEqual(original.definition);
      await expect(page.locator(".ruleset-detail-section table tbody tr")).toHaveCount(1);
    }

    await page.goto(`/rulesets/${rulesetId}/edit`);
    await page.locator("#cfg-cash").fill("27");
    await page.locator("form[data-ruleset-create-form] button[type=submit]").click();
    await expect(page).toHaveURL(new RegExp(`/rulesets/${rulesetId}$`));
    await expect(page.locator("#ruleset-detail-starting-cash")).toHaveValue("27");
    await expect(page.locator(".ruleset-detail-section table tbody tr")).toHaveCount(2);
    await page.reload();
    await expect(page.locator("#ruleset-detail-starting-cash")).toHaveValue("27");
    await page.goto(`/rulesets/${rulesetId}?version=1`);
    await expect(page.locator("#ruleset-detail-starting-cash")).toHaveValue("23");
  } finally {
    if (rulesetId) {
      const cleanup = await request.delete(`${apiUrl}/api/v1/rulesets/${rulesetId}`, { headers });
      expect(cleanup.status()).toBe(204);
    }
  }
});
}

// Penjelasan: Melakukan operasi dengan memanggil `async function expectNoHorizontalOverflow(page) {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
test("pemain dipantau sama dengan jumlah peserta unik sesi instruktur", async ({ page }) => {
  await page.goto("/players");
  await expect(page.locator(".page-intro .subhead")).toContainText("pemain yang terdaftar dalam sesi Anda");
  await expect(page.getByText("Pemain di Luar Sesi Anda", { exact: true })).toHaveCount(0);
  await expect(page.getByText("Daftar Pemain Umum", { exact: true })).toHaveCount(0);
  const participantIds = await page.locator('.players-session-card a[href*="/players/"]').evaluateAll(
    anchors => anchors.map(anchor => anchor.getAttribute("href").split("/players/")[1])
  );
  const participantCount = new Set(participantIds).size;
  expect(participantCount).toBe(4);
  await expect(page.locator(".ruleset-index-count-block")).toContainText("Pemain Dipantau");
  await expect(page.locator(".ruleset-index-count")).toHaveText(String(participantCount));
  await page.goto("/");
  await expect(page.locator("#home-total-players")).toHaveText(String(participantCount));
});

test("analitika membedakan perubahan koin target dibeli dan jumlah aksi", async ({ page }) => {
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  await expect(page.locator(".player-analysis-scorecard > div").nth(2).locator("dd")).toHaveText("+20%");
  await page.locator("#player-analysis-atlas > summary").click();
  const goal = page.locator(".player-analysis-card--goal-ambition");
  await expect(goal.locator(".player-analysis-card__takeaway")).toContainText("Target Finansial Berhasil Dibeli");
  await expect(goal.locator(".player-analysis-card__takeaway > div > strong")).toHaveText(/1\s*target/);
  const planning = page.locator(".player-analysis-card--planning-horizon");
  await planning.locator("summary").click();
  await expect(planning.locator(".player-analysis-card__metrics dt")).toHaveText([
    "Aksi untuk Tabungan dan Pembelian Target", "Aksi untuk Mengaktifkan Asuransi",
    "Aksi untuk Pelunasan Pinjaman", "Total Aksi yang Digunakan"
  ]);
  await expect(planning.locator(".player-analysis-card__metrics dd strong")).toHaveText(["4", "1", "1", "32"]);
  await expect(planning).toContainText("(4 + 1 + 1) ÷ 32 × 100% = 18.75%");
  await expect(planning).not.toContainText(/token aksi|aksi utama/i);
  await expectNoHorizontalOverflow(page);
});

async function expectNoHorizontalOverflow(page) {
  // Penjelasan: Menyimpan `overflow` dengan menunggu operasi asinkron `page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth)` selesai sebelum memakai hasilnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
  // Penjelasan: Melakukan operasi dengan memanggil `expect(overflow).toBeLessThanOrEqual(1)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(overflow).toBeLessThanOrEqual(1);
// Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
}

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("beranda hanya menampilkan angka data, bukan watermark atau persentase redundan", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("beranda hanya menampilkan angka data, bukan watermark atau persentase redundan", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto("/")` selesai sebelum memakai hasilnya.
  await page.goto("/");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator(".home-stat-mark")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.locator(".home-stat-mark")).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator("#home-active-percentage")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.locator("#home-active-percentage")).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator(".home-live-summary")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.locator(".home-live-summary")).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator("#home-active-sessions")).toHaveText(/^\d+$/)` selesai sebelum memakai hasilnya.
  await expect(page.locator("#home-active-sessions")).toHaveText(/^\d+$/);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("tiga bagian pemain adalah accordion konsisten dan menyimpan pilihan", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("tiga bagian pemain adalah accordion konsisten dan menyimpan pilihan", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  // Penjelasan: Menyimpan `summary` dengan memanggil `page.locator("#player-statistics-summary")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const summary = page.locator("#player-statistics-summary");
  // Penjelasan: Menyimpan `analysis` dengan memanggil `page.locator("#player-analysis-atlas")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const analysis = page.locator("#player-analysis-atlas");
  // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const evidence = page.locator("#player-evidence-library");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(summary).toHaveAttribute("open", "")` selesai sebelum memakai hasilnya.
  await expect(summary).toHaveAttribute("open", "");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(analysis).not.toHaveAttribute("open", "")` selesai sebelum memakai hasilnya.
  await expect(analysis).not.toHaveAttribute("open", "");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(evidence).not.toHaveAttribute("open", "")` selesai sebelum memakai hasilnya.
  await expect(evidence).not.toHaveAttribute("open", "");

  // Penjelasan: Menyimpan `summaries` dengan memanggil `page.locator("[data-player-section-accordion] > summary")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const summaries = page.locator("[data-player-section-accordion] > summary");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(summaries).toHaveCount(3)` selesai sebelum memakai hasilnya.
  await expect(summaries).toHaveCount(3);
  // Penjelasan: Mengulang blok dengan pengaturan `let index = 0; index < 3; index += 1`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
  for (let index = 0; index < 3; index += 1) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(summaries.nth(index)).toHaveCSS("cursor", "pointer")` selesai sebelum memakai hasilnya.
    await expect(summaries.nth(index)).toHaveCSS("cursor", "pointer");
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `analysis.locator(":scope > summary").focus()` selesai sebelum memakai hasilnya.
  await analysis.locator(":scope > summary").focus();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.keyboard.press("Enter")` selesai sebelum memakai hasilnya.
  await page.keyboard.press("Enter");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(analysis).toHaveAttribute("open", "")` selesai sebelum memakai hasilnya.
  await expect(analysis).toHaveAttribute("open", "");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.reload()` selesai sebelum memakai hasilnya.
  await page.reload();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(analysis).toHaveAttribute("open", "")` selesai sebelum memakai hasilnya.
  await expect(analysis).toHaveAttribute("open", "");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("mode pemula tidak merender kelompok atau metrik khusus mahir", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("mode pemula tidak merender kelompok atau metrik khusus mahir", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.locator("#player-analysis-atlas > summary").click()` selesai sebelum memakai hasilnya.
  await page.locator("#player-analysis-atlas > summary").click();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.locator("#player-evidence-library > summary").click()` selesai sebelum memakai hasilnya.
  await page.locator("#player-evidence-library > summary").click();

  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator(".player-analysis-card__mode")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.locator(".player-analysis-card__mode")).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator(".player-evidence-domain__mode")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.locator(".player-evidence-domain__mode")).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator(".player-metric-series__outside-quota")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.locator(".player-metric-series__outside-quota")).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("ringkasan pemain menyusun enam informasi termasuk status misi koleksi", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("ringkasan pemain menyusun enam informasi termasuk status misi koleksi", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  // Penjelasan: Menyimpan `scorecard` dengan memanggil `page.locator("#player-statistics-summary .player-analysis-scorecard")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const scorecard = page.locator("#player-statistics-summary .player-analysis-scorecard");
  // Penjelasan: Menyimpan `mission` dengan memanggil `scorecard.locator(".player-analysis-scorecard__mission")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const mission = scorecard.locator(".player-analysis-scorecard__mission");

  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(mission.locator("dt")).toHaveText("Misi Koleksi")` selesai sebelum memakai hasilnya.
  await expect(mission.locator("dt")).toHaveText("Misi Koleksi");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(mission.locator("dd")).toHaveText("Belum selesai")` selesai sebelum memakai hasilnya.
  await expect(mission.locator("dd")).toHaveText("Belum selesai");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(mission).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(mission).toBeVisible();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(scorecard.locator(":scope > div")).toHaveCount(6)` selesai sebelum memakai hasilnya.
  await expect(scorecard.locator(":scope > div")).toHaveCount(6);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(scorecard.locator("dt")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(scorecard.locator("dt")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Koin Tersisa"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Koin Tersisa",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Selisih Koin Masuk dan Keluar"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Selisih Koin Masuk dan Keluar",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Koin Tersisa dibanding Koin Awal"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Perubahan Koin dari Awal",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Poin Kebahagiaan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Poin Kebahagiaan",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Pemerataan Kartu Kebutuhan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Pemerataan Kartu Kebutuhan",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Misi Koleksi"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Misi Koleksi"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(mission).toHaveCSS("grid-column", "auto")` selesai sebelum memakai hasilnya.
  await expect(mission).toHaveCSS("grid-column", "auto");

  // Disable entrance/hover animation before comparing the actual grid layout.
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.emulateMedia({ reducedMotion: "reduce" })` selesai sebelum memakai hasilnya.
  await page.emulateMedia({ reducedMotion: "reduce" });
  // Penjelasan: Menyimpan `boxes` dengan mentransformasikan setiap anggota koleksi dengan `item => {` untuk membentuk array hasil yang urutannya mengikuti sumber. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const boxes = await scorecard.locator(":scope > div").evaluateAll(items => items.map(item => {
    // Penjelasan: Menyimpan `box` dengan memanggil `item.getBoundingClientRect()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const box = item.getBoundingClientRect();
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `{ x: box.x, y: box.y, width: box.width, height: box.height }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
    return { x: box.x, y: box.y, width: box.width, height: box.height };
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  }));
  // Penjelasan: Menyimpan `columns` dengan memilih nilai melalui kondisi ternary `page.viewportSize().width <= 560 ? 2 : 3`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
  const columns = page.viewportSize().width <= 560 ? 2 : 3;
  // Penjelasan: Mengulang blok dengan pengaturan `let index = 0; index < boxes.length; index += 1`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
  for (let index = 0; index < boxes.length; index += 1) {
    // Penjelasan: Menyimpan `rowStart` dengan memanggil `Math.floor(index / columns) * columns` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const rowStart = Math.floor(index / columns) * columns;
    // Penjelasan: Melakukan operasi dengan menghitung besar absolut `boxes[index].y - boxes[rowStart].y)).toBeLessThanOrEqual(1` tanpa tanda negatif untuk keputusan format atau skala.
    expect(Math.abs(boxes[index].y - boxes[rowStart].y)).toBeLessThanOrEqual(1);
    // Penjelasan: Melakukan operasi dengan menghitung besar absolut `boxes[index].width - boxes[0].width)).toBeLessThanOrEqual(1` tanpa tanda negatif untuk keputusan format atau skala.
    expect(Math.abs(boxes[index].width - boxes[0].width)).toBeLessThanOrEqual(1);
    // Penjelasan: Melakukan operasi dengan menghitung besar absolut `boxes[index].height - boxes[rowStart].height)).toBeLessThanOrEqual(1` tanpa tanda negatif untuk keputusan format atau skala.
    expect(Math.abs(boxes[index].height - boxes[rowStart].height)).toBeLessThanOrEqual(1);
    // Penjelasan: Memeriksa kondisi `index >= columns`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (index >= columns) {
      // Penjelasan: Melakukan operasi dengan memanggil `expect(boxes[index].y).toBeGreaterThan(boxes[index - columns].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
      expect(boxes[index].y).toBeGreaterThan(boxes[index - columns].y);
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);

  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.locator("#player-analysis-atlas > summary").click()` selesai sebelum memakai hasilnya.
  await page.locator("#player-analysis-atlas > summary").click();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator(".player-analysis-card--action-efficiency")).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(page.locator(".player-analysis-card--action-efficiency")).toBeVisible();

  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(mission).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(mission).toBeVisible();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(mission.locator("dt")).toHaveText("Misi Koleksi")` selesai sebelum memakai hasilnya.
  await expect(mission.locator("dt")).toHaveText("Misi Koleksi");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(scorecard.locator(":scope > div")).toHaveCount(6)` selesai sebelum memakai hasilnya.
  await expect(scorecard.locator(":scope > div")).toHaveCount(6);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(scorecard.locator("dt").last()).toHaveText("Misi Koleksi")` selesai sebelum memakai hasilnya.
  await expect(scorecard.locator("dt").last()).toHaveText("Misi Koleksi");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("koin dan keuangan menampilkan satu tabel transaksi komprehensif", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("koin dan keuangan menampilkan satu tabel transaksi komprehensif", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const evidence = page.locator("#player-evidence-library");
  // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await evidence.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await evidence.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `coinDomain` dengan memanggil `page.locator("details.player-evidence-domain").first()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const coinDomain = page.locator("details.player-evidence-domain").first();
  // Penjelasan: Memeriksa kondisi `(await coinDomain.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await coinDomain.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `coinDomain.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await coinDomain.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.getByText("Perjalanan Koin dan Peristiwa", { exact: true })).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.getByText("Perjalanan Koin dan Peristiwa", { exact: true })).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(coinDomain.getByRole("heading", { name: "Riwayat Transaksi" })).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(coinDomain.getByRole("heading", { name: "Riwayat Transaksi" })).toBeVisible();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(coinDomain.getByRole("columnheader", { name: "Koin Masuk (koin)" })).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(coinDomain.getByRole("columnheader", { name: "Koin Masuk (koin)" })).toBeVisible();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(coinDomain.getByRole("columnheader", { name: "Koin Keluar (koin)" })).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(coinDomain.getByRole("columnheader", { name: "Koin Keluar (koin)" })).toBeVisible();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(coinDomain.getByRole("columnheader", { name: "Perubahan Koin (koin)" })).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(coinDomain.getByRole("columnheader", { name: "Perubahan Koin (koin)" })).toBeVisible();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(coinDomain.getByRole("columnheader", { name: "Saldo setelah Kejadian (koin)" })).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(coinDomain.getByRole("columnheader", { name: "Saldo setelah Kejadian (koin)" })).toBeVisible();

  // Penjelasan: Menyimpan `headings` dengan menunggu operasi asinkron `coinDomain.locator(".player-metric-card h5").allTextContents()` selesai sebelum memakai hasilnya.
  const headings = await coinDomain.locator(".player-metric-card h5").allTextContents();
  // Penjelasan: Melakukan operasi dengan memanggil `expect(headings).toEqual([` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(headings).toEqual([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Koin Awal"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Koin Awal",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Koin Tersisa"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Koin Tersisa",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Riwayat Transaksi"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Riwayat Transaksi",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Pinjaman Pertama Tercatat"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Pinjaman Pertama Tercatat",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Aktivitas Terakhir Tercatat"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Aktivitas Terakhir Tercatat"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);

  // Penjelasan: Menyimpan `cards` dengan memanggil `coinDomain.locator(".player-metric-card")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const cards = coinDomain.locator(".player-metric-card");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards).toHaveCount(5)` selesai sebelum memakai hasilnya.
  await expect(cards).toHaveCount(5);
  // Penjelasan: Menyimpan `transactionCard` dengan memanggil `cards.nth(2)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const transactionCard = cards.nth(2);
  // Penjelasan: Menyimpan `firstTransactionDay` dengan memanggil `transactionCard.locator("tbody tr").first().locator("td").first()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const firstTransactionDay = transactionCard.locator("tbody tr").first().locator("td").first();
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(await transactionCard.evaluate(card => getComputedStyle(card.parentElement).borderTopWidth)).toBe("0px");`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(await transactionCard.evaluate(card => getComputedStyle(card.parentElement).borderTopWidth)).toBe("0px");
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(await firstTransactionDay.evaluate(cell => getComputedStyle(cell).whiteSpace)).toBe("nowrap");`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(await firstTransactionDay.evaluate(cell => getComputedStyle(cell).whiteSpace)).toBe("nowrap");
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(await firstTransactionDay.evaluate(cell => cell.scrollWidth <= cell.clientWidth)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(await firstTransactionDay.evaluate(cell => cell.scrollWidth <= cell.clientWidth)).toBeTruthy();
  // Penjelasan: Menyimpan `layout` dengan mentransformasikan setiap anggota koleksi dengan `item => {` untuk membentuk array hasil yang urutannya mengikuti sumber. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const layout = await cards.evaluateAll(items => items.map(item => {
    // Penjelasan: Menyimpan `box` dengan memanggil `item.getBoundingClientRect()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const box = item.getBoundingClientRect();
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya; eksekusi fungsi berakhir setelah nilai dihitung.
    return {
      // Penjelasan: Mengisi properti `overview` pada objek atau konfigurasi dengan memeriksa keberadaan kelas `"player-metric-card--coin-overview"` untuk membaca status visual komponen; konsumen objek membaca nilai ini melalui nama properti tersebut.
      overview: item.classList.contains("player-metric-card--coin-overview"),
      // Penjelasan: Mengisi properti `series` pada objek atau konfigurasi dengan memeriksa keberadaan kelas `"player-metric-card--series"` untuk membaca status visual komponen; konsumen objek membaca nilai ini melalui nama properti tersebut.
      series: item.classList.contains("player-metric-card--series"),
      // Penjelasan: Mengisi properti `width` pada objek atau konfigurasi dengan membaca nilai `box.width` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
      width: box.width,
      // Penjelasan: Mengisi properti `y` pada objek atau konfigurasi dengan membaca nilai `box.y` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
      y: box.y
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    };
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  }));
  // Penjelasan: Menyimpan `gridWidth` dengan menunggu operasi asinkron `coinDomain.locator(".player-metric-card-grid").evaluate(` selesai sebelum memakai hasilnya.
  const gridWidth = await coinDomain.locator(".player-metric-card-grid").evaluate(
    // Penjelasan: Memperbarui `grid` dengan memanggil `> grid.getBoundingClientRect().width` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    grid => grid.getBoundingClientRect().width
  // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  );

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(0, 2).every(item => item.overview)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(layout.slice(0, 2).every(item => item.overview)).toBeTruthy();
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].series).toBeTruthy()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[2].series).toBeTruthy();
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(3).every(item => !item.series)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(layout.slice(3).every(item => !item.series)).toBeTruthy();
  // Penjelasan: Memeriksa kondisi `page.viewportSize().width >= 768`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (page.viewportSize().width >= 768) {
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[0].y).toBe(layout[1].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[0].y).toBe(layout[1].y);
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(0, 2).every(item => item.width > gridWidth * 0.45)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(layout.slice(0, 2).every(item => item.width > gridWidth * 0.45)).toBeTruthy();
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].width).toBeGreaterThan(gridWidth * 0.9)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[2].width).toBeGreaterThan(gridWidth * 0.9);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[3].y).toBe(layout[4].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[3].y).toBe(layout[4].y);
  // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
  } else {
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[1].y).toBeGreaterThan(layout[0].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[1].y).toBeGreaterThan(layout[0].y);
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.every(item => item.width > gridWidth * 0.9)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(layout.every(item => item.width > gridWidth * 0.9)).toBeTruthy();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].y).toBeGreaterThan(layout[1].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[2].y).toBeGreaterThan(layout[1].y);
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[3].y).toBeGreaterThan(layout[2].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[3].y).toBeGreaterThan(layout[2].y);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("kartu bahan ringkas dengan jumlah bahan untuk setiap pesanan", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("kartu bahan ringkas dengan jumlah bahan untuk setiap pesanan", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.locator("#player-evidence-library > summary").click()` selesai sebelum memakai hasilnya.
  await page.locator("#player-evidence-library > summary").click();
  // Penjelasan: Menyimpan `ingredients` dengan memanggil `page.locator("details.player-evidence-domain").nth(1)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const ingredients = page.locator("details.player-evidence-domain").nth(1);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `ingredients.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
  await ingredients.locator(":scope > summary").click();

  // Penjelasan: Menyimpan `cards` dengan memanggil `ingredients.locator(".player-metric-card")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const cards = ingredients.locator(".player-metric-card");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.locator("h5")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.locator("h5")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Bahan Terkumpul"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Bahan Terkumpul",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Bahan Tersisa"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Bahan Tersisa",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Bahan Digunakan per Pesanan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Bahan Digunakan per Pesanan",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Bahan Digunakan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Bahan Digunakan",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Rata-rata Bahan per Pesanan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Rata-rata Bahan per Pesanan",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Biaya Pembelian Bahan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Biaya Pembelian Bahan"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(1).locator(".player-metric-card__value strong")).toHaveText("2")` selesai sebelum memakai hasilnya.
  await expect(cards.nth(1).locator(".player-metric-card__value strong")).toHaveText("2");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(1)).toContainText("Bahan yang sudah digunakan untuk pesanan atau dibuang tidak termasuk.")` selesai sebelum memakai hasilnya.
  await expect(cards.nth(1)).toContainText("Bahan yang sudah digunakan untuk pesanan atau dibuang tidak termasuk.");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(2).locator("th")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.nth(2).locator("th")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Pesanan ke-", "Jumlah Kartu Bahan yang Digunakan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Pesanan ke-", "Jumlah Kartu Bahan yang Digunakan"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(2).locator("tbody tr")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.nth(2).locator("tbody tr")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"1 2", "2 3", "3 3", "4 2", "5 3"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "1 2", "2 3", "3 3", "4 2", "5 3"
  // Penjelasan: Melakukan operasi dengan membaca nilai `], { useInnerText: true })` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
  ], { useInnerText: true });
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(2)).toContainText("Angka pesanan bukan nomor pada kartu pesanan.")` selesai sebelum memakai hasilnya.
  await expect(cards.nth(2)).toContainText("Angka pesanan bukan nomor pada kartu pesanan.");
  // Penjelasan: Menyimpan `layout` dengan mentransformasikan setiap anggota koleksi dengan `item => {` untuk membentuk array hasil yang urutannya mengikuti sumber. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const layout = await cards.evaluateAll(items => items.map(item => {
    // Penjelasan: Menyimpan `box` dengan memanggil `item.getBoundingClientRect()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const box = item.getBoundingClientRect();
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `{ width: box.width, y: box.y }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
    return { width: box.width, y: box.y };
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  }));
  // Penjelasan: Menyimpan `gridWidth` dengan menunggu operasi asinkron `ingredients.locator(".player-metric-card-grid").evaluate(` selesai sebelum memakai hasilnya.
  const gridWidth = await ingredients.locator(".player-metric-card-grid").evaluate(
    // Penjelasan: Memperbarui `grid` dengan memanggil `> grid.clientWidth - parseFloat(getComputedStyle(grid).paddingLeft) - parseFloat(getComputedStyle(grid).paddingRight)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    grid => grid.clientWidth - parseFloat(getComputedStyle(grid).paddingLeft) - parseFloat(getComputedStyle(grid).paddingRight)
  // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  );
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].width).toBeGreaterThan(gridWidth * 0.9)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[2].width).toBeGreaterThan(gridWidth * 0.9);
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].y).toBeGreaterThan(layout[1].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[2].y).toBeGreaterThan(layout[1].y);
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[3].y).toBeGreaterThan(layout[2].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[3].y).toBeGreaterThan(layout[2].y);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("rincian risiko tersusun berurutan tanpa kartu mengapit tabel", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("rincian risiko tersusun berurutan tanpa kartu mengapit tabel", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const evidence = page.locator("#player-evidence-library");
  // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await evidence.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await evidence.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `riskDomain` dengan memanggil `page.locator("details.player-evidence-domain").nth(7)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const riskDomain = page.locator("details.player-evidence-domain").nth(7);
  // Penjelasan: Memeriksa kondisi `(await riskDomain.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await riskDomain.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `riskDomain.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await riskDomain.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `cards` dengan memanggil `riskDomain.locator(".player-metric-card")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const cards = riskDomain.locator(".player-metric-card");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards).toHaveCount(6)` selesai sebelum memakai hasilnya.
  await expect(cards).toHaveCount(6);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.locator("h5")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.locator("h5")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Kartu Risiko Kehidupan yang Muncul"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Kartu Risiko Kehidupan yang Muncul",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Nominal Dampak Koin per Kartu Risiko"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Nominal Dampak Koin per Kartu Risiko",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Nominal Dampak Koin Kartu Risiko"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Nominal Dampak Koin Kartu Risiko",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Risiko yang Ditanggung Asuransi"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Risiko yang Ditanggung Asuransi",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Premi Asuransi Dibayar"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Premi Asuransi Dibayar",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Penggunaan Tindakan Darurat"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Penggunaan Tindakan Darurat"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(1).locator("th").first()).toHaveText("No.")` selesai sebelum memakai hasilnya.
  await expect(cards.nth(1).locator("th").first()).toHaveText("No.");

  // Penjelasan: Menyimpan `layout` dengan mentransformasikan setiap anggota koleksi dengan `item => {` untuk membentuk array hasil yang urutannya mengikuti sumber. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const layout = await cards.evaluateAll(items => items.map(item => {
    // Penjelasan: Menyimpan `box` dengan memanggil `item.getBoundingClientRect()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const box = item.getBoundingClientRect();
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `{ width: box.width, y: box.y }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
    return { width: box.width, y: box.y };
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  }));
  // Penjelasan: Menyimpan `gridWidth` dengan menunggu operasi asinkron `riskDomain.locator(".player-metric-card-grid").evaluate(` selesai sebelum memakai hasilnya.
  const gridWidth = await riskDomain.locator(".player-metric-card-grid").evaluate(
    // Penjelasan: Memperbarui `grid` dengan memanggil `> grid.getBoundingClientRect().width` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    grid => grid.getBoundingClientRect().width
  // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  );

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(0, 3).every(item => item.width > gridWidth * 0.9)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(layout.slice(0, 3).every(item => item.width > gridWidth * 0.9)).toBeTruthy();
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[1].y).toBeGreaterThan(layout[0].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[1].y).toBeGreaterThan(layout[0].y);
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].y).toBeGreaterThan(layout[1].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[2].y).toBeGreaterThan(layout[1].y);
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[3].y).toBeGreaterThan(layout[2].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[3].y).toBeGreaterThan(layout[2].y);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("rincian emas tersusun menjadi empat ringkasan dua harga dan tiga hasil", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("rincian emas tersusun menjadi empat ringkasan dua harga dan tiga hasil", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const evidence = page.locator("#player-evidence-library");
  // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await evidence.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await evidence.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `goldDomain` dengan memanggil `page.locator("details.player-evidence-domain").nth(5)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const goldDomain = page.locator("details.player-evidence-domain").nth(5);
  // Penjelasan: Memeriksa kondisi `(await goldDomain.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await goldDomain.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `goldDomain.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await goldDomain.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `cards` dengan memanggil `goldDomain.locator(".player-metric-card")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const cards = goldDomain.locator(".player-metric-card");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards).toHaveCount(9)` selesai sebelum memakai hasilnya.
  await expect(cards).toHaveCount(9);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.locator("h5")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.locator("h5")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Kartu Emas Awal"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Kartu Emas Awal",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Kartu Emas Dibeli selama Permainan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Kartu Emas Dibeli selama Permainan",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Kartu Emas Dijual"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Kartu Emas Dijual",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Kartu Emas Tersisa"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Kartu Emas Tersisa",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Harga Emas Saat Beli"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Harga Beli per Kartu Emas",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Harga Emas Saat Jual"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Harga Jual per Kartu Emas",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Biaya Pembelian Emas"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Biaya Pembelian Emas",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Pendapatan Penjualan Emas"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Pendapatan Penjualan Emas",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Selisih Arus Kas Transaksi Emas"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Selisih Arus Kas Transaksi Emas"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);

  // Penjelasan: Menyimpan `layout` dengan mentransformasikan setiap anggota koleksi dengan `item => {` untuk membentuk array hasil yang urutannya mengikuti sumber. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const layout = await cards.evaluateAll(items => items.map(item => {
    // Penjelasan: Menyimpan `box` dengan memanggil `item.getBoundingClientRect()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const box = item.getBoundingClientRect();
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan memeriksa keberadaan kelas `"player-metric-card--series"` untuk membaca status visual komponen; eksekusi fungsi berakhir setelah nilai dihitung.
    return { series: item.classList.contains("player-metric-card--series"), width: box.width, y: box.y };
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  }));
  // Penjelasan: Menyimpan `gridContentWidth` dengan menunggu operasi asinkron `goldDomain.locator(".player-metric-card-grid").evaluate(grid => {` selesai sebelum memakai hasilnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const gridContentWidth = await goldDomain.locator(".player-metric-card-grid").evaluate(grid => {
    // Penjelasan: Menyimpan `style` dengan memanggil `getComputedStyle(grid)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const style = getComputedStyle(grid);
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan memanggil `grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; eksekusi fungsi berakhir setelah nilai dihitung.
    return grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(0, 4).every(item => !item.series)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(layout.slice(0, 4).every(item => !item.series)).toBeTruthy();
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(4, 6).every(item => item.series)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(layout.slice(4, 6).every(item => item.series)).toBeTruthy();
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(6).every(item => !item.series)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(layout.slice(6).every(item => !item.series)).toBeTruthy();
  // Penjelasan: Memeriksa kondisi `page.viewportSize().width >= 768`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (page.viewportSize().width >= 768) {
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[0].y).toBe(layout[1].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[0].y).toBe(layout[1].y);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].y).toBe(layout[3].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[2].y).toBe(layout[3].y);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[4].y).toBe(layout[5].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[4].y).toBe(layout[5].y);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[6].y).toBe(layout[7].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[6].y).toBe(layout[7].y);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[7].y).toBe(layout[8].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[7].y).toBe(layout[8].y);
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(0, 6).every(item => item.width > gridContentWidth * 0.45)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(layout.slice(0, 6).every(item => item.width > gridContentWidth * 0.45)).toBeTruthy();
  // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
  } else {
    // Penjelasan: Melakukan operasi dengan memanggil `expect(` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(
      // Penjelasan: Mendefinisikan fungsi atau pemetaan `layout.every(item => item.width > gridContentWidth * 0.98),`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
      layout.every(item => item.width > gridContentWidth * 0.98),
      // Penjelasan: Mendefinisikan fungsi atau pemetaan `JSON.stringify({ gridContentWidth, widths: layout.map(item => item.width) })`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
      JSON.stringify({ gridContentWidth, widths: layout.map(item => item.width) })
    // Penjelasan: Melakukan operasi dengan memanggil `).toBeTruthy()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    ).toBeTruthy();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[4].y).toBeGreaterThan(layout[3].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[4].y).toBeGreaterThan(layout[3].y);
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[6].y).toBeGreaterThan(layout[5].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[6].y).toBeGreaterThan(layout[5].y);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("rincian donasi menggabungkan jumlah dan peringkat dalam satu tabel", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("rincian donasi menggabungkan jumlah dan peringkat dalam satu tabel", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const evidence = page.locator("#player-evidence-library");
  // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await evidence.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await evidence.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `donationDomain` dengan memanggil `page.locator("details.player-evidence-domain").nth(4)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const donationDomain = page.locator("details.player-evidence-domain").nth(4);
  // Penjelasan: Memeriksa kondisi `(await donationDomain.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await donationDomain.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `donationDomain.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await donationDomain.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `cards` dengan memanggil `donationDomain.locator(".player-metric-card")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const cards = donationDomain.locator(".player-metric-card");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards).toHaveCount(4)` selesai sebelum memakai hasilnya.
  await expect(cards).toHaveCount(4);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.locator("h5")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.locator("h5")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Donasi Setiap Jumat"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Donasi Setiap Jumat",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Koin Donasi"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Koin Donasi",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Kartu Juara Donasi Diperoleh"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Kartu Juara Donasi Diperoleh",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Poin Kebahagiaan dari Donasi"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Poin Kebahagiaan dari Donasi"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);
  // Penjelasan: Menyimpan `table` dengan memanggil `donationDomain.locator("table")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const table = donationDomain.locator("table");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(table).toHaveCount(1)` selesai sebelum memakai hasilnya.
  await expect(table).toHaveCount(1);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(table.locator("th")).toHaveText(["Hari", /Jumlah Donasi\s*\(koin\)/, "Peringkat"])` selesai sebelum memakai hasilnya.
  await expect(table.locator("th")).toHaveText(["Hari", /Jumlah Donasi\s*\(koin\)/, "Peringkat"]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(table.locator("tbody tr")).toHaveCount(3)` selesai sebelum memakai hasilnya.
  await expect(table.locator("tbody tr")).toHaveCount(3);
  // Penjelasan: Mengulang blok dengan pengaturan `const [index, values] of [[0, ["5", "1", "4"]], [1, ["12", "4", "2"]], [2, ["19", "1", "4"]]]`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
  for (const [index, values] of [[0, ["5", "1", "4"]], [1, ["12", "4", "2"]], [2, ["19", "1", "4"]]]) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(table.locator("tbody tr").nth(index).locator("td")).toHaveText(values)` selesai sebelum memakai hasilnya.
    await expect(table.locator("tbody tr").nth(index).locator("td")).toHaveText(values);
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `layout` dengan mentransformasikan setiap anggota koleksi dengan `item => {` untuk membentuk array hasil yang urutannya mengikuti sumber. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const layout = await cards.evaluateAll(items => items.map(item => {
    // Penjelasan: Menyimpan `box` dengan memanggil `item.getBoundingClientRect()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const box = item.getBoundingClientRect();
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan memeriksa keberadaan kelas `"player-metric-card--series"` untuk membaca status visual komponen; eksekusi fungsi berakhir setelah nilai dihitung.
    return { series: item.classList.contains("player-metric-card--series"), width: box.width, y: box.y };
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  }));
  // Penjelasan: Menyimpan `gridContentWidth` dengan menunggu operasi asinkron `donationDomain.locator(".player-metric-card-grid").evaluate(grid => {` selesai sebelum memakai hasilnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const gridContentWidth = await donationDomain.locator(".player-metric-card-grid").evaluate(grid => {
    // Penjelasan: Menyimpan `style` dengan memanggil `getComputedStyle(grid)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const style = getComputedStyle(grid);
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan memanggil `grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; eksekusi fungsi berakhir setelah nilai dihitung.
    return grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[0].series).toBeTruthy()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[0].series).toBeTruthy();
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[0].width).toBeGreaterThan(gridContentWidth * 0.98)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[0].width).toBeGreaterThan(gridContentWidth * 0.98);
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(1).every(item => !item.series)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  expect(layout.slice(1).every(item => !item.series)).toBeTruthy();
  // Penjelasan: Memeriksa kondisi `page.viewportSize().width >= 768`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (page.viewportSize().width >= 768) {
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[1].y).toBe(layout[2].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[1].y).toBe(layout[2].y);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].y).toBe(layout[3].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[2].y).toBe(layout[3].y);
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(1).every(item => item.width > gridContentWidth * 0.3)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(layout.slice(1).every(item => item.width > gridContentWidth * 0.3)).toBeTruthy();
  // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
  } else {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.every(item => item.width > gridContentWidth * 0.98)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(layout.every(item => item.width > gridContentWidth * 0.98)).toBeTruthy();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[1].y).toBeGreaterThan(layout[0].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[1].y).toBeGreaterThan(layout[0].y);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("kartu kebutuhan tetap menjelaskan kepemilikan tanpa indikator 70 persen", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("kartu kebutuhan tetap menjelaskan kepemilikan tanpa indikator 70 persen", async ({ page }) => {
  // Penjelasan: Mengulang blok dengan pengaturan `const sessionId of [advancedSessionId, beginnerSessionId]`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
  for (const sessionId of [advancedSessionId, beginnerSessionId]) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${sessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
    await page.goto(`/sessions/${sessionId}/players/${playerId}`);
    // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const evidence = page.locator("#player-evidence-library");
    // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if ((await evidence.getAttribute("open")) === null) {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
      await evidence.locator(":scope > summary").click();
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
    // Penjelasan: Menyimpan `needs` dengan memanggil `evidence.locator("details.player-evidence-domain").nth(3)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const needs = evidence.locator("details.player-evidence-domain").nth(3);
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `needs.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await needs.locator(":scope > summary").click();
    // Penjelasan: Melakukan operasi dengan menyaring koleksi menggunakan predikat `{ hasText: /70%/ })).toHaveCount(0)`; hanya elemen yang memenuhi kondisi masuk hasil.
    await expect(needs.locator(".player-metric-card h5").filter({ hasText: /70%/ })).toHaveCount(0);
    // Penjelasan: Menyimpan `owned` dengan menyaring koleksi menggunakan predikat `{`; hanya elemen yang memenuhi kondisi masuk hasil.
    const owned = needs.locator(".player-metric-card").filter({
      // Penjelasan: Mengisi properti `has` pada objek atau konfigurasi dengan memanggil `page.getByRole("heading", { name: "Kartu Kebutuhan yang Masih Dimiliki", exact: true })` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; konsumen objek membaca nilai ini melalui nama properti tersebut.
      has: page.getByRole("heading", { name: "Kartu Kebutuhan yang Masih Dimiliki", exact: true })
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(owned).toBeVisible()` selesai sebelum memakai hasilnya.
    await expect(owned).toBeVisible();
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(owned.locator(".player-metric-card__explanation")).toContainText("Jumlah kartu kebutuhan yang masih dimiliki setelah aktivitas terakhir.")` selesai sebelum memakai hasilnya.
    await expect(owned.locator(".player-metric-card__explanation")).toContainText("Jumlah kartu kebutuhan yang masih dimiliki setelah aktivitas terakhir.");
    // Penjelasan: Memeriksa kondisi `sessionId === advancedSessionId`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (sessionId === advancedSessionId) {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(owned.locator(".player-metric-card__value strong")).toHaveText("1")` selesai sebelum memakai hasilnya.
      await expect(owned.locator(".player-metric-card__value strong")).toHaveText("1");
      // Penjelasan: Menyimpan `purchased` dengan menyaring koleksi menggunakan predikat `{`; hanya elemen yang memenuhi kondisi masuk hasil.
      const purchased = needs.locator(".player-metric-card").filter({
        // Penjelasan: Mengisi properti `has` pada objek atau konfigurasi dengan memanggil `page.getByRole("heading", { name: "Kartu Kebutuhan Dibeli", exact: true })` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; konsumen objek membaca nilai ini melalui nama properti tersebut.
        has: page.getByRole("heading", { name: "Kartu Kebutuhan Dibeli", exact: true })
      // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
      });
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(purchased.locator(".player-metric-card__value strong")).toHaveText("1")` selesai sebelum memakai hasilnya.
      await expect(purchased.locator(".player-metric-card__value strong")).toHaveText("1");
    // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
    } else {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(owned.locator(".player-metric-card__explanation")).not.toContainText("Tindakan Darurat")` selesai sebelum memakai hasilnya.
      await expect(owned.locator(".player-metric-card__explanation")).not.toContainText("Tindakan Darurat");
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
    // Penjelasan: Menyimpan `analysis` dengan memanggil `page.locator("#player-analysis-atlas")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const analysis = page.locator("#player-analysis-atlas");
    // Penjelasan: Memeriksa kondisi `(await analysis.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if ((await analysis.getAttribute("open")) === null) {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `analysis.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
      await analysis.locator(":scope > summary").click();
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(analysis.locator(".player-analysis-card--fulfillment-diversity")).toContainText("Pemerataan Kartu Kebutuhan")` selesai sebelum memakai hasilnya.
    await expect(analysis.locator(".player-analysis-card--fulfillment-diversity")).toContainText("Pemerataan Kartu Kebutuhan");
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
    await expectNoHorizontalOverflow(page);
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("target finansial diringkas menjadi hasil target tabungan dan sisa pinjaman", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("target finansial diringkas menjadi hasil target tabungan dan sisa pinjaman", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const evidence = page.locator("#player-evidence-library");
  // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await evidence.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await evidence.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
  // Penjelasan: Menyimpan `goals` dengan menyaring koleksi menggunakan predikat `{`; hanya elemen yang memenuhi kondisi masuk hasil.
  const goals = evidence.locator("details.player-evidence-domain").filter({
    // Penjelasan: Mengisi properti `has` pada objek atau konfigurasi dengan memanggil `page.locator("summary strong", { hasText: /^Target Finansial$/ })` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; konsumen objek membaca nilai ini melalui nama properti tersebut.
    has: page.locator("summary strong", { hasText: /^Target Finansial$/ })
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `goals.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
  await goals.locator(":scope > summary").click();
  // Penjelasan: Menyimpan `cards` dengan memanggil `goals.locator(".player-metric-card")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const cards = goals.locator(".player-metric-card");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards).toHaveCount(3)` selesai sebelum memakai hasilnya.
  await expect(cards).toHaveCount(3);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.locator("h5")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.locator("h5")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Target Finansial yang Selesai", "Koin dalam Tabungan", "Sisa Pinjaman"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Target Finansial Berhasil Dibeli", "Koin dalam Tabungan", "Sisa Pinjaman"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.locator(".player-metric-card__value strong")).toHaveText(["1", "0", "0"])` selesai sebelum memakai hasilnya.
  await expect(cards.locator(".player-metric-card__value strong")).toHaveText(["1", "0", "0"]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.locator(".player-metric-card__value small")).toHaveText(["target", "koin", "koin"])` selesai sebelum memakai hasilnya.
  await expect(cards.locator(".player-metric-card__value small")).toHaveText(["target", "koin", "koin"]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.first().locator(".player-metric-card__explanation")).toHaveText("Dari 1 target yang mulai didanai.")` selesai sebelum memakai hasilnya.
  await expect(cards.first().locator(".player-metric-card__explanation")).toHaveText("Dari 1 target yang mulai didanai.");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(goals.locator("table, .player-evidence-domain__guide")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(goals.locator("table, .player-evidence-domain__guide")).toHaveCount(0);

  // Penjelasan: Menyimpan `boxes` dengan mentransformasikan setiap anggota koleksi dengan `item => {` untuk membentuk array hasil yang urutannya mengikuti sumber. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const boxes = await cards.evaluateAll(items => items.map(item => {
    // Penjelasan: Menyimpan `box` dengan memanggil `item.getBoundingClientRect()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const box = item.getBoundingClientRect();
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `{ width: box.width, y: box.y }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
    return { width: box.width, y: box.y };
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  }));
  // Penjelasan: Menyimpan `width` dengan menunggu operasi asinkron `goals.locator(".player-metric-card-grid").evaluate(grid => {` selesai sebelum memakai hasilnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const width = await goals.locator(".player-metric-card-grid").evaluate(grid => {
    // Penjelasan: Menyimpan `style` dengan memanggil `getComputedStyle(grid)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const style = getComputedStyle(grid);
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan memanggil `grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; eksekusi fungsi berakhir setelah nilai dihitung.
    return grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
  // Penjelasan: Memeriksa kondisi `page.viewportSize().width >= 768`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (page.viewportSize().width >= 768) {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(boxes.every(box => box.y === boxes[0].y && box.width > width * 0.3)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(boxes.every(box => box.y === boxes[0].y && box.width > width * 0.3)).toBeTruthy();
  // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
  } else {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(boxes.every(box => box.width > width * 0.98)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(boxes.every(box => box.width > width * 0.98)).toBeTruthy();
    // Penjelasan: Melakukan operasi dengan memanggil `expect(boxes[1].y).toBeGreaterThan(boxes[0].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(boxes[1].y).toBeGreaterThan(boxes[0].y);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(boxes[2].y).toBeGreaterThan(boxes[1].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(boxes[2].y).toBeGreaterThan(boxes[1].y);
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);

  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${beginnerSessionId}/players/${playerId}`);
  // Penjelasan: Melakukan operasi dengan menyaring koleksi menggunakan predikat `{ hasText: /^Target Finansial$/ })).toHaveCount(0)`; hanya elemen yang memenuhi kondisi masuk hasil.
  await expect(page.locator("details.player-evidence-domain > summary strong").filter({ hasText: /^Target Finansial$/ })).toHaveCount(0);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("istilah Poin Kebahagiaan konsisten pada label satuan rumus dan panduan", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("istilah Poin Kebahagiaan konsisten pada label satuan rumus dan panduan", async ({ page }) => {
  // Penjelasan: Mengulang blok dengan pengaturan `const sessionId of [advancedSessionId, beginnerSessionId]`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
  for (const sessionId of [advancedSessionId, beginnerSessionId]) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${sessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
    await page.goto(`/sessions/${sessionId}/players/${playerId}`);
    // Penjelasan: Menyimpan `analysis` dengan memanggil `page.locator("#player-analysis-atlas")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const analysis = page.locator("#player-analysis-atlas");
    // Penjelasan: Memeriksa kondisi `(await analysis.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if ((await analysis.getAttribute("open")) === null) {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `analysis.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
      await analysis.locator(":scope > summary").click();
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
    // Penjelasan: Menyimpan `happiness` dengan memanggil `analysis.locator(".player-analysis-card--happiness-portfolio")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const happiness = analysis.locator(".player-analysis-card--happiness-portfolio");
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `happiness.locator(".player-analysis-card__method > summary").click()` selesai sebelum memakai hasilnya.
    await happiness.locator(".player-analysis-card__method > summary").click();
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(happiness).toContainText("Poin Kebahagiaan Kartu Kebutuhan")` selesai sebelum memakai hasilnya.
    await expect(happiness).toContainText("Poin Kebahagiaan Kartu Kebutuhan");
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(happiness).toContainText("Poin Kebahagiaan Dana Pensiun")` selesai sebelum memakai hasilnya.
    await expect(happiness).toContainText("Poin Kebahagiaan Dana Pensiun");
    // Penjelasan: Menyimpan `units` dengan memanggil `happiness.locator(".player-analysis-card__metric small")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const units = happiness.locator(".player-analysis-card__metric small");
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(await units.count()).toBeGreaterThan(0)` selesai sebelum memakai hasilnya.
    expect(await units.count()).toBeGreaterThan(0);
    // Penjelasan: Mengulang blok dengan pengaturan `const unit of await units.all()`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
    for (const unit of await units.all()) {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(unit).toHaveText("poin kebahagiaan")` selesai sebelum memakai hasilnya.
      await expect(unit).toHaveText("poin kebahagiaan");
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const evidence = page.locator("#player-evidence-library");
    // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if ((await evidence.getAttribute("open")) === null) {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
      await evidence.locator(":scope > summary").click();
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
    // Penjelasan: Menyimpan `pension` dengan memanggil `evidence.locator("details.player-evidence-domain").nth(6)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const pension = evidence.locator("details.player-evidence-domain").nth(6);
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `pension.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await pension.locator(":scope > summary").click();
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(pension).toContainText("Poin Kebahagiaan Dana Pensiun")` selesai sebelum memakai hasilnya.
    await expect(pension).toContainText("Poin Kebahagiaan Dana Pensiun");
    // Penjelasan: Menyimpan `content` dengan menunggu operasi asinkron `page.locator("#content").innerText()` selesai sebelum memakai hasilnya.
    const content = await page.locator("#content").innerText();
    // Penjelasan: Melakukan operasi dengan memanggil `expect(content.match(/\bpoin\b(?!\s+kebahagiaan\b)|\bpoin\s+kebahagiaan\s+kebahagiaan\b/gi) || []).toEqual([])` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(content.match(/\bpoin\b(?!\s+kebahagiaan\b)|\bpoin\s+kebahagiaan\s+kebahagiaan\b/gi) || []).toEqual([]);
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
    await expectNoHorizontalOverflow(page);
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Mengulang blok dengan pengaturan `const path of ["/", `/sessions/${advancedSessionId}`, "/rulebook"]`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
  for (const path of ["/", `/sessions/${advancedSessionId}`, "/rulebook"]) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(path)` selesai sebelum memakai hasilnya.
    await page.goto(path);
    // Penjelasan: Menyimpan `content` dengan menunggu operasi asinkron `page.locator("#content").innerText()` selesai sebelum memakai hasilnya.
    const content = await page.locator("#content").innerText();
    // Penjelasan: Melakukan operasi dengan memanggil `expect(content.match(/\bpoin\b(?!\s+kebahagiaan\b)/gi) || []).toEqual([])` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(content.match(/\bpoin\b(?!\s+kebahagiaan\b)/gi) || []).toEqual([]);
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
    await expectNoHorizontalOverflow(page);
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("peringkat dana pensiun ditampilkan sebagai posisi ke-4", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("peringkat dana pensiun ditampilkan sebagai posisi ke-4", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);
  // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const evidence = page.locator("#player-evidence-library");
  // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await evidence.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await evidence.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `pension` dengan memanggil `evidence.locator("details.player-evidence-domain").nth(6)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const pension = evidence.locator("details.player-evidence-domain").nth(6);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `pension.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
  await pension.locator(":scope > summary").click();
  // Penjelasan: Menyimpan `rankCard` dengan menyaring koleksi menggunakan predikat `{`; hanya elemen yang memenuhi kondisi masuk hasil.
  const rankCard = pension.locator(".player-metric-card").filter({
    // Penjelasan: Mengisi properti `has` pada objek atau konfigurasi dengan memanggil `page.getByRole("heading", { name: "Peringkat Dana Pensiun", exact: true })` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; konsumen objek membaca nilai ini melalui nama properti tersebut.
    has: page.getByRole("heading", { name: "Peringkat Dana Pensiun", exact: true })
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(rankCard.locator(".player-metric-card__value")).toHaveText("ke-4")` selesai sebelum memakai hasilnya.
  await expect(rankCard.locator(".player-metric-card__value")).toHaveText("ke-4");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(rankCard.locator(".player-metric-card__value small")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(rankCard.locator(".player-metric-card__value small")).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("rincian pesanan tersusun menjadi hasil tabel dan dua ringkasan", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("rincian pesanan tersusun menjadi hasil tabel dan dua ringkasan", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  // Penjelasan: Menyimpan `evidence` dengan memanggil `page.locator("#player-evidence-library")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const evidence = page.locator("#player-evidence-library");
  // Penjelasan: Memeriksa kondisi `(await evidence.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await evidence.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `evidence.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await evidence.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `orderDomain` dengan memanggil `page.locator("details.player-evidence-domain").nth(2)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const orderDomain = page.locator("details.player-evidence-domain").nth(2);
  // Penjelasan: Memeriksa kondisi `(await orderDomain.getAttribute("open")) === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if ((await orderDomain.getAttribute("open")) === null) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `orderDomain.locator(":scope > summary").click()` selesai sebelum memakai hasilnya.
    await orderDomain.locator(":scope > summary").click();
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `cards` dengan memanggil `orderDomain.locator(".player-metric-card")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const cards = orderDomain.locator(".player-metric-card");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards).toHaveCount(4)` selesai sebelum memakai hasilnya.
  await expect(cards).toHaveCount(4);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.locator("h5")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.locator("h5")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Pesanan Selesai"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Pesanan Selesai",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Pendapatan per Pesanan Makanan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Pendapatan per Pesanan Makanan",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Total Pendapatan Pesanan"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Total Pendapatan Pesanan",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Rata-rata Pesanan per Hari dengan Aksi Utama"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Rata-rata Pesanan per Hari Pemain Beraksi"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(1).locator("th")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.nth(1).locator("th")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"Pesanan ke-", "Pendapatan yang Dihasilkan (koin)"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "Pesanan ke-", "Pendapatan yang Dihasilkan (koin)"
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ]);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(1).locator("tbody tr")).toHaveText([` selesai sebelum memakai hasilnya.
  await expect(cards.nth(1).locator("tbody tr")).toHaveText([
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"1 13", "2 22", "3 22", "4 14", "5 20"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "1 13", "2 22", "3 22", "4 14", "5 20"
  // Penjelasan: Melakukan operasi dengan membaca nilai `], { useInnerText: true })` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
  ], { useInnerText: true });
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(1)).toContainText("sebelum dikurangi biaya bahan")` selesai sebelum memakai hasilnya.
  await expect(cards.nth(1)).toContainText("sebelum dikurangi biaya bahan");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(cards.nth(1)).toContainText("pesanan pertama yang diselesaikan pemain, bukan nomor kartu pesanan")` selesai sebelum memakai hasilnya.
  await expect(cards.nth(1)).toContainText("pesanan pertama yang diselesaikan pemain, bukan nomor kartu pesanan");

  // Penjelasan: Menyimpan `layout` dengan mentransformasikan setiap anggota koleksi dengan `item => {` untuk membentuk array hasil yang urutannya mengikuti sumber. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const layout = await cards.evaluateAll(items => items.map(item => {
    // Penjelasan: Menyimpan `box` dengan memanggil `item.getBoundingClientRect()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const box = item.getBoundingClientRect();
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `{ width: box.width, y: box.y }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
    return { width: box.width, y: box.y };
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  }));
  // Penjelasan: Menyimpan `gridContentWidth` dengan menunggu operasi asinkron `orderDomain.locator(".player-metric-card-grid").evaluate(grid => {` selesai sebelum memakai hasilnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const gridContentWidth = await orderDomain.locator(".player-metric-card-grid").evaluate(grid => {
    // Penjelasan: Menyimpan `style` dengan memanggil `getComputedStyle(grid)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const style = getComputedStyle(grid);
    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan memanggil `grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; eksekusi fungsi berakhir setelah nilai dihitung.
    return grid.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[1].y).toBeGreaterThan(layout[0].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[1].y).toBeGreaterThan(layout[0].y);
  // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].y).toBeGreaterThan(layout[1].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  expect(layout[2].y).toBeGreaterThan(layout[1].y);
  // Penjelasan: Memeriksa kondisi `page.viewportSize().width >= 768`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (page.viewportSize().width >= 768) {
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[0].width).toBeGreaterThan(gridContentWidth * 0.98)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[0].width).toBeGreaterThan(gridContentWidth * 0.98);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[1].width).toBeGreaterThan(gridContentWidth * 0.98)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[1].width).toBeGreaterThan(gridContentWidth * 0.98);
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[2].y).toBe(layout[3].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[2].y).toBe(layout[3].y);
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.slice(2).every(item => item.width > gridContentWidth * 0.45)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(layout.slice(2).every(item => item.width > gridContentWidth * 0.45)).toBeTruthy();
  // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
  } else {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `expect(layout.every(item => item.width > gridContentWidth * 0.98)).toBeTruthy();`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    expect(layout.every(item => item.width > gridContentWidth * 0.98)).toBeTruthy();
    // Penjelasan: Melakukan operasi dengan memanggil `expect(layout[3].y).toBeGreaterThan(layout[2].y)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    expect(layout[3].y).toBeGreaterThan(layout[2].y);
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("halaman pemain tidak menampilkan riwayat aktivitas keuangan terpisah", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("halaman pemain tidak menampilkan riwayat aktivitas keuangan terpisah", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}/players/${playerId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}/players/${playerId}`);

  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator("#player-transaction-history")).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.locator("#player-transaction-history")).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.getByText("Riwayat Aktivitas Keuangan", { exact: true })).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.getByText("Riwayat Aktivitas Keuangan", { exact: true })).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator("#player-statistics-dashboard")).toBeVisible()` selesai sebelum memakai hasilnya.
  await expect(page.locator("#player-statistics-dashboard")).toBeVisible();
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("rincian sesi tidak menampilkan kartu pelanggaran aturan", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("rincian sesi tidak menampilkan kartu pelanggaran aturan", async ({ page }) => {
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}`)` selesai sebelum memakai hasilnya.
  await page.goto(`/sessions/${advancedSessionId}`);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.getByText(/pelanggaran aturan|rule violations/i)).toHaveCount(0)` selesai sebelum memakai hasilnya.
  await expect(page.getByText(/pelanggaran aturan|rule violations/i)).toHaveCount(0);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator('.work-calendar-cell[data-day="25"] .work-calendar-cell__number')).toHaveText("25")` selesai sebelum memakai hasilnya.
  await expect(page.locator('.work-calendar-cell[data-day="25"] .work-calendar-cell__number')).toHaveText("25");
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator('.work-calendar-cell--finish[data-day="26"]')).toContainText(/selesai|finish/i)` selesai sebelum memakai hasilnya.
  await expect(page.locator('.work-calendar-cell--finish[data-day="26"]')).toContainText(/selesai|finish/i);
  // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
  await expectNoHorizontalOverflow(page);
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});

// Penjelasan: Mendefinisikan fungsi atau pemetaan `test("daftar pemain tetap terbaca tanpa overflow pada semua ukuran utama", async ({ page }) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
test("daftar pemain tetap terbaca tanpa overflow pada semua ukuran utama", async ({ page }) => {
  // Penjelasan: Menyimpan `viewports` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
  const viewports = [
    // Penjelasan: Melakukan operasi dengan membaca nilai `{ width: 320, height: 800, mobile: true }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
    { width: 320, height: 800, mobile: true },
    // Penjelasan: Melakukan operasi dengan membaca nilai `{ width: 768, height: 1024, mobile: true }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
    { width: 768, height: 1024, mobile: true },
    // Penjelasan: Melakukan operasi dengan membaca nilai `{ width: 1024, height: 768, mobile: false }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
    { width: 1024, height: 768, mobile: false },
    // Penjelasan: Melakukan operasi dengan membaca nilai `{ width: 1440, height: 900, mobile: false }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
    { width: 1440, height: 900, mobile: false }
  // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  ];

  // Penjelasan: Mengulang blok dengan pengaturan `const viewport of viewports`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
  for (const viewport of viewports) {
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.setViewportSize({ width: viewport.width, height: viewport.height })` selesai sebelum memakai hasilnya.
    await page.setViewportSize({ width: viewport.width, height: viewport.height });
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `page.goto(`/sessions/${advancedSessionId}`)` selesai sebelum memakai hasilnya.
    await page.goto(`/sessions/${advancedSessionId}`);

    // Penjelasan: Menyimpan `desktopTable` dengan memanggil `page.locator(".happiness-score-desktop")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const desktopTable = page.locator(".happiness-score-desktop");
    // Penjelasan: Menyimpan `mobileCards` dengan memanggil `page.locator(".happiness-score-mobile")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    const mobileCards = page.locator(".happiness-score-mobile");
    // Penjelasan: Memeriksa kondisi `viewport.mobile`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (viewport.mobile) {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(desktopTable).toBeHidden()` selesai sebelum memakai hasilnya.
      await expect(desktopTable).toBeHidden();
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(mobileCards).toBeVisible()` selesai sebelum memakai hasilnya.
      await expect(mobileCards).toBeVisible();
    // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
    } else {
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(desktopTable).toBeVisible()` selesai sebelum memakai hasilnya.
      await expect(desktopTable).toBeVisible();
      // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(mobileCards).toBeHidden()` selesai sebelum memakai hasilnya.
      await expect(mobileCards).toBeHidden();
      // Penjelasan: Menyimpan `playerColumnColors` dengan menunggu operasi asinkron `page.locator(".happiness-score-table tbody tr:first-child td").evaluateAll(` selesai sebelum memakai hasilnya.
      const playerColumnColors = await page.locator(".happiness-score-table tbody tr:first-child td").evaluateAll(
        // Penjelasan: Memperbarui `cells` dengan mentransformasikan setiap anggota koleksi dengan `cell => getComputedStyle(cell).backgroundColor)` untuk membentuk array hasil yang urutannya mengikuti sumber.
        cells => cells.map(cell => getComputedStyle(cell).backgroundColor)
      // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
      );
      // Penjelasan: Melakukan operasi dengan membuat instance `Set` dengan masukan `playerColumnColors).size).toBe(4`.
      expect(new Set(playerColumnColors).size).toBe(4);
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expect(page.locator(".happiness-score-mobile-card")).toHaveCount(4)` selesai sebelum memakai hasilnya.
    await expect(page.locator(".happiness-score-mobile-card")).toHaveCount(4);
    // Penjelasan: Menyimpan `playerCardColors` dengan menunggu operasi asinkron `page.locator(".happiness-score-mobile-card").evaluateAll(` selesai sebelum memakai hasilnya.
    const playerCardColors = await page.locator(".happiness-score-mobile-card").evaluateAll(
      // Penjelasan: Memperbarui `cards` dengan mentransformasikan setiap anggota koleksi dengan `card => getComputedStyle(card).backgroundColor)` untuk membentuk array hasil yang urutannya mengikuti sumber.
      cards => cards.map(card => getComputedStyle(card).backgroundColor)
    // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    );
    // Penjelasan: Melakukan operasi dengan membuat instance `Set` dengan masukan `playerCardColors).size).toBe(4`.
    expect(new Set(playerCardColors).size).toBe(4);
    // Penjelasan: Melakukan operasi dengan menunggu operasi asinkron `expectNoHorizontalOverflow(page)` selesai sebelum memakai hasilnya.
    await expectNoHorizontalOverflow(page);
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});
