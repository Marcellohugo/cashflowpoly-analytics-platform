// Fungsi file: Menggambar chart metric pada halaman detail pemain dari payload data-chart.
// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
            // Penjelasan: Menyimpan `chartNodes` dengan mencari seluruh elemen DOM yang cocok dengan selector `".js-metric-line-chart[data-chart]")`; daftar ini menjadi sasaran perilaku antarmuka.
            const chartNodes = Array.from(document.querySelectorAll(".js-metric-line-chart[data-chart]"));
            // Penjelasan: Memeriksa kondisi `!chartNodes.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
            if (!chartNodes.length) {
                // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                return;
            // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
            }

            // Penjelasan: Menyimpan `ns` dengan menggunakan literal `"http://www.w3.org/2000/svg"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
            const ns = "http://www.w3.org/2000/svg";
            // Penjelasan: Menyimpan `seriesPalette` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
            const seriesPalette = [
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#1ba784"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#1ba784",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#2d7dd2"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#2d7dd2",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#f4a84a"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#f4a84a",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#ef4e4e"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#ef4e4e",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#6a5acd"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#6a5acd",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#00a6a6"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#00a6a6",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#b56576"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#b56576",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#5f8f00"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#5f8f00"
            // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            ];
            // Penjelasan: Menyimpan `variablePalette` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
            const variablePalette = [
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#1ba784"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#1ba784",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#2d7dd2"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#2d7dd2",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#f4a84a"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#f4a84a",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#ef4e4e"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#ef4e4e",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#6a5acd"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#6a5acd",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#00a6a6"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#00a6a6",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#ff7f50"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#ff7f50",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#7f5af0"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#7f5af0",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#16a34a"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#16a34a",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#d97706"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#d97706",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#0284c7"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#0284c7",
                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"#be123c"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                "#be123c"
            // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            ];
            // Penjelasan: Menyimpan `colorForSeries` dengan menghitung ekspresi `(index) => seriesPalette[index % seriesPalette.length]` dengan urutan operator untuk memperoleh nilai turunan dari data masukan. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const colorForSeries = (index) => seriesPalette[index % seriesPalette.length];
            // Penjelasan: Menyimpan `colorForVariable` dengan menghitung ekspresi `(index) => variablePalette[index % variablePalette.length]` dengan urutan operator untuk memperoleh nilai turunan dari data masukan. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const colorForVariable = (index) => variablePalette[index % variablePalette.length];
            // Penjelasan: Menyimpan `runtimeConfig` dengan membaca nilai `window.cashflowpolyPlayerDetailCharts || {}` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
            const runtimeConfig = window.cashflowpolyPlayerDetailCharts || {};
            // Penjelasan: Menyimpan `tooltipText` dengan menyalin properti sumber ke objek target; sumber yang lebih akhir menimpa nilai bawaan dengan konfigurasi yang tersedia.
            const tooltipText = Object.assign({
                // Penjelasan: Mengisi properti `metric` pada objek atau konfigurasi dengan menggunakan literal `"Metric"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                metric: "Metric",
                // Penjelasan: Mengisi properti `series` pada objek atau konfigurasi dengan menggunakan literal `"Series"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                series: "Series",
                // Penjelasan: Mengisi properti `points` pada objek atau konfigurasi dengan menggunakan literal `"Points"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                points: "Points",
                // Penjelasan: Mengisi properti `formula` pada objek atau konfigurasi dengan menggunakan literal `"Formula"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                formula: "Formula",
                // Penjelasan: Mengisi properti `defaultFormula` pada objek atau konfigurasi dengan menggunakan literal `"Source calculation not available"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                defaultFormula: "Source calculation not available",
                // Penjelasan: Mengisi properti `selectedDetail` pada objek atau konfigurasi dengan menggunakan literal `"Selected detail"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                selectedDetail: "Selected detail",
                // Penjelasan: Mengisi properti `tapHint` pada objek atau konfigurasi dengan menggunakan literal `"Tap a point or bar to inspect details."` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                tapHint: "Tap a point or bar to inspect details.",
                // Penjelasan: Mengisi properti `itemPrefix` pada objek atau konfigurasi dengan menggunakan literal `"Item"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                itemPrefix: "Item"
            // Penjelasan: Melakukan operasi dengan membaca nilai `}, runtimeConfig.tooltipText || {})` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
            }, runtimeConfig.tooltipText || {});
            // Penjelasan: Menyimpan `chartStatusText` dengan menyalin properti sumber ke objek target; sumber yang lebih akhir menimpa nilai bawaan dengan konfigurasi yang tersedia.
            const chartStatusText = Object.assign({
                // Penjelasan: Mengisi properti `noSeries` pada objek atau konfigurasi dengan menggunakan literal `"No chart series available."` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                noSeries: "No chart series available.",
                // Penjelasan: Mengisi properti `noPoints` pada objek atau konfigurasi dengan menggunakan literal `"No numeric points available."` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                noPoints: "No numeric points available.",
                // Penjelasan: Mengisi properti `invalidPayload` pada objek atau konfigurasi dengan menggunakan literal `"Invalid chart payload."` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                invalidPayload: "Invalid chart payload."
            // Penjelasan: Melakukan operasi dengan membaca nilai `}, runtimeConfig.chartStatusText || {})` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
            }, runtimeConfig.chartStatusText || {});
            // Penjelasan: Menyimpan `formatMetricValue` dengan membaca nilai `(value) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const formatMetricValue = (value) => {
                // Penjelasan: Memeriksa kondisi `typeof value !== "number" || !Number.isFinite(value)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (typeof value !== "number" || !Number.isFinite(value)) {
                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menggunakan literal `"-"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; eksekusi fungsi berakhir setelah nilai dihitung.
                    return "-";
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Menyimpan `abs` dengan menghitung besar absolut `value` tanpa tanda negatif untuk keputusan format atau skala.
                const abs = Math.abs(value);
                // Penjelasan: Menyimpan `decimals` dengan memilih nilai melalui kondisi ternary `abs >= 100 ? 0 : abs >= 10 ? 1 : 2`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
                const decimals = abs >= 100 ? 0 : abs >= 10 ? 1 : 2;
                // Penjelasan: Mengembalikan hasil kepada pemanggil dengan memformat nilai dengan aturan lokal dan opsi `undefined, {`; pemisah ribuan dan desimal mengikuti locale yang dipilih; eksekusi fungsi berakhir setelah nilai dihitung.
                return value.toLocaleString(undefined, {
                    // Penjelasan: Mengisi properti `minimumFractionDigits` pada objek atau konfigurasi dengan menggunakan konstanta numerik `0` sebagai nilai awal atau parameter perhitungan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    minimumFractionDigits: 0,
                    // Penjelasan: Mengisi properti `maximumFractionDigits` pada objek atau konfigurasi dengan membaca nilai `decimals` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    maximumFractionDigits: decimals
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                });
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };
            // Penjelasan: Menyimpan `makeHtmlNode` dengan membaca nilai `(name, className = "") => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const makeHtmlNode = (name, className = "") => {
                // Penjelasan: Menyimpan `node` dengan membuat elemen HTML `name` yang selanjutnya diberi isi, kelas, dan ditempelkan ke dokumen.
                const node = document.createElement(name);
                // Penjelasan: Memeriksa kondisi `className.length > 0`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (className.length > 0) {
                    // Penjelasan: Memperbarui `node.className` dengan membaca nilai `className` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    node.className = className;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
                // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `node` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                return node;
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };
            // Penjelasan: Menyimpan `ensureBarInsightPanel` dengan membaca nilai `(hostCard, detailLabel) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const ensureBarInsightPanel = (hostCard, detailLabel) => {
                // Penjelasan: Memeriksa kondisi `!hostCard`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (!hostCard) {
                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menggunakan null sebagai penanda bahwa objek atau pilihan belum tersedia; eksekusi fungsi berakhir setelah nilai dihitung.
                    return null;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Menyimpan `resolvedDetailLabel` dengan membaca nilai `` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                const resolvedDetailLabel =
                    // Penjelasan: Melakukan operasi dengan menghapus spasi pada awal dan akhir string sebelum nilai diperiksa atau ditampilkan.
                    typeof detailLabel === "string" && detailLabel.trim().length > 0
                        // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `detailLabel.trim()`; nilai ini menjadi keluaran ekspresi pilihan.
                        ? detailLabel.trim()
                        // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `tooltipText.formula;`; nilai cadangan ini melengkapi ekspresi pilihan.
                        : tooltipText.formula;

                // Penjelasan: Menyimpan `panel` dengan mencari turunan pertama dengan selector `".js-chart-bar-insight"`; pemanggil memeriksa ketersediaannya sebelum mengubah tampilan.
                let panel = hostCard.querySelector(".js-chart-bar-insight");
                // Penjelasan: Memeriksa kondisi `!panel`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (!panel) {
                    // Penjelasan: Memperbarui `panel` dengan memanggil `makeHtmlNode("div", "chart-bar-insight js-chart-bar-insight")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    panel = makeHtmlNode("div", "chart-bar-insight js-chart-bar-insight");
                    // Penjelasan: Memperbarui `panel.hidden` dengan mengaktifkan flag Boolean dengan nilai true.
                    panel.hidden = true;
                    // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"aria-live", "polite"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
                    panel.setAttribute("aria-live", "polite");

                    // Penjelasan: Menyimpan `title` dengan memanggil `makeHtmlNode("p", "chart-bar-insight-title")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    const title = makeHtmlNode("p", "chart-bar-insight-title");
                    // Penjelasan: Memperbarui `title.textContent` dengan membaca nilai `tooltipText.selectedDetail` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    title.textContent = tooltipText.selectedDetail;
                    // Penjelasan: Melakukan operasi dengan menempatkan node `title` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                    panel.appendChild(title);

                    // Penjelasan: Menyimpan `hint` dengan memanggil `makeHtmlNode("p", "chart-bar-insight-hint")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    const hint = makeHtmlNode("p", "chart-bar-insight-hint");
                    // Penjelasan: Memperbarui `hint.textContent` dengan membaca nilai `tooltipText.tapHint` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    hint.textContent = tooltipText.tapHint;
                    // Penjelasan: Melakukan operasi dengan menempatkan node `hint` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                    panel.appendChild(hint);

                    // Penjelasan: Menyimpan `makeRow` dengan membaca nilai `(label, keyClass, valueClass) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                    const makeRow = (label, keyClass, valueClass) => {
                        // Penjelasan: Menyimpan `row` dengan memanggil `makeHtmlNode("div", "chart-bar-insight-row")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const row = makeHtmlNode("div", "chart-bar-insight-row");
                        // Penjelasan: Menyimpan `keyClassName` dengan membaca nilai `keyClass && keyClass.length > 0` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        const keyClassName = keyClass && keyClass.length > 0
                            // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu ``chart-bar-insight-key ${keyClass}``; nilai ini menjadi keluaran ekspresi pilihan.
                            ? `chart-bar-insight-key ${keyClass}`
                            // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `"chart-bar-insight-key";`; nilai cadangan ini melengkapi ekspresi pilihan.
                            : "chart-bar-insight-key";
                        // Penjelasan: Menyimpan `key` dengan memanggil `makeHtmlNode("span", keyClassName)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const key = makeHtmlNode("span", keyClassName);
                        // Penjelasan: Memperbarui `key.textContent` dengan membaca nilai `label` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        key.textContent = label;
                        // Penjelasan: Menyimpan `value` dengan memanggil `makeHtmlNode("span", `chart-bar-insight-value ${valueClass}`)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const value = makeHtmlNode("span", `chart-bar-insight-value ${valueClass}`);
                        // Penjelasan: Memperbarui `value.textContent` dengan menggunakan literal `"-"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                        value.textContent = "-";
                        // Penjelasan: Melakukan operasi dengan menempatkan node `key` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                        row.appendChild(key);
                        // Penjelasan: Melakukan operasi dengan menempatkan node `value` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                        row.appendChild(value);
                        // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `row` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                        return row;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    };

                    // Penjelasan: Melakukan operasi dengan memanggil `panel.appendChild(makeRow(` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    panel.appendChild(makeRow(
                        // Penjelasan: Melakukan operasi dengan membaca nilai `tooltipText.metric` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        tooltipText.metric,
                        // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"js-chart-bar-insight-metric-key"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                        "js-chart-bar-insight-metric-key",
                        // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"js-chart-bar-insight-metric"))` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                        "js-chart-bar-insight-metric"));
                    // Penjelasan: Melakukan operasi dengan memanggil `panel.appendChild(makeRow(` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    panel.appendChild(makeRow(
                        // Penjelasan: Melakukan operasi dengan membaca nilai `tooltipText.points` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        tooltipText.points,
                        // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"js-chart-bar-insight-points-key"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                        "js-chart-bar-insight-points-key",
                        // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"js-chart-bar-insight-points"))` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                        "js-chart-bar-insight-points"));
                    // Penjelasan: Melakukan operasi dengan memanggil `panel.appendChild(makeRow(` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    panel.appendChild(makeRow(
                        // Penjelasan: Melakukan operasi dengan membaca nilai `resolvedDetailLabel` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        resolvedDetailLabel,
                        // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"js-chart-bar-insight-detail-key"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                        "js-chart-bar-insight-detail-key",
                        // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"js-chart-bar-insight-formula"))` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                        "js-chart-bar-insight-formula"));

                    // Penjelasan: Melakukan operasi dengan menempatkan node `panel` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                    hostCard.appendChild(panel);
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Menyimpan `detailKey` dengan mencari turunan pertama dengan selector `".js-chart-bar-insight-detail-key"`; pemanggil memeriksa ketersediaannya sebelum mengubah tampilan.
                const detailKey = panel.querySelector(".js-chart-bar-insight-detail-key");
                // Penjelasan: Memeriksa kondisi `detailKey`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (detailKey) {
                    // Penjelasan: Memperbarui `detailKey.textContent` dengan membaca nilai `resolvedDetailLabel` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    detailKey.textContent = resolvedDetailLabel;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya; eksekusi fungsi berakhir setelah nilai dihitung.
                return {
                    // Penjelasan: Melakukan operasi dengan membaca nilai `panel` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    panel,
                    // Penjelasan: Mengisi properti `metric` pada objek atau konfigurasi dengan mencari turunan pertama dengan selector `".js-chart-bar-insight-metric"`; pemanggil memeriksa ketersediaannya sebelum mengubah tampilan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    metric: panel.querySelector(".js-chart-bar-insight-metric"),
                    // Penjelasan: Mengisi properti `points` pada objek atau konfigurasi dengan mencari turunan pertama dengan selector `".js-chart-bar-insight-points"`; pemanggil memeriksa ketersediaannya sebelum mengubah tampilan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    points: panel.querySelector(".js-chart-bar-insight-points"),
                    // Penjelasan: Mengisi properti `formula` pada objek atau konfigurasi dengan mencari turunan pertama dengan selector `".js-chart-bar-insight-formula"`; pemanggil memeriksa ketersediaannya sebelum mengubah tampilan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    formula: panel.querySelector(".js-chart-bar-insight-formula"),
                    // Penjelasan: Melakukan operasi dengan membaca nilai `detailKey` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    detailKey
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                };
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };
            // Penjelasan: Menyimpan `revealBarInsight` dengan membaca nilai `(insight, detail) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const revealBarInsight = (insight, detail) => {
                // Penjelasan: Memeriksa kondisi `!insight`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (!insight) {
                    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                    return;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Memperbarui `insight.panel.hidden` dengan menonaktifkan flag Boolean dengan nilai false.
                insight.panel.hidden = false;
                // Penjelasan: Memeriksa kondisi `insight.metric`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (insight.metric) {
                    // Penjelasan: Memperbarui `insight.metric.textContent` dengan membaca nilai `detail.metric` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    insight.metric.textContent = detail.metric;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
                // Penjelasan: Memeriksa kondisi `insight.points`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (insight.points) {
                    // Penjelasan: Memperbarui `insight.points.textContent` dengan membaca nilai `detail.points` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    insight.points.textContent = detail.points;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
                // Penjelasan: Memeriksa kondisi `insight.formula`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (insight.formula) {
                    // Penjelasan: Memperbarui `insight.formula.textContent` dengan membaca nilai `detail.formula` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    insight.formula.textContent = detail.formula;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Menyimpan `makeNode` dengan membaca nilai `(name, attrs = {}) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const makeNode = (name, attrs = {}) => {
                // Penjelasan: Menyimpan `node` dengan membuat elemen dengan namespace dan nama `ns, name`; namespace SVG diperlukan agar browser menggambar bentuk grafik dengan benar.
                const node = document.createElementNS(ns, name);
                // Penjelasan: Mendefinisikan fungsi atau pemetaan `Object.entries(attrs).forEach(([key, value]) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                Object.entries(attrs).forEach(([key, value]) => {
                    // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `key, String(value)`; atribut mengendalikan presentasi atau aksesibilitas komponen.
                    node.setAttribute(key, String(value));
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                });
                // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `node` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                return node;
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Menyimpan `clearNode` dengan membaca nilai `(node) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const clearNode = (node) => {
                // Penjelasan: Mengulang blok selama kondisi `node.firstChild` tetap benar; nilai yang diperiksa diperbarui oleh badan perulangan.
                while (node.firstChild) {
                    // Penjelasan: Melakukan operasi dengan memanggil `node.removeChild(node.firstChild)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    node.removeChild(node.firstChild);
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Menyimpan `drawEmpty` dengan membaca nilai `(svg, message) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const drawEmpty = (svg, message) => {
                // Penjelasan: Melakukan operasi dengan memanggil `clearNode(svg)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                clearNode(svg);
                // Penjelasan: Menyimpan `vb` dengan memilih nilai melalui kondisi ternary `svg.viewBox && svg.viewBox.baseVal ? svg.viewBox.baseVal : null`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
                const vb = svg.viewBox && svg.viewBox.baseVal ? svg.viewBox.baseVal : null;
                // Penjelasan: Menyimpan `width` dengan memilih nilai melalui kondisi ternary `vb && vb.width ? vb.width : 840`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
                const width = vb && vb.width ? vb.width : 840;
                // Penjelasan: Menyimpan `height` dengan memilih nilai melalui kondisi ternary `vb && vb.height ? vb.height : 360`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
                const height = vb && vb.height ? vb.height : 360;
                // Penjelasan: Menyimpan `text` dengan memanggil `makeNode("text", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                const text = makeNode("text", {
                    // Penjelasan: Mengisi properti `x` pada objek atau konfigurasi dengan menghitung ekspresi `width / 2` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    x: width / 2,
                    // Penjelasan: Mengisi properti `y` pada objek atau konfigurasi dengan menghitung ekspresi `height / 2` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    y: height / 2,
                    // Penjelasan: Mengisi properti `"text-anchor"` pada objek atau konfigurasi dengan menggunakan literal `"middle"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    "text-anchor": "middle",
                    // Penjelasan: Mengisi properti `"dominant-baseline"` pada objek atau konfigurasi dengan menggunakan literal `"middle"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    "dominant-baseline": "middle",
                    // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan menggunakan literal `"#4f6e83"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    fill: "#4f6e83",
                    // Penjelasan: Mengisi properti `"font-size"` pada objek atau konfigurasi dengan menggunakan literal `"13"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    "font-size": "13",
                    // Penjelasan: Mengisi properti `"font-family"` pada objek atau konfigurasi dengan menggunakan literal `"Nunito, Segoe UI, sans-serif"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    "font-family": "Nunito, Segoe UI, sans-serif"
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                });
                // Penjelasan: Memperbarui `text.textContent` dengan membaca nilai `message` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                text.textContent = message;
                // Penjelasan: Melakukan operasi dengan menempatkan node `text` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                svg.appendChild(text);
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Menyimpan `buildPath` dengan membaca nilai `(points) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const buildPath = (points) => {
                // Penjelasan: Memeriksa kondisi `!points.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (!points.length) {
                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menggunakan literal `""` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; eksekusi fungsi berakhir setelah nilai dihitung.
                    return "";
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Menyimpan `path` dengan menggunakan literal ``M ${points[0].x} ${points[0].y}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                let path = `M ${points[0].x} ${points[0].y}`;
                // Penjelasan: Mengulang blok dengan pengaturan `let index = 1; index < points.length; index += 1`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
                for (let index = 1; index < points.length; index += 1) {
                    // Penjelasan: Memperbarui `path` dengan menggunakan literal `` L ${points[index].x} ${points[index].y}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; operator `+=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                    path += ` L ${points[index].x} ${points[index].y}`;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
                // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `path` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                return path;
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Menyimpan `normalizeLabel` dengan membaca nilai `(value, index) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const normalizeLabel = (value, index) => {
                // Penjelasan: Menyimpan `text` dengan mengevaluasi `String(value ?? "")`; operator ?? memakai nilai cadangan hanya ketika sisi kiri null atau undefined.
                const text = String(value ?? "")
                    // Penjelasan: Melanjutkan rantai operasi dengan mengganti bagian teks berdasarkan pola dan pengganti `/[_-]+/g, " "`; hasil tahap sebelumnya menjadi sumber tahap ini.
                    .replace(/[_-]+/g, " ")
                    // Penjelasan: Melanjutkan rantai operasi dengan mengganti bagian teks berdasarkan pola dan pengganti `/\s+/g, " "`; hasil tahap sebelumnya menjadi sumber tahap ini.
                    .replace(/\s+/g, " ")
                    // Penjelasan: Melanjutkan rantai operasi dengan menghapus spasi pada awal dan akhir string sebelum nilai diperiksa atau ditampilkan; hasil tahap sebelumnya menjadi sumber tahap ini.
                    .trim();

                // Penjelasan: Memeriksa kondisi `text.length > 0`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (text.length > 0) {
                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `text` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                    return text;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menggunakan literal ``${tooltipText.itemPrefix} ${index + 1}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; eksekusi fungsi berakhir setelah nilai dihitung.
                return `${tooltipText.itemPrefix} ${index + 1}`;
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Menyimpan `makeUniqueLabels` dengan membaca nilai `(labels) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const makeUniqueLabels = (labels) => {
                // Penjelasan: Menyimpan `usageMap` dengan membuat instance `Map` dengan masukan `tanpa argumen`.
                const usageMap = new Map();
                // Penjelasan: Mengembalikan hasil kepada pemanggil dengan mentransformasikan setiap anggota koleksi dengan `(label) => {` untuk membentuk array hasil yang urutannya mengikuti sumber; eksekusi fungsi berakhir setelah nilai dihitung.
                return labels.map((label) => {
                    // Penjelasan: Menyimpan `key` dengan menormalkan string menjadi huruf kecil sehingga pencocokan kunci tidak bergantung kapitalisasi masukan.
                    const key = String(label ?? "").toLowerCase();
                    // Penjelasan: Menyimpan `nextCount` dengan memanggil `(usageMap.get(key) || 0) + 1` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    const nextCount = (usageMap.get(key) || 0) + 1;
                    // Penjelasan: Melakukan operasi dengan memanggil `usageMap.set(key, nextCount)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    usageMap.set(key, nextCount);
                    // Penjelasan: Memeriksa kondisi `nextCount === 1`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                    if (nextCount === 1) {
                        // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `label` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                        return label;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                    }

                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menggunakan literal ``${label} (${nextCount})`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; eksekusi fungsi berakhir setelah nilai dihitung.
                    return `${label} (${nextCount})`;
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                });
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Menyimpan `wrapAxisLabel` dengan membaca nilai `(label, maxCharsPerLine = 16, maxLines = 2) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const wrapAxisLabel = (label, maxCharsPerLine = 16, maxLines = 2) => {
                // Penjelasan: Menyimpan `compactLabel` dengan mengganti bagian teks berdasarkan pola dan pengganti `/\s+/g, " ").trim(`.
                const compactLabel = String(label ?? "").replace(/\s+/g, " ").trim();
                // Penjelasan: Memeriksa kondisi `!compactLabel.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (!compactLabel.length) {
                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `["-"]` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                    return ["-"];
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Menyimpan `words` dengan memanggil `compactLabel.split(" ")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                const words = compactLabel.split(" ");
                // Penjelasan: Menyimpan `lines` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
                const lines = [];
                // Penjelasan: Menyimpan `currentLine` dengan menggunakan literal `""` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                let currentLine = "";

                // Penjelasan: Mendefinisikan fungsi atau pemetaan `words.forEach((word) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                words.forEach((word) => {
                    // Penjelasan: Menyimpan `chunks` dengan membaca nilai `word.length > maxCharsPerLine` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    const chunks = word.length > maxCharsPerLine
                        // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `word.match(new RegExp(`.{1,${maxCharsPerLine}}`, "g")) || [word]`; nilai ini menjadi keluaran ekspresi pilihan.
                        ? word.match(new RegExp(`.{1,${maxCharsPerLine}}`, "g")) || [word]
                        // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `[word];`; nilai cadangan ini melengkapi ekspresi pilihan.
                        : [word];

                    // Penjelasan: Mendefinisikan fungsi atau pemetaan `chunks.forEach((chunk) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                    chunks.forEach((chunk) => {
                        // Penjelasan: Menyimpan `candidate` dengan membaca nilai `currentLine.length > 0` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        const candidate = currentLine.length > 0
                            // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu ``${currentLine} ${chunk}``; nilai ini menjadi keluaran ekspresi pilihan.
                            ? `${currentLine} ${chunk}`
                            // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `chunk;`; nilai cadangan ini melengkapi ekspresi pilihan.
                            : chunk;

                        // Penjelasan: Memeriksa kondisi `candidate.length <= maxCharsPerLine`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                        if (candidate.length <= maxCharsPerLine) {
                            // Penjelasan: Memperbarui `currentLine` dengan membaca nilai `candidate` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            currentLine = candidate;
                            // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                            return;
                        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                        }

                        // Penjelasan: Memeriksa kondisi `currentLine.length > 0`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                        if (currentLine.length > 0) {
                            // Penjelasan: Melakukan operasi dengan menambahkan `currentLine)` ke akhir array untuk membentuk kumpulan data atau markup dalam urutan yang benar.
                            lines.push(currentLine);
                        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                        }
                        // Penjelasan: Memperbarui `currentLine` dengan membaca nilai `chunk` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        currentLine = chunk;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    });
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                });

                // Penjelasan: Memeriksa kondisi `currentLine.length > 0`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (currentLine.length > 0) {
                    // Penjelasan: Melakukan operasi dengan menambahkan `currentLine)` ke akhir array untuk membentuk kumpulan data atau markup dalam urutan yang benar.
                    lines.push(currentLine);
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Memeriksa kondisi `lines.length <= maxLines`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (lines.length <= maxLines) {
                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `lines` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                    return lines;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Menyimpan `trimmedLines` dengan memanggil `lines.slice(0, maxLines)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                const trimmedLines = lines.slice(0, maxLines);
                // Penjelasan: Menyimpan `lastIndex` dengan menghitung ekspresi `maxLines - 1` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                const lastIndex = maxLines - 1;
                // Penjelasan: Menyimpan `lastLine` dengan membaca nilai `trimmedLines[lastIndex]` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                const lastLine = trimmedLines[lastIndex];
                // Penjelasan: Memeriksa kondisi `lastLine.length >= maxCharsPerLine - 1`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (lastLine.length >= maxCharsPerLine - 1) {
                    // Penjelasan: Memperbarui `trimmedLines[lastIndex]` dengan mengambil nilai terbesar dari `1, maxCharsPerLine - 1)).trim(` untuk menjaga hasil tidak di bawah batas minimum.
                    trimmedLines[lastIndex] = `${lastLine.slice(0, Math.max(1, maxCharsPerLine - 1)).trim()}...`;
                // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
                } else {
                    // Penjelasan: Memperbarui `trimmedLines[lastIndex]` dengan menggunakan literal ``${lastLine}...`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                    trimmedLines[lastIndex] = `${lastLine}...`;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `trimmedLines` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                return trimmedLines;
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Menyimpan `drawChart` dengan membaca nilai `(svg, payload) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
            const drawChart = (svg, payload) => {
                // Penjelasan: Menyimpan `normalizedLabels` dengan membaca `Array.isArray(payload?.labels)` dengan akses opsional sehingga properti pada objek kosong tidak menyebabkan kegagalan.
                const normalizedLabels = Array.isArray(payload?.labels)
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `payload.labels.map((value, index) => normalizeLabel(value, index))`; nilai ini menjadi keluaran ekspresi pilihan.
                    ? payload.labels.map((value, index) => normalizeLabel(value, index))
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `[];`; nilai cadangan ini melengkapi ekspresi pilihan.
                    : [];
                // Penjelasan: Menyimpan `labels` dengan memanggil `makeUniqueLabels(normalizedLabels)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                const labels = makeUniqueLabels(normalizedLabels);
                // Penjelasan: Menyimpan `chartType` dengan menormalkan string menjadi huruf kecil sehingga pencocokan kunci tidak bergantung kapitalisasi masukan.
                const chartType = String(payload?.chartType ?? "line").toLowerCase();
                // Penjelasan: Menyimpan `metricKeys` dengan membaca `Array.isArray(payload?.keys)` dengan akses opsional sehingga properti pada objek kosong tidak menyebabkan kegagalan.
                const metricKeys = Array.isArray(payload?.keys)
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `payload.keys.map((value) => String(value ?? ""))`; nilai ini menjadi keluaran ekspresi pilihan.
                    ? payload.keys.map((value) => String(value ?? ""))
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `[];`; nilai cadangan ini melengkapi ekspresi pilihan.
                    : [];
                // Penjelasan: Menyimpan `formulaHints` dengan membaca `Array.isArray(payload?.formulas)` dengan akses opsional sehingga properti pada objek kosong tidak menyebabkan kegagalan.
                const formulaHints = Array.isArray(payload?.formulas)
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `payload.formulas.map((value) => String(value ?? "").trim())`; nilai ini menjadi keluaran ekspresi pilihan.
                    ? payload.formulas.map((value) => String(value ?? "").trim())
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `[];`; nilai cadangan ini melengkapi ekspresi pilihan.
                    : [];
                // Penjelasan: Menyimpan `detailLabel` dengan menghapus spasi pada awal dan akhir string sebelum nilai diperiksa atau ditampilkan.
                const detailLabel = typeof payload?.detailLabel === "string" && payload.detailLabel.trim().length > 0
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `payload.detailLabel.trim()`; nilai ini menjadi keluaran ekspresi pilihan.
                    ? payload.detailLabel.trim()
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `tooltipText.formula;`; nilai cadangan ini melengkapi ekspresi pilihan.
                    : tooltipText.formula;
                // Penjelasan: Menyimpan `detailFallback` dengan menghapus spasi pada awal dan akhir string sebelum nilai diperiksa atau ditampilkan.
                const detailFallback = typeof payload?.detailFallback === "string" && payload.detailFallback.trim().length > 0
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `payload.detailFallback.trim()`; nilai ini menjadi keluaran ekspresi pilihan.
                    ? payload.detailFallback.trim()
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `tooltipText.defaultFormula;`; nilai cadangan ini melengkapi ekspresi pilihan.
                    : tooltipText.defaultFormula;
                // Penjelasan: Menyimpan `series` dengan membaca `Array.isArray(payload?.series)` dengan akses opsional sehingga properti pada objek kosong tidak menyebabkan kegagalan.
                const series = Array.isArray(payload?.series)
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `payload.series`; nilai ini menjadi keluaran ekspresi pilihan.
                    ? payload.series
                        // Penjelasan: Mendefinisikan fungsi atau pemetaan `.map((item, index) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                        .map((item, index) => {
                            // Penjelasan: Menyimpan `values` dengan membaca `Array.isArray(item?.values)` dengan akses opsional sehingga properti pada objek kosong tidak menyebabkan kegagalan.
                            const values = Array.isArray(item?.values)
                                // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `item.values.map((value) => {`; nilai ini menjadi keluaran ekspresi pilihan.
                                ? item.values.map((value) => {
                                    // Penjelasan: Memeriksa kondisi `typeof value !== "number" || !Number.isFinite(value)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                                    if (typeof value !== "number" || !Number.isFinite(value)) {
                                        // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menggunakan null sebagai penanda bahwa objek atau pilihan belum tersedia; eksekusi fungsi berakhir setelah nilai dihitung.
                                        return null;
                                    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                                    }
                                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `value` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                                    return value;
                                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                                })
                                // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `[];`; nilai cadangan ini melengkapi ekspresi pilihan.
                                : [];

                            // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya; eksekusi fungsi berakhir setelah nilai dihitung.
                            return {
                                // Penjelasan: Mengisi properti `name` pada objek atau konfigurasi dengan mengevaluasi `String(item?.name ?? `${tooltipText.series} ${index + 1}`)`; operator ?? memakai nilai cadangan hanya ketika sisi kiri null atau undefined; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                name: String(item?.name ?? `${tooltipText.series} ${index + 1}`),
                                // Penjelasan: Melakukan operasi dengan membaca nilai `values` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                                values
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            };
                        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                        })
                        // Penjelasan: Mendefinisikan fungsi atau pemetaan `.filter((item) => item.values.some((value) => value !== null))`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                        .filter((item) => item.values.some((value) => value !== null))
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `[];`; nilai cadangan ini melengkapi ekspresi pilihan.
                    : [];

                // Penjelasan: Memeriksa kondisi `!labels.length || !series.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (!labels.length || !series.length) {
                    // Penjelasan: Melakukan operasi dengan memanggil `drawEmpty(svg, chartStatusText.noSeries)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    drawEmpty(svg, chartStatusText.noSeries);
                    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                    return;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Menyimpan `numericValues` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
                const numericValues = [];
                // Penjelasan: Mendefinisikan fungsi atau pemetaan `series.forEach((item) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                series.forEach((item) => {
                    // Penjelasan: Mendefinisikan fungsi atau pemetaan `item.values.forEach((value) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                    item.values.forEach((value) => {
                        // Penjelasan: Memeriksa kondisi `value !== null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                        if (value !== null) {
                            // Penjelasan: Melakukan operasi dengan menambahkan `value)` ke akhir array untuk membentuk kumpulan data atau markup dalam urutan yang benar.
                            numericValues.push(value);
                        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                        }
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    });
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                });

                // Penjelasan: Memeriksa kondisi `!numericValues.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (!numericValues.length) {
                    // Penjelasan: Melakukan operasi dengan memanggil `drawEmpty(svg, chartStatusText.noPoints)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    drawEmpty(svg, chartStatusText.noPoints);
                    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                    return;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Melakukan operasi dengan memanggil `clearNode(svg)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                clearNode(svg);

                // Penjelasan: Menyimpan `vb` dengan memilih nilai melalui kondisi ternary `svg.viewBox && svg.viewBox.baseVal ? svg.viewBox.baseVal : null`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
                const vb = svg.viewBox && svg.viewBox.baseVal ? svg.viewBox.baseVal : null;
                // Penjelasan: Menyimpan `baseWidth` dengan memilih nilai melalui kondisi ternary `vb && vb.width ? vb.width : 840`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
                const baseWidth = vb && vb.width ? vb.width : 840;
                // Penjelasan: Menyimpan `height` dengan memilih nilai melalui kondisi ternary `vb && vb.height ? vb.height : 360`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
                const height = vb && vb.height ? vb.height : 360;
                // Penjelasan: Menyimpan `longestLabelLength` dengan mengambil nilai terbesar dari `max, label.length), 0` untuk menjaga hasil tidak di bawah batas minimum. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                const longestLabelLength = labels.reduce((max, label) => Math.max(max, label.length), 0);
                // Penjelasan: Menyimpan `averageLabelLength` dengan membaca nilai `labels.length` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                const averageLabelLength = labels.length
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `labels.reduce((total, label) => total + label.length, 0) / labels.length`; nilai ini menjadi keluaran ekspresi pilihan.
                    ? labels.reduce((total, label) => total + label.length, 0) / labels.length
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `0;`; nilai cadangan ini melengkapi ekspresi pilihan.
                    : 0;
                // Penjelasan: Menyimpan `hostCard` dengan memanggil `svg.closest(".chart-card")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                const hostCard = svg.closest(".chart-card");
                // Penjelasan: Menyimpan `chartInsight` dengan memanggil `ensureBarInsightPanel(hostCard, detailLabel)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                const chartInsight = ensureBarInsightPanel(hostCard, detailLabel);
                // Penjelasan: Memeriksa kondisi `chartInsight`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (chartInsight) {
                    // Penjelasan: Memperbarui `chartInsight.panel.hidden` dengan mengaktifkan flag Boolean dengan nilai true.
                    chartInsight.panel.hidden = true;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
                // Penjelasan: Menyimpan `width` dengan membaca nilai `baseWidth` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                let width = baseWidth;
                // Penjelasan: Menyimpan `barLabelCharsPerLine` dengan menggunakan konstanta numerik `16` sebagai nilai awal atau parameter perhitungan.
                let barLabelCharsPerLine = 16;
                // Penjelasan: Menyimpan `wrappedAxisLabels` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
                let wrappedAxisLabels = [];
                // Penjelasan: Memeriksa kondisi `chartType === "bar"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (chartType === "bar") {
                    // Penjelasan: Menyimpan `minPitchPerLabel` dengan memanggil `Math.min(260, Math.max(` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    const minPitchPerLabel = Math.min(260, Math.max(
                        // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan konstanta numerik `124` sebagai nilai awal atau parameter perhitungan; posisi baris ini menentukan parameter atau urutan elemen.
                        124,
                        // Penjelasan: Melakukan operasi dengan menghitung ekspresi `longestLabelLength * 7.1` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                        longestLabelLength * 7.1,
                        // Penjelasan: Melakukan operasi dengan menghitung ekspresi `averageLabelLength * 6.6 + 42))` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                        averageLabelLength * 6.6 + 42));
                    // Penjelasan: Menyimpan `dynamicWidth` dengan mengambil nilai terbesar dari `baseWidth, labels.length * minPitchPerLabel + 280` untuk menjaga hasil tidak di bawah batas minimum.
                    const dynamicWidth = Math.max(baseWidth, labels.length * minPitchPerLabel + 280);
                    // Penjelasan: Memperbarui `width` dengan membaca nilai `dynamicWidth` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    width = dynamicWidth;
                    // Penjelasan: Memperbarui `barLabelCharsPerLine` dengan mengambil nilai terkecil dari `22, Math.round(minPitchPerLabel / 8))` untuk membatasi hasil pada batas atas yang ditentukan.
                    barLabelCharsPerLine = Math.max(11, Math.min(22, Math.round(minPitchPerLabel / 8)));
                    // Penjelasan: Memperbarui `wrappedAxisLabels` dengan mentransformasikan setiap anggota koleksi dengan `(label) => wrapAxisLabel(label, barLabelCharsPerLine, 2))` untuk membentuk array hasil yang urutannya mengikuti sumber.
                    wrappedAxisLabels = labels.map((label) => wrapAxisLabel(label, barLabelCharsPerLine, 2));
                    // Penjelasan: Memperbarui `svg.style.width` dengan menggunakan literal ``${dynamicWidth}px`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                    svg.style.width = `${dynamicWidth}px`;
                    // Penjelasan: Memperbarui `svg.style.maxWidth` dengan menggunakan literal `"none"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                    svg.style.maxWidth = "none";
                    // Penjelasan: Melakukan operasi dengan menambahkan kelas `"chart-card-hscroll"` sehingga aturan CSS terkait diaktifkan pada elemen.
                    hostCard?.classList.add("chart-card-hscroll");
                // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
                } else {
                    // Penjelasan: Memperbarui `svg.style.width` dengan menggunakan literal `"100%"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                    svg.style.width = "100%";
                    // Penjelasan: Memperbarui `svg.style.maxWidth` dengan menggunakan literal `"100%"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                    svg.style.maxWidth = "100%";
                    // Penjelasan: Melakukan operasi dengan menghapus kelas `"chart-card-hscroll"` sehingga status visual terkait dinonaktifkan.
                    hostCard?.classList.remove("chart-card-hscroll");
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
                // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"viewBox", `0 0 ${width} ${height}``; atribut mengendalikan presentasi atau aksesibilitas komponen.
                svg.setAttribute("viewBox", `0 0 ${width} ${height}`);

                // Penjelasan: Menyimpan `legendLineHeight` dengan menggunakan konstanta numerik `16` sebagai nilai awal atau parameter perhitungan.
                const legendLineHeight = 16;
                // Penjelasan: Menyimpan `estimateLegendRows` dengan membaca nilai `() => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                const estimateLegendRows = () => {
                    // Penjelasan: Memeriksa kondisi `chartType === "bar" && series.length === 1`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                    if (chartType === "bar" && series.length === 1) {
                        // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menggunakan konstanta numerik `0` sebagai nilai awal atau parameter perhitungan; eksekusi fungsi berakhir setelah nilai dihitung.
                        return 0;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                    }

                    // Penjelasan: Menyimpan `rowMaxWidth` dengan mengambil nilai terbesar dari `180, width - 110` untuk menjaga hasil tidak di bawah batas minimum.
                    const rowMaxWidth = Math.max(180, width - 110);
                    // Penjelasan: Menyimpan `rowWidth` dengan menggunakan konstanta numerik `0` sebagai nilai awal atau parameter perhitungan.
                    let rowWidth = 0;
                    // Penjelasan: Menyimpan `rows` dengan menggunakan konstanta numerik `1` sebagai nilai awal atau parameter perhitungan.
                    let rows = 1;
                    // Penjelasan: Mendefinisikan fungsi atau pemetaan `series.forEach((item) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                    series.forEach((item) => {
                        // Penjelasan: Menyimpan `itemWidth` dengan mengambil nilai terbesar dari `110, item.name.length * 8 + 36` untuk menjaga hasil tidak di bawah batas minimum.
                        const itemWidth = Math.max(110, item.name.length * 8 + 36);
                        // Penjelasan: Memeriksa kondisi `rowWidth + itemWidth > rowMaxWidth`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                        if (rowWidth + itemWidth > rowMaxWidth) {
                            // Penjelasan: Memperbarui `rows` dengan menggunakan konstanta numerik `1` sebagai nilai awal atau parameter perhitungan; operator `+=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                            rows += 1;
                            // Penjelasan: Memperbarui `rowWidth` dengan membaca nilai `itemWidth` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            rowWidth = itemWidth;
                            // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                            return;
                        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                        }

                        // Penjelasan: Memperbarui `rowWidth` dengan membaca nilai `itemWidth` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; operator `+=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                        rowWidth += itemWidth;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    });

                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan membaca nilai `rows` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; eksekusi fungsi berakhir setelah nilai dihitung.
                    return rows;
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                };

                // Penjelasan: Menyimpan `legendRows` dengan memanggil `estimateLegendRows()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                const legendRows = estimateLegendRows();
                // Penjelasan: Menyimpan `margin` dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya.
                const margin = {
                    // Penjelasan: Mengisi properti `top` pada objek atau konfigurasi dengan menghitung ekspresi `16 + legendRows * legendLineHeight + 12` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    top: 16 + legendRows * legendLineHeight + 12,
                    // Penjelasan: Mengisi properti `right` pada objek atau konfigurasi dengan memilih nilai melalui kondisi ternary `chartType === "bar" ? 52 : 24`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    right: chartType === "bar" ? 52 : 24,
                    // Penjelasan: Mengisi properti `bottom` pada objek atau konfigurasi dengan membaca nilai `chartType === "bar"` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    bottom: chartType === "bar"
                        // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `80 + Math.max(0, (`; nilai ini menjadi keluaran ekspresi pilihan.
                        ? 80 + Math.max(0, (
                            // Penjelasan: Mendefinisikan fungsi atau pemetaan `wrappedAxisLabels.reduce((max, lines) => Math.max(max, lines.length), 1) - 1`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                            wrappedAxisLabels.reduce((max, lines) => Math.max(max, lines.length), 1) - 1
                        // Penjelasan: Melakukan operasi dengan menghitung ekspresi `) * 13)` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                        ) * 13)
                        // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `58,`; nilai cadangan ini melengkapi ekspresi pilihan.
                        : 58,
                    // Penjelasan: Mengisi properti `left` pada objek atau konfigurasi dengan menggunakan konstanta numerik `62` sebagai nilai awal atau parameter perhitungan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    left: 62
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                };
                // Penjelasan: Menyimpan `plotWidth` dengan mengambil nilai terbesar dari `80, width - margin.left - margin.right` untuk menjaga hasil tidak di bawah batas minimum.
                const plotWidth = Math.max(80, width - margin.left - margin.right);
                // Penjelasan: Menyimpan `plotHeight` dengan mengambil nilai terbesar dari `80, height - margin.top - margin.bottom` untuk menjaga hasil tidak di bawah batas minimum.
                const plotHeight = Math.max(80, height - margin.top - margin.bottom);

                // Penjelasan: Menyimpan `minY` dengan mengambil nilai terkecil dari `...numericValues` untuk membatasi hasil pada batas atas yang ditentukan.
                let minY = Math.min(...numericValues);
                // Penjelasan: Menyimpan `maxY` dengan mengambil nilai terbesar dari `...numericValues` untuk menjaga hasil tidak di bawah batas minimum.
                let maxY = Math.max(...numericValues);
                // Penjelasan: Menyimpan `hasNegativeValues` dengan memeriksa apakah setidaknya satu elemen memenuhi `(value) => value < 0)`; pemeriksaan berhenti saat ada kecocokan. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                const hasNegativeValues = numericValues.some((value) => value < 0);
                // Penjelasan: Memeriksa kondisi `chartType === "bar"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (chartType === "bar") {
                    // Penjelasan: Memperbarui `minY` dengan mengambil nilai terkecil dari `minY, 0` untuk membatasi hasil pada batas atas yang ditentukan.
                    minY = Math.min(minY, 0);
                    // Penjelasan: Memperbarui `maxY` dengan mengambil nilai terbesar dari `maxY, 0` untuk menjaga hasil tidak di bawah batas minimum.
                    maxY = Math.max(maxY, 0);
                    // Penjelasan: Menyimpan `range` dengan mengambil nilai terbesar dari `0.000001, maxY - minY` untuk menjaga hasil tidak di bawah batas minimum.
                    const range = Math.max(0.000001, maxY - minY);
                    // Penjelasan: Menyimpan `topPad` dengan mengambil nilai terbesar dari `0.5, range * 0.09` untuk menjaga hasil tidak di bawah batas minimum.
                    const topPad = Math.max(0.5, range * 0.09);
                    // Penjelasan: Memperbarui `maxY` dengan membaca nilai `topPad` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; operator `+=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                    maxY += topPad;
                    // Penjelasan: Memeriksa kondisi `hasNegativeValues`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                    if (hasNegativeValues) {
                        // Penjelasan: Memperbarui `minY` dengan menghitung ekspresi `topPad * 0.35` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; operator `-=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                        minY -= topPad * 0.35;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                    }
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
                // Penjelasan: Memeriksa kondisi `Math.abs(maxY - minY) < 0.000001`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (Math.abs(maxY - minY) < 0.000001) {
                    // Penjelasan: Menyimpan `pad` dengan menghitung besar absolut `maxY) < 1 ? 1 : Math.abs(maxY` tanpa tanda negatif untuk keputusan format atau skala.
                    const pad = Math.abs(maxY) < 1 ? 1 : Math.abs(maxY) * 0.1;
                    // Penjelasan: Memperbarui `minY` dengan membaca nilai `pad` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; operator `-=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                    minY -= pad;
                    // Penjelasan: Memperbarui `maxY` dengan membaca nilai `pad` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; operator `+=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                    maxY += pad;
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Menyimpan `clusterWidth` dengan mengambil nilai terbesar dari `1, labels.length` untuk menjaga hasil tidak di bawah batas minimum.
                const clusterWidth = plotWidth / Math.max(1, labels.length);
                // Penjelasan: Menyimpan `xForIndex` dengan membaca nilai `(index) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                const xForIndex = (index) => {
                    // Penjelasan: Memeriksa kondisi `chartType === "bar"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                    if (chartType === "bar") {
                        // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menghitung ekspresi `margin.left + clusterWidth * index + clusterWidth / 2` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; eksekusi fungsi berakhir setelah nilai dihitung.
                        return margin.left + clusterWidth * index + clusterWidth / 2;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                    }

                    // Penjelasan: Memeriksa kondisi `labels.length === 1`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                    if (labels.length === 1) {
                        // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menghitung ekspresi `margin.left + plotWidth / 2` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; eksekusi fungsi berakhir setelah nilai dihitung.
                        return margin.left + plotWidth / 2;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                    }

                    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menghitung ekspresi `margin.left + (index * plotWidth) / (labels.length - 1)` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; eksekusi fungsi berakhir setelah nilai dihitung.
                    return margin.left + (index * plotWidth) / (labels.length - 1);
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                };

                // Penjelasan: Menyimpan `yForValue` dengan membaca nilai `(value) => (` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                const yForValue = (value) => (
                    // Penjelasan: Melakukan operasi dengan menghitung ekspresi `margin.top + ((maxY - value) * plotHeight) / (maxY - minY)` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                    margin.top + ((maxY - value) * plotHeight) / (maxY - minY)
                // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                );
                // Penjelasan: Menyimpan `zeroY` dengan memanggil `yForValue(0)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                const zeroY = yForValue(0);

                // Penjelasan: Menyimpan `gridTicks` dengan menggunakan konstanta numerik `4` sebagai nilai awal atau parameter perhitungan.
                const gridTicks = 4;
                // Penjelasan: Mengulang blok dengan pengaturan `let tick = 0; tick <= gridTicks; tick += 1`; inisialisasi, syarat kelanjutan, dan perubahan indeks mengendalikan jumlah iterasi.
                for (let tick = 0; tick <= gridTicks; tick += 1) {
                    // Penjelasan: Menyimpan `ratio` dengan menghitung ekspresi `tick / gridTicks` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                    const ratio = tick / gridTicks;
                    // Penjelasan: Menyimpan `y` dengan menghitung ekspresi `margin.top + ratio * plotHeight` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                    const y = margin.top + ratio * plotHeight;
                    // Penjelasan: Menyimpan `value` dengan menghitung ekspresi `maxY - (maxY - minY) * ratio` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                    const value = maxY - (maxY - minY) * ratio;

                    // Penjelasan: Melakukan operasi dengan memanggil `svg.appendChild(makeNode("line", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    svg.appendChild(makeNode("line", {
                        // Penjelasan: Mengisi properti `x1` pada objek atau konfigurasi dengan membaca nilai `margin.left` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        x1: margin.left,
                        // Penjelasan: Mengisi properti `y1` pada objek atau konfigurasi dengan membaca nilai `y` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        y1: y,
                        // Penjelasan: Mengisi properti `x2` pada objek atau konfigurasi dengan menghitung ekspresi `margin.left + plotWidth` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        x2: margin.left + plotWidth,
                        // Penjelasan: Mengisi properti `y2` pada objek atau konfigurasi dengan membaca nilai `y` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        y2: y,
                        // Penjelasan: Mengisi properti `stroke` pada objek atau konfigurasi dengan menggunakan literal `"rgba(60, 125, 145, 0.18)"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        stroke: "rgba(60, 125, 145, 0.18)",
                        // Penjelasan: Mengisi properti `"stroke-width"` pada objek atau konfigurasi dengan menggunakan literal `"1"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        "stroke-width": "1"
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    }));

                    // Penjelasan: Menyimpan `label` dengan memanggil `makeNode("text", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    const label = makeNode("text", {
                        // Penjelasan: Mengisi properti `x` pada objek atau konfigurasi dengan menghitung ekspresi `margin.left - 8` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        x: margin.left - 8,
                        // Penjelasan: Mengisi properti `y` pada objek atau konfigurasi dengan menghitung ekspresi `y + 4` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        y: y + 4,
                        // Penjelasan: Mengisi properti `"text-anchor"` pada objek atau konfigurasi dengan menggunakan literal `"end"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        "text-anchor": "end",
                        // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan menggunakan literal `"#5f7f92"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        fill: "#5f7f92",
                        // Penjelasan: Mengisi properti `"font-size"` pada objek atau konfigurasi dengan menggunakan literal `"12"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        "font-size": "12",
                        // Penjelasan: Mengisi properti `"font-family"` pada objek atau konfigurasi dengan menggunakan literal `"Nunito, Segoe UI, sans-serif"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        "font-family": "Nunito, Segoe UI, sans-serif"
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    });
                    // Penjelasan: Memperbarui `label.textContent` dengan mengganti bagian teks berdasarkan pola dan pengganti `/\.0$/, ""`.
                    label.textContent = value.toFixed(1).replace(/\.0$/, "");
                    // Penjelasan: Melakukan operasi dengan menempatkan node `label` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                    svg.appendChild(label);
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Melakukan operasi dengan memanggil `svg.appendChild(makeNode("line", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                svg.appendChild(makeNode("line", {
                    // Penjelasan: Mengisi properti `x1` pada objek atau konfigurasi dengan membaca nilai `margin.left` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    x1: margin.left,
                    // Penjelasan: Mengisi properti `y1` pada objek atau konfigurasi dengan membaca nilai `margin.top` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    y1: margin.top,
                    // Penjelasan: Mengisi properti `x2` pada objek atau konfigurasi dengan membaca nilai `margin.left` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    x2: margin.left,
                    // Penjelasan: Mengisi properti `y2` pada objek atau konfigurasi dengan menghitung ekspresi `margin.top + plotHeight` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    y2: margin.top + plotHeight,
                    // Penjelasan: Mengisi properti `stroke` pada objek atau konfigurasi dengan menggunakan literal `"#9ccfd2"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    stroke: "#9ccfd2",
                    // Penjelasan: Mengisi properti `"stroke-width"` pada objek atau konfigurasi dengan menggunakan literal `"1.4"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    "stroke-width": "1.4"
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                }));

                // Penjelasan: Melakukan operasi dengan memanggil `svg.appendChild(makeNode("line", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                svg.appendChild(makeNode("line", {
                    // Penjelasan: Mengisi properti `x1` pada objek atau konfigurasi dengan membaca nilai `margin.left` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    x1: margin.left,
                    // Penjelasan: Mengisi properti `y1` pada objek atau konfigurasi dengan memilih nilai melalui kondisi ternary `chartType === "bar" ? zeroY : margin.top + plotHeight`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    y1: chartType === "bar" ? zeroY : margin.top + plotHeight,
                    // Penjelasan: Mengisi properti `x2` pada objek atau konfigurasi dengan menghitung ekspresi `margin.left + plotWidth` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    x2: margin.left + plotWidth,
                    // Penjelasan: Mengisi properti `y2` pada objek atau konfigurasi dengan memilih nilai melalui kondisi ternary `chartType === "bar" ? zeroY : margin.top + plotHeight`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    y2: chartType === "bar" ? zeroY : margin.top + plotHeight,
                    // Penjelasan: Mengisi properti `stroke` pada objek atau konfigurasi dengan menggunakan literal `"#9ccfd2"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    stroke: "#9ccfd2",
                    // Penjelasan: Mengisi properti `"stroke-width"` pada objek atau konfigurasi dengan menggunakan literal `"1.4"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                    "stroke-width": "1.4"
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                }));

                // Penjelasan: Menyimpan `xLabelStep` dengan membaca nilai `chartType === "bar"` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                const xLabelStep = chartType === "bar"
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `1`; nilai ini menjadi keluaran ekspresi pilihan.
                    ? 1
                    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `Math.max(1, Math.ceil(labels.length / 6));`; nilai cadangan ini melengkapi ekspresi pilihan.
                    : Math.max(1, Math.ceil(labels.length / 6));
                // Penjelasan: Mendefinisikan fungsi atau pemetaan `labels.forEach((label, index) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                labels.forEach((label, index) => {
                    // Penjelasan: Menyimpan `isLast` dengan menghitung ekspresi `index === labels.length - 1` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                    const isLast = index === labels.length - 1;
                    // Penjelasan: Memeriksa kondisi `!isLast && index % xLabelStep !== 0`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                    if (!isLast && index % xLabelStep !== 0) {
                        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                        return;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                    }

                    // Penjelasan: Memeriksa kondisi `chartType === "bar"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                    if (chartType === "bar") {
                        // Penjelasan: Menyimpan `x` dengan memanggil `xForIndex(index)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const x = xForIndex(index);
                        // Penjelasan: Menyimpan `y` dengan menghitung ekspresi `margin.top + plotHeight + 18` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                        const y = margin.top + plotHeight + 18;
                        // Penjelasan: Menyimpan `labelLines` dengan membaca nilai `wrappedAxisLabels[index] || [label]` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        const labelLines = wrappedAxisLabels[index] || [label];
                        // Penjelasan: Menyimpan `text` dengan memanggil `makeNode("text", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const text = makeNode("text", {
                            // Penjelasan: Melakukan operasi dengan membaca nilai `x` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            x,
                            // Penjelasan: Melakukan operasi dengan membaca nilai `y` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            y,
                            // Penjelasan: Mengisi properti `"text-anchor"` pada objek atau konfigurasi dengan menggunakan literal `"middle"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            "text-anchor": "middle",
                            // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan menggunakan literal `"#5f7f92"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            fill: "#5f7f92",
                            // Penjelasan: Mengisi properti `"font-size"` pada objek atau konfigurasi dengan menggunakan literal `"10.5"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            "font-size": "10.5",
                            // Penjelasan: Mengisi properti `"font-family"` pada objek atau konfigurasi dengan menggunakan literal `"Nunito, Segoe UI, sans-serif"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            "font-family": "Nunito, Segoe UI, sans-serif"
                        // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                        });
                        // Penjelasan: Mendefinisikan fungsi atau pemetaan `labelLines.forEach((line, lineIndex) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                        labelLines.forEach((line, lineIndex) => {
                            // Penjelasan: Menyimpan `tspan` dengan memanggil `makeNode("tspan", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const tspan = makeNode("tspan", {
                                // Penjelasan: Melakukan operasi dengan membaca nilai `x` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                                x,
                                // Penjelasan: Mengisi properti `dy` pada objek atau konfigurasi dengan memilih nilai melalui kondisi ternary `lineIndex === 0 ? 0 : 12`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                dy: lineIndex === 0 ? 0 : 12
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Memperbarui `tspan.textContent` dengan membaca nilai `line` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            tspan.textContent = line;
                            // Penjelasan: Melakukan operasi dengan menempatkan node `tspan` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                            text.appendChild(tspan);
                        // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                        });
                        // Penjelasan: Memeriksa kondisi `labelLines.join(" ").replace(/\.\.\.$/, "").trim() !== label`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                        if (labelLines.join(" ").replace(/\.\.\.$/, "").trim() !== label) {
                            // Penjelasan: Menyimpan `fullLabel` dengan memanggil `makeNode("title")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const fullLabel = makeNode("title");
                            // Penjelasan: Memperbarui `fullLabel.textContent` dengan membaca nilai `label` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            fullLabel.textContent = label;
                            // Penjelasan: Melakukan operasi dengan menempatkan node `fullLabel` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                            text.appendChild(fullLabel);
                        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                        }
                        // Penjelasan: Melakukan operasi dengan menempatkan node `text` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                        svg.appendChild(text);
                        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                        return;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                    }

                    // Penjelasan: Menyimpan `text` dengan memanggil `makeNode("text", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    const text = makeNode("text", {
                        // Penjelasan: Mengisi properti `x` pada objek atau konfigurasi dengan memanggil `xForIndex(index)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        x: xForIndex(index),
                        // Penjelasan: Mengisi properti `y` pada objek atau konfigurasi dengan menghitung ekspresi `margin.top + plotHeight + 18` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        y: margin.top + plotHeight + 18,
                        // Penjelasan: Mengisi properti `"text-anchor"` pada objek atau konfigurasi dengan menggunakan literal `"middle"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        "text-anchor": "middle",
                        // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan menggunakan literal `"#5f7f92"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        fill: "#5f7f92",
                        // Penjelasan: Mengisi properti `"font-size"` pada objek atau konfigurasi dengan menggunakan literal `"11"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        "font-size": "11",
                        // Penjelasan: Mengisi properti `"font-family"` pada objek atau konfigurasi dengan menggunakan literal `"Nunito, Segoe UI, sans-serif"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                        "font-family": "Nunito, Segoe UI, sans-serif"
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    });
                    // Penjelasan: Memperbarui `text.textContent` dengan membaca nilai `label` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    text.textContent = label;
                    // Penjelasan: Melakukan operasi dengan menempatkan node `text` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                    svg.appendChild(text);
                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                });

                // Penjelasan: Memeriksa kondisi `chartType === "bar"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (chartType === "bar") {
                    // Penjelasan: Menyimpan `seriesCount` dengan mengambil nilai terbesar dari `1, series.length` untuk menjaga hasil tidak di bawah batas minimum.
                    const seriesCount = Math.max(1, series.length);
                    // Penjelasan: Menyimpan `seriesGap` dengan membaca nilai `seriesCount > 1` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    const seriesGap = seriesCount > 1
                        // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `Math.max(2, Math.min(8, clusterWidth * 0.05))`; nilai ini menjadi keluaran ekspresi pilihan.
                        ? Math.max(2, Math.min(8, clusterWidth * 0.05))
                        // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `0;`; nilai cadangan ini melengkapi ekspresi pilihan.
                        : 0;
                    // Penjelasan: Menyimpan `clusterInnerWidth` dengan mengambil nilai terkecil dari `220, clusterWidth * 0.72)` untuk membatasi hasil pada batas atas yang ditentukan.
                    const clusterInnerWidth = Math.max(12, Math.min(220, clusterWidth * 0.72));
                    // Penjelasan: Menyimpan `rawBarWidth` dengan membaca nilai `` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    const rawBarWidth =
                        // Penjelasan: Melakukan operasi dengan mengambil nilai terbesar dari `0, seriesCount - 1)` untuk menjaga hasil tidak di bawah batas minimum.
                        (clusterInnerWidth - seriesGap * Math.max(0, seriesCount - 1)) / seriesCount;
                    // Penjelasan: Menyimpan `barWidth` dengan mengambil nilai terkecil dari `56, rawBarWidth)` untuk membatasi hasil pada batas atas yang ditentukan.
                    const barWidth = Math.max(4, Math.min(56, rawBarWidth));
                    // Penjelasan: Menyimpan `barGroupWidth` dengan mengambil nilai terbesar dari `0, seriesCount - 1` untuk menjaga hasil tidak di bawah batas minimum.
                    const barGroupWidth = barWidth * seriesCount + seriesGap * Math.max(0, seriesCount - 1);
                    // Penjelasan: Menyimpan `groupOffset` dengan mengambil nilai terbesar dari `0, (clusterWidth - barGroupWidth) / 2` untuk menjaga hasil tidak di bawah batas minimum.
                    const groupOffset = Math.max(0, (clusterWidth - barGroupWidth) / 2);

                    // Penjelasan: Mendefinisikan fungsi atau pemetaan `series.forEach((item, seriesIndex) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                    series.forEach((item, seriesIndex) => {
                        // Penjelasan: Menyimpan `seriesColor` dengan memanggil `colorForSeries(seriesIndex)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const seriesColor = colorForSeries(seriesIndex);
                        // Penjelasan: Mendefinisikan fungsi atau pemetaan `item.values.forEach((value, index) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                        item.values.forEach((value, index) => {
                            // Penjelasan: Memeriksa kondisi `value === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                            if (value === null) {
                                // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                                return;
                            // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                            }

                            // Penjelasan: Menyimpan `x` dengan membaca nilai `` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            const x =
                                // Penjelasan: Melakukan operasi dengan menghitung ekspresi `margin.left +` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                                margin.left +
                                // Penjelasan: Melakukan operasi dengan menghitung ekspresi `clusterWidth * index +` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                                clusterWidth * index +
                                // Penjelasan: Melakukan operasi dengan menghitung ekspresi `groupOffset +` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                                groupOffset +
                                // Penjelasan: Melakukan operasi dengan menghitung ekspresi `seriesIndex * (barWidth + seriesGap)` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                                seriesIndex * (barWidth + seriesGap);
                            // Penjelasan: Menyimpan `y` dengan memanggil `yForValue(value)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const y = yForValue(value);
                            // Penjelasan: Menyimpan `barTop` dengan mengambil nilai terkecil dari `y, zeroY` untuk membatasi hasil pada batas atas yang ditentukan.
                            const barTop = Math.min(y, zeroY);
                            // Penjelasan: Menyimpan `barHeight` dengan mengambil nilai terbesar dari `1.5, Math.abs(zeroY - y)` untuk menjaga hasil tidak di bawah batas minimum.
                            const barHeight = Math.max(1.5, Math.abs(zeroY - y));
                            // Penjelasan: Menyimpan `barColor` dengan memanggil `colorForVariable(index)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const barColor = colorForVariable(index);
                            // Penjelasan: Menyimpan `rect` dengan memanggil `makeNode("rect", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const rect = makeNode("rect", {
                                // Penjelasan: Melakukan operasi dengan membaca nilai `x` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                                x,
                                // Penjelasan: Mengisi properti `y` pada objek atau konfigurasi dengan membaca nilai `barTop` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                y: barTop,
                                // Penjelasan: Mengisi properti `width` pada objek atau konfigurasi dengan membaca nilai `barWidth` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                width: barWidth,
                                // Penjelasan: Mengisi properti `height` pada objek atau konfigurasi dengan membaca nilai `barHeight` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                height: barHeight,
                                // Penjelasan: Mengisi properti `rx` pada objek atau konfigurasi dengan menggunakan literal `"2"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                rx: "2",
                                // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan membaca nilai `barColor` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                fill: barColor,
                                // Penjelasan: Mengisi properti `stroke` pada objek atau konfigurasi dengan memilih nilai melalui kondisi ternary `series.length > 1 ? seriesColor : "none"`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                stroke: series.length > 1 ? seriesColor : "none",
                                // Penjelasan: Mengisi properti `"stroke-width"` pada objek atau konfigurasi dengan memilih nilai melalui kondisi ternary `series.length > 1 ? "1.2" : "0"`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                "stroke-width": series.length > 1 ? "1.2" : "0",
                                // Penjelasan: Mengisi properti `opacity` pada objek atau konfigurasi dengan menggunakan literal `"0.92"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                opacity: "0.92"
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Menyimpan `variableLabel` dengan membaca nilai `` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            const variableLabel =
                                // Penjelasan: Melakukan operasi dengan membaca nilai `labels[index] ||` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                                labels[index] ||
                                // Penjelasan: Melakukan operasi dengan membaca nilai `metricKeys[index] ||` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                                metricKeys[index] ||
                                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal ``${tooltipText.itemPrefix} ${index + 1}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                                `${tooltipText.itemPrefix} ${index + 1}`;
                            // Penjelasan: Menyimpan `tooltipLines` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
                            const tooltipLines = [
                                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal ``${tooltipText.metric}: ${variableLabel}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                                `${tooltipText.metric}: ${variableLabel}`,
                                // Penjelasan: Melanjutkan rantai operasi dengan memilih nilai melalui kondisi ternary `value...(series.length > 1 ? [`${tooltipText.series}: ${item.name}`] : [])`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah; hasil tahap sebelumnya menjadi sumber tahap ini.
                                ...(series.length > 1 ? [`${tooltipText.series}: ${item.name}`] : []),
                                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal ``${tooltipText.points}: ${formatMetricValue(value)}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                                `${tooltipText.points}: ${formatMetricValue(value)}`,
                                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal ``${detailLabel}: ${formulaHints[index] || detailFallback}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                                `${detailLabel}: ${formulaHints[index] || detailFallback}`
                            // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            ];
                            // Penjelasan: Menyimpan `insightDetail` dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya.
                            const insightDetail = {
                                // Penjelasan: Mengisi properti `metric` pada objek atau konfigurasi dengan membaca nilai `variableLabel` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                metric: variableLabel,
                                // Penjelasan: Mengisi properti `points` pada objek atau konfigurasi dengan memanggil `formatMetricValue(value)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                points: formatMetricValue(value),
                                // Penjelasan: Mengisi properti `formula` pada objek atau konfigurasi dengan membaca nilai `formulaHints[index] || detailFallback` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                formula: formulaHints[index] || detailFallback
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            };
                            // Penjelasan: Menyimpan `title` dengan memanggil `makeNode("title")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const title = makeNode("title");
                            // Penjelasan: Memperbarui `title.textContent` dengan menggabungkan elemen array menjadi satu string dengan pemisah `"\n"`.
                            title.textContent = tooltipLines.join("\n");
                            // Penjelasan: Melakukan operasi dengan menempatkan node `title` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                            rect.appendChild(title);
                            // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"tabindex", "0"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
                            rect.setAttribute("tabindex", "0");
                            // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"role", "button"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
                            rect.setAttribute("role", "button");
                            // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"aria-label", tooltipLines.join(". ")`; atribut mengendalikan presentasi atau aksesibilitas komponen.
                            rect.setAttribute("aria-label", tooltipLines.join(". "));
                            // Penjelasan: Memperbarui `rect.style.cursor` dengan menggunakan literal `"pointer"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                            rect.style.cursor = "pointer";
                            // Penjelasan: Menyimpan `onSelectBar` dengan memanggil `() => revealBarInsight(chartInsight, insightDetail)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                            const onSelectBar = () => revealBarInsight(chartInsight, insightDetail);
                            // Penjelasan: Mendefinisikan fungsi atau pemetaan `rect.addEventListener("click", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                            rect.addEventListener("click", (event) => {
                                // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
                                event.preventDefault();
                                // Penjelasan: Melakukan operasi dengan memanggil `onSelectBar()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                                onSelectBar();
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Mendefinisikan fungsi atau pemetaan `rect.addEventListener("keydown", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                            rect.addEventListener("keydown", (event) => {
                                // Penjelasan: Memeriksa kondisi `event.key === "Enter" || event.key === " "`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                                if (event.key === "Enter" || event.key === " ") {
                                    // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
                                    event.preventDefault();
                                    // Penjelasan: Melakukan operasi dengan memanggil `onSelectBar()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                                    onSelectBar();
                                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                                }
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Melakukan operasi dengan menempatkan node `rect` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                            svg.appendChild(rect);
                        // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                        });
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    });
                // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
                } else {
                    // Penjelasan: Mendefinisikan fungsi atau pemetaan `series.forEach((item, seriesIndex) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                    series.forEach((item, seriesIndex) => {
                        // Penjelasan: Menyimpan `color` dengan memanggil `colorForSeries(seriesIndex)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const color = colorForSeries(seriesIndex);
                        // Penjelasan: Menyimpan `segment` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
                        let segment = [];

                        // Penjelasan: Menyimpan `flushSegment` dengan membaca nilai `() => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                        const flushSegment = () => {
                            // Penjelasan: Memeriksa kondisi `!segment.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                            if (!segment.length) {
                                // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                                return;
                            // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                            }

                            // Penjelasan: Menyimpan `path` dengan memanggil `buildPath(segment)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const path = buildPath(segment);
                            // Penjelasan: Memeriksa kondisi `path.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                            if (path.length) {
                                // Penjelasan: Melakukan operasi dengan memanggil `svg.appendChild(makeNode("path", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                                svg.appendChild(makeNode("path", {
                                    // Penjelasan: Mengisi properti `d` pada objek atau konfigurasi dengan membaca nilai `path` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                    d: path,
                                    // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan menggunakan literal `"none"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                    fill: "none",
                                    // Penjelasan: Mengisi properti `stroke` pada objek atau konfigurasi dengan membaca nilai `color` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                    stroke: color,
                                    // Penjelasan: Mengisi properti `"stroke-width"` pada objek atau konfigurasi dengan menggunakan literal `"2.4"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                    "stroke-width": "2.4",
                                    // Penjelasan: Mengisi properti `"stroke-linejoin"` pada objek atau konfigurasi dengan menggunakan literal `"round"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                    "stroke-linejoin": "round",
                                    // Penjelasan: Mengisi properti `"stroke-linecap"` pada objek atau konfigurasi dengan menggunakan literal `"round"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                    "stroke-linecap": "round"
                                // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                                }));
                            // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                            }
                            // Penjelasan: Memperbarui `segment` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
                            segment = [];
                        // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                        };

                        // Penjelasan: Mendefinisikan fungsi atau pemetaan `item.values.forEach((value, index) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                        item.values.forEach((value, index) => {
                            // Penjelasan: Memeriksa kondisi `value === null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                            if (value === null) {
                                // Penjelasan: Melakukan operasi dengan memanggil `flushSegment()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                                flushSegment();
                                // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
                                return;
                            // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                            }

                            // Penjelasan: Menyimpan `point` dengan memanggil `{ x: xForIndex(index), y: yForValue(value) }` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const point = { x: xForIndex(index), y: yForValue(value) };
                            // Penjelasan: Melakukan operasi dengan menambahkan `point)` ke akhir array untuk membentuk kumpulan data atau markup dalam urutan yang benar.
                            segment.push(point);
                            // Penjelasan: Menyimpan `pointLabel` dengan membaca nilai `` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            const pointLabel =
                                // Penjelasan: Melakukan operasi dengan membaca nilai `labels[index] ||` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                                labels[index] ||
                                // Penjelasan: Melakukan operasi dengan membaca nilai `metricKeys[index] ||` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                                metricKeys[index] ||
                                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal ``${tooltipText.itemPrefix} ${index + 1}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                                `${tooltipText.itemPrefix} ${index + 1}`;
                            // Penjelasan: Menyimpan `metricLabel` dengan membaca nilai `series.length > 1` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            const metricLabel = series.length > 1
                                // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu ``${item.name} - ${pointLabel}``; nilai ini menjadi keluaran ekspresi pilihan.
                                ? `${item.name} - ${pointLabel}`
                                // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `pointLabel;`; nilai cadangan ini melengkapi ekspresi pilihan.
                                : pointLabel;
                            // Penjelasan: Menyimpan `tooltipLines` dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data.
                            const tooltipLines = [
                                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal ``${tooltipText.metric}: ${metricLabel}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                                `${tooltipText.metric}: ${metricLabel}`,
                                // Penjelasan: Melanjutkan rantai operasi dengan memilih nilai melalui kondisi ternary `value...(series.length > 1 ? [`${tooltipText.series}: ${item.name}`] : [])`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah; hasil tahap sebelumnya menjadi sumber tahap ini.
                                ...(series.length > 1 ? [`${tooltipText.series}: ${item.name}`] : []),
                                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal ``${tooltipText.points}: ${formatMetricValue(value)}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                                `${tooltipText.points}: ${formatMetricValue(value)}`,
                                // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal ``${detailLabel}: ${formulaHints[index] || detailFallback}`` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
                                `${detailLabel}: ${formulaHints[index] || detailFallback}`
                            // Penjelasan: Menutup koleksi array yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            ];
                            // Penjelasan: Menyimpan `insightDetail` dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya.
                            const insightDetail = {
                                // Penjelasan: Mengisi properti `metric` pada objek atau konfigurasi dengan membaca nilai `metricLabel` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                metric: metricLabel,
                                // Penjelasan: Mengisi properti `points` pada objek atau konfigurasi dengan memanggil `formatMetricValue(value)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                points: formatMetricValue(value),
                                // Penjelasan: Mengisi properti `formula` pada objek atau konfigurasi dengan membaca nilai `formulaHints[index] || detailFallback` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                formula: formulaHints[index] || detailFallback
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            };
                            // Penjelasan: Menyimpan `hitTarget` dengan memanggil `makeNode("circle", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const hitTarget = makeNode("circle", {
                                // Penjelasan: Mengisi properti `cx` pada objek atau konfigurasi dengan membaca nilai `point.x` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                cx: point.x,
                                // Penjelasan: Mengisi properti `cy` pada objek atau konfigurasi dengan membaca nilai `point.y` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                cy: point.y,
                                // Penjelasan: Mengisi properti `r` pada objek atau konfigurasi dengan menggunakan literal `"10"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                r: "10",
                                // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan menggunakan literal `"transparent"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                fill: "transparent"
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Menyimpan `pointNode` dengan memanggil `makeNode("circle", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const pointNode = makeNode("circle", {
                                // Penjelasan: Mengisi properti `cx` pada objek atau konfigurasi dengan membaca nilai `point.x` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                cx: point.x,
                                // Penjelasan: Mengisi properti `cy` pada objek atau konfigurasi dengan membaca nilai `point.y` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                cy: point.y,
                                // Penjelasan: Mengisi properti `r` pada objek atau konfigurasi dengan menggunakan literal `"3"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                r: "3",
                                // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan membaca nilai `color` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                fill: color,
                                // Penjelasan: Mengisi properti `stroke` pada objek atau konfigurasi dengan menggunakan literal `"#ffffff"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                stroke: "#ffffff",
                                // Penjelasan: Mengisi properti `"stroke-width"` pada objek atau konfigurasi dengan menggunakan literal `"1"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                "stroke-width": "1"
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Menyimpan `title` dengan memanggil `makeNode("title")` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            const title = makeNode("title");
                            // Penjelasan: Memperbarui `title.textContent` dengan menggabungkan elemen array menjadi satu string dengan pemisah `"\n"`.
                            title.textContent = tooltipLines.join("\n");
                            // Penjelasan: Melakukan operasi dengan menempatkan node `title` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                            pointNode.appendChild(title);
                            // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"tabindex", "0"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
                            pointNode.setAttribute("tabindex", "0");
                            // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"role", "button"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
                            pointNode.setAttribute("role", "button");
                            // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"aria-label", tooltipLines.join(". ")`; atribut mengendalikan presentasi atau aksesibilitas komponen.
                            pointNode.setAttribute("aria-label", tooltipLines.join(". "));
                            // Penjelasan: Memperbarui `pointNode.style.cursor` dengan menggunakan literal `"pointer"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                            pointNode.style.cursor = "pointer";
                            // Penjelasan: Memperbarui `hitTarget.style.cursor` dengan menggunakan literal `"pointer"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
                            hitTarget.style.cursor = "pointer";
                            // Penjelasan: Menyimpan `onSelectPoint` dengan memanggil `() => revealBarInsight(chartInsight, insightDetail)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
                            const onSelectPoint = () => revealBarInsight(chartInsight, insightDetail);
                            // Penjelasan: Mendefinisikan fungsi atau pemetaan `hitTarget.addEventListener("click", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                            hitTarget.addEventListener("click", (event) => {
                                // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
                                event.preventDefault();
                                // Penjelasan: Melakukan operasi dengan memanggil `onSelectPoint()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                                onSelectPoint();
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Mendefinisikan fungsi atau pemetaan `pointNode.addEventListener("click", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                            pointNode.addEventListener("click", (event) => {
                                // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
                                event.preventDefault();
                                // Penjelasan: Melakukan operasi dengan memanggil `onSelectPoint()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                                onSelectPoint();
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Mendefinisikan fungsi atau pemetaan `pointNode.addEventListener("keydown", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                            pointNode.addEventListener("keydown", (event) => {
                                // Penjelasan: Memeriksa kondisi `event.key === "Enter" || event.key === " "`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                                if (event.key === "Enter" || event.key === " ") {
                                    // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
                                    event.preventDefault();
                                    // Penjelasan: Melakukan operasi dengan memanggil `onSelectPoint()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                                    onSelectPoint();
                                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                                }
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            });
                            // Penjelasan: Melakukan operasi dengan menempatkan node `hitTarget` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                            svg.appendChild(hitTarget);
                            // Penjelasan: Melakukan operasi dengan menempatkan node `pointNode` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                            svg.appendChild(pointNode);
                        // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                        });
                        // Penjelasan: Melakukan operasi dengan memanggil `flushSegment()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        flushSegment();
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    });
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }

                // Penjelasan: Memeriksa kondisi `!(chartType === "bar" && series.length === 1)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                if (!(chartType === "bar" && series.length === 1)) {
                    // Penjelasan: Menyimpan `legendStartX` dengan membaca nilai `margin.left` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    const legendStartX = margin.left;
                    // Penjelasan: Menyimpan `legendX` dengan membaca nilai `legendStartX` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                    let legendX = legendStartX;
                    // Penjelasan: Menyimpan `legendRow` dengan menggunakan konstanta numerik `0` sebagai nilai awal atau parameter perhitungan.
                    let legendRow = 0;
                    // Penjelasan: Menyimpan `legendYBase` dengan menggunakan konstanta numerik `16` sebagai nilai awal atau parameter perhitungan.
                    const legendYBase = 16;
                    // Penjelasan: Menyimpan `legendMaxX` dengan menghitung ekspresi `margin.left + plotWidth` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                    const legendMaxX = margin.left + plotWidth;
                    // Penjelasan: Mendefinisikan fungsi atau pemetaan `series.forEach((item, index) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
                    series.forEach((item, index) => {
                        // Penjelasan: Menyimpan `color` dengan memanggil `colorForSeries(index)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const color = colorForSeries(index);
                        // Penjelasan: Menyimpan `itemWidth` dengan mengambil nilai terbesar dari `110, item.name.length * 8 + 36` untuk menjaga hasil tidak di bawah batas minimum.
                        const itemWidth = Math.max(110, item.name.length * 8 + 36);
                        // Penjelasan: Memeriksa kondisi `legendX + itemWidth > legendMaxX`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                        if (legendX + itemWidth > legendMaxX) {
                            // Penjelasan: Memperbarui `legendRow` dengan menggunakan konstanta numerik `1` sebagai nilai awal atau parameter perhitungan; operator `+=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                            legendRow += 1;
                            // Penjelasan: Memperbarui `legendX` dengan membaca nilai `legendStartX` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                            legendX = legendStartX;
                        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                        }

                        // Penjelasan: Menyimpan `legendY` dengan menghitung ekspresi `legendYBase + legendRow * legendLineHeight` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
                        const legendY = legendYBase + legendRow * legendLineHeight;
                        // Penjelasan: Memeriksa kondisi `chartType === "bar"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
                        if (chartType === "bar") {
                            // Penjelasan: Melakukan operasi dengan memanggil `svg.appendChild(makeNode("rect", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            svg.appendChild(makeNode("rect", {
                                // Penjelasan: Mengisi properti `x` pada objek atau konfigurasi dengan membaca nilai `legendX` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                x: legendX,
                                // Penjelasan: Mengisi properti `y` pada objek atau konfigurasi dengan menghitung ekspresi `legendY - 4.5` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                y: legendY - 4.5,
                                // Penjelasan: Mengisi properti `width` pada objek atau konfigurasi dengan menggunakan literal `"12"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                width: "12",
                                // Penjelasan: Mengisi properti `height` pada objek atau konfigurasi dengan menggunakan literal `"9"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                height: "9",
                                // Penjelasan: Mengisi properti `rx` pada objek atau konfigurasi dengan menggunakan literal `"2"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                rx: "2",
                                // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan membaca nilai `color` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                fill: color
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            }));
                        // Penjelasan: Memulai cabang alternatif yang hanya diproses apabila kondisi if sebelumnya tidak terpenuhi.
                        } else {
                            // Penjelasan: Melakukan operasi dengan memanggil `svg.appendChild(makeNode("line", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                            svg.appendChild(makeNode("line", {
                                // Penjelasan: Mengisi properti `x1` pada objek atau konfigurasi dengan membaca nilai `legendX` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                x1: legendX,
                                // Penjelasan: Mengisi properti `y1` pada objek atau konfigurasi dengan membaca nilai `legendY` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                y1: legendY,
                                // Penjelasan: Mengisi properti `x2` pada objek atau konfigurasi dengan menghitung ekspresi `legendX + 14` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                x2: legendX + 14,
                                // Penjelasan: Mengisi properti `y2` pada objek atau konfigurasi dengan membaca nilai `legendY` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                y2: legendY,
                                // Penjelasan: Mengisi properti `stroke` pada objek atau konfigurasi dengan membaca nilai `color` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                stroke: color,
                                // Penjelasan: Mengisi properti `"stroke-width"` pada objek atau konfigurasi dengan menggunakan literal `"2.2"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                "stroke-width": "2.2",
                                // Penjelasan: Mengisi properti `"stroke-linecap"` pada objek atau konfigurasi dengan menggunakan literal `"round"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                                "stroke-linecap": "round"
                            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                            }));
                        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                        }

                        // Penjelasan: Menyimpan `label` dengan memanggil `makeNode("text", {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                        const label = makeNode("text", {
                            // Penjelasan: Mengisi properti `x` pada objek atau konfigurasi dengan menghitung ekspresi `legendX + 18` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            x: legendX + 18,
                            // Penjelasan: Mengisi properti `y` pada objek atau konfigurasi dengan menghitung ekspresi `legendY + 4` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            y: legendY + 4,
                            // Penjelasan: Mengisi properti `fill` pada objek atau konfigurasi dengan menggunakan literal `"#32576e"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            fill: "#32576e",
                            // Penjelasan: Mengisi properti `"font-size"` pada objek atau konfigurasi dengan menggunakan literal `"11"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            "font-size": "11",
                            // Penjelasan: Mengisi properti `"font-family"` pada objek atau konfigurasi dengan menggunakan literal `"Nunito, Segoe UI, sans-serif"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
                            "font-family": "Nunito, Segoe UI, sans-serif"
                        // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                        });
                        // Penjelasan: Memperbarui `label.textContent` dengan membaca nilai `item.name` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
                        label.textContent = item.name;
                        // Penjelasan: Melakukan operasi dengan menempatkan node `label` sebagai anak terakhir elemen induk sehingga muncul dalam struktur DOM.
                        svg.appendChild(label);

                        // Penjelasan: Memperbarui `legendX` dengan membaca nilai `itemWidth` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; operator `+=` menggabungkan nilai baru dengan nilai yang sudah tersimpan.
                        legendX += itemWidth;
                    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
                    });
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            };

            // Penjelasan: Mendefinisikan fungsi atau pemetaan `chartNodes.forEach((node) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
            chartNodes.forEach((node) => {
                // Penjelasan: Memulai bagian yang dapat gagal, misalnya parsing atau permintaan data; kesalahan diteruskan ke catch yang menyertainya.
                try {
                    // Penjelasan: Menyimpan `payload` dengan mendeserialisasi string JSON `node.dataset.chart || "{}"` menjadi objek JavaScript; JSON tidak valid akan masuk penanganan kesalahan.
                    const payload = JSON.parse(node.dataset.chart || "{}");
                    // Penjelasan: Melakukan operasi dengan memanggil `drawChart(node, payload)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    drawChart(node, payload);
                // Penjelasan: Menangani kesalahan melalui catch `{`; alur ini menyediakan pemulihan atau pesan kesalahan ketika operasi pada try gagal.
                } catch {
                    // Penjelasan: Melakukan operasi dengan memanggil `drawEmpty(node, chartStatusText.invalidPayload)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
                    drawEmpty(node, chartStatusText.invalidPayload);
                // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
                }
            // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
            });
        // Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
        })();

