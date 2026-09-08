/* Fungsi file: Menyediakan skrip frontend global untuk perilaku interaktif umum pada UI. */
// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `reduceMotion` dengan memeriksa media query `"(prefers-reduced-motion: reduce)"`; preferensi browser ini dipakai untuk menyesuaikan efek visual.
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  // Penjelasan: Menyimpan `targets` dengan memanggil `Array.from(` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  const targets = Array.from(
    // Penjelasan: Melakukan operasi dengan mencari seluruh elemen DOM yang cocok dengan selector `".section-shell, .stat-card, .table-wrap"`; daftar ini menjadi sasaran perilaku antarmuka.
    document.querySelectorAll(".section-shell, .stat-card, .table-wrap")
  // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  );

  // Penjelasan: Memeriksa kondisi `!targets.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!targets.length) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `targets.forEach((el, index) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  targets.forEach((el, index) => {
    // Penjelasan: Melakukan operasi dengan menambahkan kelas `"reveal"` sehingga aturan CSS terkait diaktifkan pada elemen.
    el.classList.add("reveal");
    // Penjelasan: Menyimpan `delay` dengan mengambil nilai terkecil dari `index * 70, 350` untuk membatasi hasil pada batas atas yang ditentukan.
    const delay = Math.min(index * 70, 350);
    // Penjelasan: Melakukan operasi dengan menetapkan properti CSS `"--delay", `${delay}ms`` pada elemen, termasuk variabel kustom untuk waktu animasi.
    el.style.setProperty("--delay", `${delay}ms`);
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Menyimpan `sectionShells` dengan memeriksa keberadaan kelas `"section-shell")` untuk membaca status visual komponen. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const sectionShells = targets.filter((el) => el.classList.contains("section-shell"));
  // Penjelasan: Menyimpan `observedTargets` dengan memeriksa keberadaan kelas `"section-shell")` untuk membaca status visual komponen. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const observedTargets = targets.filter((el) => !el.classList.contains("section-shell"));

  // Keep primary page containers visible from first paint.
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `sectionShells.forEach((el) => el.classList.add("is-visible"));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  sectionShells.forEach((el) => el.classList.add("is-visible"));

  // Penjelasan: Memeriksa kondisi `reduceMotion`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (reduceMotion) {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `targets.forEach((el) => el.classList.add("is-visible"));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    targets.forEach((el) => el.classList.add("is-visible"));
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Memeriksa kondisi `!observedTargets.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!observedTargets.length) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `observer` dengan membuat pemantau perpotongan elemen dengan area pandang; callback mengaktifkan tampilan ketika elemen mulai terlihat.
  const observer = new IntersectionObserver(
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `(entries) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    (entries) => {
      // Penjelasan: Mendefinisikan fungsi atau pemetaan `entries.forEach((entry) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
      entries.forEach((entry) => {
        // Penjelasan: Memeriksa kondisi `!entry.isIntersecting`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        if (!entry.isIntersecting) {
          // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
          return;
        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
        }
        // Penjelasan: Melakukan operasi dengan menambahkan kelas `"is-visible"` sehingga aturan CSS terkait diaktifkan pada elemen.
        entry.target.classList.add("is-visible");
        // Penjelasan: Melakukan operasi dengan berhenti memantau `entry.target` setelah efek tampilan tidak perlu dipicu ulang.
        observer.unobserve(entry.target);
      // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
      });
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    },
    // Penjelasan: Melakukan operasi dengan membaca nilai `{ threshold: 0.01 }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
    { threshold: 0.01 }
  // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  );

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `observedTargets.forEach((el) => observer.observe(el));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  observedTargets.forEach((el) => observer.observe(el));

  // Failsafe: never leave elements hidden if observer callback misses.
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `window.setTimeout(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  window.setTimeout(() => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `observedTargets.forEach((el) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    observedTargets.forEach((el) => {
      // Penjelasan: Memeriksa kondisi `el.classList.contains("is-visible")`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (el.classList.contains("is-visible")) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Melakukan operasi dengan menambahkan kelas `"is-visible"` sehingga aturan CSS terkait diaktifkan pada elemen.
      el.classList.add("is-visible");
      // Penjelasan: Melakukan operasi dengan berhenti memantau `el` setelah efek tampilan tidak perlu dipicu ulang.
      observer.unobserve(el);
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Melakukan operasi dengan membaca nilai `}, 1200)` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
  }, 1200);
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `dashboard` dengan mencari elemen DOM pertama yang sesuai dengan `".player-stats-dashboard"`; hasil dapat null bila komponen tidak ada pada halaman.
  const dashboard = document.querySelector(".player-stats-dashboard");
  // Penjelasan: Memeriksa kondisi `!dashboard`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!dashboard) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `targets` dengan mencari semua turunan elemen yang sesuai dengan selector `"[data-player-stats-reveal]")`.
  const targets = Array.from(dashboard.querySelectorAll("[data-player-stats-reveal]"));
  // Penjelasan: Menyimpan `revealAll` dengan menambahkan kelas `"is-visible")` sehingga aturan CSS terkait diaktifkan pada elemen. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const revealAll = () => targets.forEach((target) => target.classList.add("is-visible"));
  // Penjelasan: Menyimpan `reduceMotion` dengan memeriksa media query `"(prefers-reduced-motion: reduce)"`; preferensi browser ini dipakai untuk menyesuaikan efek visual.
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // Penjelasan: Memeriksa kondisi `reduceMotion || !("IntersectionObserver" in window)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (reduceMotion || !("IntersectionObserver" in window)) {
    // Penjelasan: Melakukan operasi dengan memanggil `revealAll()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    revealAll();
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Melakukan operasi dengan menambahkan kelas `"player-stats-motion-ready"` sehingga aturan CSS terkait diaktifkan pada elemen.
  dashboard.classList.add("player-stats-motion-ready");
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `targets.forEach((target, index) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  targets.forEach((target, index) => {
    // Penjelasan: Melakukan operasi dengan mengambil nilai terkecil dari `index * 60, 240)}ms`` untuk membatasi hasil pada batas atas yang ditentukan.
    target.style.setProperty("--player-stats-delay", `${Math.min(index * 60, 240)}ms`);
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Menyimpan `observer` dengan membuat pemantau perpotongan elemen dengan area pandang; callback mengaktifkan tampilan ketika elemen mulai terlihat.
  const observer = new IntersectionObserver(
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `(entries) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    (entries) => {
      // Penjelasan: Mendefinisikan fungsi atau pemetaan `entries.forEach((entry) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
      entries.forEach((entry) => {
        // Penjelasan: Memeriksa kondisi `!entry.isIntersecting`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        if (!entry.isIntersecting) {
          // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
          return;
        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
        }

        // Penjelasan: Melakukan operasi dengan menambahkan kelas `"is-visible"` sehingga aturan CSS terkait diaktifkan pada elemen.
        entry.target.classList.add("is-visible");
        // Penjelasan: Melakukan operasi dengan berhenti memantau `entry.target` setelah efek tampilan tidak perlu dipicu ulang.
        observer.unobserve(entry.target);
      // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
      });
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    },
    // Penjelasan: Melakukan operasi dengan menghitung ekspresi `{ rootMargin: "0px 0px -8%", threshold: 0.08 }` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
    { rootMargin: "0px 0px -8%", threshold: 0.08 }
  // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  );

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `targets.forEach((target) => observer.observe(target));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  targets.forEach((target) => observer.observe(target));
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `toggles` dengan mencari seluruh elemen DOM yang cocok dengan selector `".js-nav-toggle")`; daftar ini menjadi sasaran perilaku antarmuka.
  const toggles = Array.from(document.querySelectorAll(".js-nav-toggle"));
  // Penjelasan: Memeriksa kondisi `!toggles.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!toggles.length) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `navShell` dengan mencari elemen DOM pertama yang sesuai dengan `".nav-shell"`; hasil dapat null bila komponen tidak ada pada halaman.
  const navShell = document.querySelector(".nav-shell");
  // Penjelasan: Memeriksa kondisi `!navShell`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!navShell) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `navDropdowns` dengan mencari semua turunan elemen yang sesuai dengan selector `".nav-dropdown")`.
  const navDropdowns = Array.from(navShell.querySelectorAll(".nav-dropdown"));
  // Penjelasan: Menyimpan `closeDropdowns` dengan membaca nilai `() => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const closeDropdowns = () => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `navDropdowns.forEach((dropdown) => dropdown.removeAttribute("open"));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    navDropdowns.forEach((dropdown) => dropdown.removeAttribute("open"));
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  };

  // Penjelasan: Menyimpan `closeMenu` dengan membaca nilai `() => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const closeMenu = () => {
    // Penjelasan: Melakukan operasi dengan menghapus kelas `"is-open"` sehingga status visual terkait dinonaktifkan.
    navShell.classList.remove("is-open");
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `toggles.forEach((toggle) => toggle.setAttribute("aria-expanded", "false"));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    toggles.forEach((toggle) => toggle.setAttribute("aria-expanded", "false"));
    // Penjelasan: Melakukan operasi dengan memanggil `closeDropdowns()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    closeDropdowns();
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  };

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `toggles.forEach((toggle) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  toggles.forEach((toggle) => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `toggle.addEventListener("click", () => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    toggle.addEventListener("click", () => {
      // Penjelasan: Menyimpan `isOpen` dengan membalik keberadaan kelas `"is-open"` dan menghasilkan status aktif setelah perubahan.
      const isOpen = navShell.classList.toggle("is-open");
      // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"aria-expanded", isOpen ? "true" : "false"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
      toggle.setAttribute("aria-expanded", isOpen ? "true" : "false");
      // Penjelasan: Memeriksa kondisi `!isOpen`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (!isOpen) {
        // Penjelasan: Melakukan operasi dengan memanggil `closeDropdowns()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
        closeDropdowns();
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `document.addEventListener("keydown", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  document.addEventListener("keydown", (event) => {
    // Penjelasan: Memeriksa kondisi `event.key === "Escape"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (event.key === "Escape") {
      // Penjelasan: Melakukan operasi dengan memanggil `closeMenu()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
      closeMenu();
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `window.addEventListener("resize", () => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  window.addEventListener("resize", () => {
    // Penjelasan: Memeriksa kondisi `window.innerWidth > 768`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (window.innerWidth > 768) {
      // Penjelasan: Melakukan operasi dengan memanggil `closeMenu()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
      closeMenu();
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Menyimpan `navLinks` dengan mencari seluruh elemen DOM yang cocok dengan selector `".nav-link, .nav-menu-item")`; daftar ini menjadi sasaran perilaku antarmuka.
  const navLinks = Array.from(document.querySelectorAll(".nav-link, .nav-menu-item"));
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `navLinks.forEach((link) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  navLinks.forEach((link) => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `link.addEventListener("click", () => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    link.addEventListener("click", () => {
      // Penjelasan: Memeriksa kondisi `window.innerWidth <= 768`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (window.innerWidth <= 768) {
        // Penjelasan: Melakukan operasi dengan memanggil `closeMenu()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
        closeMenu();
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `dropdowns` dengan mencari seluruh elemen DOM yang cocok dengan selector `".nav-dropdown")`; daftar ini menjadi sasaran perilaku antarmuka.
  const dropdowns = Array.from(document.querySelectorAll(".nav-dropdown"));
  // Penjelasan: Memeriksa kondisi `!dropdowns.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!dropdowns.length) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `dropdowns.forEach((dropdown) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  dropdowns.forEach((dropdown) => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `dropdown.addEventListener("toggle", () => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    dropdown.addEventListener("toggle", () => {
      // Penjelasan: Memeriksa kondisi `!dropdown.open`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (!dropdown.open) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Mendefinisikan fungsi atau pemetaan `dropdowns.forEach((otherDropdown) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
      dropdowns.forEach((otherDropdown) => {
        // Penjelasan: Memeriksa kondisi `otherDropdown === dropdown`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        if (otherDropdown === dropdown) {
          // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
          return;
        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
        }

        // Penjelasan: Melakukan operasi dengan menghapus atribut `"open"` dari elemen sehingga perilaku bawaan tanpa atribut kembali berlaku.
        otherDropdown.removeAttribute("open");
      // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
      });
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `document.addEventListener("click", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  document.addEventListener("click", (event) => {
    // Penjelasan: Menyimpan `target` dengan membaca nilai `event.target` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
    const target = event.target;
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `dropdowns.forEach((dropdown) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    dropdowns.forEach((dropdown) => {
      // Penjelasan: Memeriksa kondisi `dropdown.contains(target)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (dropdown.contains(target)) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }
      // Penjelasan: Melakukan operasi dengan menghapus atribut `"open"` dari elemen sehingga perilaku bawaan tanpa atribut kembali berlaku.
      dropdown.removeAttribute("open");
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `document.addEventListener("keydown", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  document.addEventListener("keydown", (event) => {
    // Penjelasan: Memeriksa kondisi `event.key !== "Escape"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (event.key !== "Escape") {
      // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
      return;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `dropdowns.forEach((dropdown) => dropdown.removeAttribute("open"));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    dropdowns.forEach((dropdown) => dropdown.removeAttribute("open"));
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `quickstart` dengan mencari elemen DOM pertama yang sesuai dengan `"[data-quickstart]"`; hasil dapat null bila komponen tidak ada pada halaman.
  const quickstart = document.querySelector("[data-quickstart]");
  // Penjelasan: Menyimpan `toggle` dengan mencari elemen DOM pertama yang sesuai dengan `"[data-quickstart-toggle]"`; hasil dapat null bila komponen tidak ada pada halaman.
  const toggle = document.querySelector("[data-quickstart-toggle]");
  // Penjelasan: Menyimpan `body` dengan mencari elemen DOM pertama yang sesuai dengan `"[data-quickstart-body]"`; hasil dapat null bila komponen tidak ada pada halaman.
  const body = document.querySelector("[data-quickstart-body]");
  // Penjelasan: Memeriksa kondisi `!quickstart || !toggle || !body`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!quickstart || !toggle || !body) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `expandedLabel` dengan membaca atribut `"data-expanded-label"` sebagai string; nilainya menjadi masukan konfigurasi interaksi.
  const expandedLabel = toggle.getAttribute("data-expanded-label") || "";
  // Penjelasan: Menyimpan `collapsedLabel` dengan membaca atribut `"data-collapsed-label"` sebagai string; nilainya menjadi masukan konfigurasi interaksi.
  const collapsedLabel = toggle.getAttribute("data-collapsed-label") || "";

  // Penjelasan: Menyimpan `applyState` dengan membaca nilai `(collapsed) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const applyState = (collapsed) => {
    // Penjelasan: Melakukan operasi dengan membalik keberadaan kelas `"is-collapsed", collapsed` dan menghasilkan status aktif setelah perubahan.
    quickstart.classList.toggle("is-collapsed", collapsed);
    // Penjelasan: Memperbarui `body.hidden` dengan membaca nilai `collapsed` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
    body.hidden = collapsed;
    // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"aria-expanded", collapsed ? "false" : "true"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
    toggle.setAttribute("aria-expanded", collapsed ? "false" : "true");
    // Penjelasan: Memperbarui `toggle.textContent` dengan memilih nilai melalui kondisi ternary `collapsed ? collapsedLabel : expandedLabel`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
    toggle.textContent = collapsed ? collapsedLabel : expandedLabel;
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  };

  // Penjelasan: Melakukan operasi dengan memanggil `applyState(true)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
  applyState(true);

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `toggle.addEventListener("click", () => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  toggle.addEventListener("click", () => {
    // Penjelasan: Menyimpan `nextCollapsed` dengan memeriksa keberadaan kelas `"is-collapsed"` untuk membaca status visual komponen.
    const nextCollapsed = !quickstart.classList.contains("is-collapsed");
    // Penjelasan: Melakukan operasi dengan memanggil `applyState(nextCollapsed)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    applyState(nextCollapsed);
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `switches` dengan mencari seluruh elemen DOM yang cocok dengan selector `".js-auth-switch")`; daftar ini menjadi sasaran perilaku antarmuka.
  const switches = Array.from(document.querySelectorAll(".js-auth-switch"));
  // Penjelasan: Menyimpan `card` dengan mencari elemen DOM pertama yang sesuai dengan `".auth-split-card"`; hasil dapat null bila komponen tidak ada pada halaman.
  const card = document.querySelector(".auth-split-card");

  // Penjelasan: Memeriksa kondisi `!switches.length || !card`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!switches.length || !card) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `reduceMotion` dengan memeriksa media query `"(prefers-reduced-motion: reduce)"`; preferensi browser ini dipakai untuk menyesuaikan efek visual.
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `switches.forEach((link) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  switches.forEach((link) => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `link.addEventListener("click", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    link.addEventListener("click", (event) => {
      // Penjelasan: Memeriksa kondisi `reduceMotion`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (reduceMotion) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Memeriksa kondisi `event.defaultPrevented || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (event.defaultPrevented || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Menyimpan `href` dengan membaca atribut `"href"` sebagai string; nilainya menjadi masukan konfigurasi interaksi.
      const href = link.getAttribute("href");
      // Penjelasan: Memeriksa kondisi `!href`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (!href) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
      event.preventDefault();
      // Penjelasan: Menyimpan `direction` dengan memilih nilai melalui kondisi ternary `link.dataset.authSwitchDirection === "right" ? "right" : "left"`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
      const direction = link.dataset.authSwitchDirection === "right" ? "right" : "left";
      // Penjelasan: Melakukan operasi dengan menghapus kelas `"auth-switching-left", "auth-switching-right"` sehingga status visual terkait dinonaktifkan.
      card.classList.remove("auth-switching-left", "auth-switching-right");
      // Penjelasan: Melakukan operasi dengan menambahkan kelas `direction === "right" ? "auth-switching-right" : "auth-switching-left"` sehingga aturan CSS terkait diaktifkan pada elemen.
      card.classList.add(direction === "right" ? "auth-switching-right" : "auth-switching-left");

      // Penjelasan: Mendefinisikan fungsi atau pemetaan `window.setTimeout(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
      window.setTimeout(() => {
        // Penjelasan: Melakukan operasi dengan memanggil `window.location.assign(href)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
        window.location.assign(href);
      // Penjelasan: Melakukan operasi dengan membaca nilai `}, 240)` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
      }, 240);
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `forms` dengan mencari seluruh elemen DOM yang cocok dengan selector `"form.auth-form")`; daftar ini menjadi sasaran perilaku antarmuka.
  const forms = Array.from(document.querySelectorAll("form.auth-form"));
  // Penjelasan: Memeriksa kondisi `!forms.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!forms.length) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `focusableSelector` dengan menggunakan literal `"input, select, textarea"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
  const focusableSelector = "input, select, textarea";
  // Penjelasan: Menyimpan `isNavigableField` dengan membaca nilai `(field) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const isNavigableField = (field) => {
    // Penjelasan: Memeriksa kondisi `!(field instanceof HTMLElement)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (!(field instanceof HTMLElement)) {
      // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menonaktifkan flag Boolean dengan nilai false; eksekusi fungsi berakhir setelah nilai dihitung.
      return false;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Memeriksa kondisi `field.hasAttribute("disabled")`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (field.hasAttribute("disabled")) {
      // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menonaktifkan flag Boolean dengan nilai false; eksekusi fungsi berakhir setelah nilai dihitung.
      return false;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Memeriksa kondisi `field.getAttribute("type") === "hidden"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (field.getAttribute("type") === "hidden") {
      // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menonaktifkan flag Boolean dengan nilai false; eksekusi fungsi berakhir setelah nilai dihitung.
      return false;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Memeriksa kondisi `field.getAttribute("readonly") !== null`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (field.getAttribute("readonly") !== null) {
      // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menonaktifkan flag Boolean dengan nilai false; eksekusi fungsi berakhir setelah nilai dihitung.
      return false;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan mengaktifkan flag Boolean dengan nilai true; eksekusi fungsi berakhir setelah nilai dihitung.
    return true;
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  };

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `forms.forEach((form) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  forms.forEach((form) => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `form.addEventListener("keydown", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    form.addEventListener("keydown", (event) => {
      // Penjelasan: Memeriksa kondisi `event.key !== "Enter" || event.shiftKey || event.isComposing`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (event.key !== "Enter" || event.shiftKey || event.isComposing) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Menyimpan `target` dengan membaca nilai `event.target` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
      const target = event.target;
      // Penjelasan: Memeriksa kondisi `!(target instanceof HTMLElement)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (!(target instanceof HTMLElement)) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Memeriksa kondisi `target.tagName === "TEXTAREA"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (target.tagName === "TEXTAREA") {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Menyimpan `fields` dengan mencari semua turunan elemen yang sesuai dengan selector `focusableSelector)).filter(isNavigableField`.
      const fields = Array.from(form.querySelectorAll(focusableSelector)).filter(isNavigableField);
      // Penjelasan: Memeriksa kondisi `!fields.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (!fields.length) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Menyimpan `currentIndex` dengan memanggil `fields.indexOf(target)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
      const currentIndex = fields.indexOf(target);
      // Penjelasan: Memeriksa kondisi `currentIndex < 0`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (currentIndex < 0) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Menyimpan `isLastField` dengan menghitung ekspresi `currentIndex >= fields.length - 1` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
      const isLastField = currentIndex >= fields.length - 1;
      // Penjelasan: Memeriksa kondisi `isLastField`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (isLastField) {
        // Penjelasan: Menyimpan `submitButton` dengan mencari turunan pertama dengan selector `"button[type='submit'], input[type='submit']"`; pemanggil memeriksa ketersediaannya sebelum mengubah tampilan.
        const submitButton = form.querySelector("button[type='submit'], input[type='submit']");
        // Penjelasan: Memeriksa kondisi `!(submitButton instanceof HTMLElement)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        if (!(submitButton instanceof HTMLElement)) {
          // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
          return;
        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
        }

        // Penjelasan: Menyimpan `enterLastSelectMode` dengan menormalkan string menjadi huruf kecil sehingga pencocokan kunci tidak bergantung kapitalisasi masukan.
        const enterLastSelectMode = (form.dataset.enterLastSelect ?? "").toLowerCase();
        // Penjelasan: Menyimpan `isSelectField` dengan membaca nilai `target.tagName === "SELECT"` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
        const isSelectField = target.tagName === "SELECT";
        // Penjelasan: Memeriksa kondisi `isSelectField && enterLastSelectMode === "focus-submit"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        if (isSelectField && enterLastSelectMode === "focus-submit") {
          // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
          event.preventDefault();
          // Penjelasan: Melakukan operasi dengan memanggil `submitButton.focus()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
          submitButton.focus();
          // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
          return;
        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
        }

        // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
        event.preventDefault();
        // Penjelasan: Melakukan operasi dengan memanggil `submitButton.click()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
        submitButton.click();
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Menyimpan `nextField` dengan menghitung ekspresi `fields[currentIndex + 1]` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
      const nextField = fields[currentIndex + 1];
      // Penjelasan: Memeriksa kondisi `!(nextField instanceof HTMLElement)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (!(nextField instanceof HTMLElement)) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
      event.preventDefault();
      // Penjelasan: Melakukan operasi dengan memanggil `nextField.focus()` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
      nextField.focus();
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `document.querySelectorAll("[data-password-toggle]").forEach((button) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  document.querySelectorAll("[data-password-toggle]").forEach((button) => {
    // Penjelasan: Menyimpan `inputId` dengan membaca atribut `"aria-controls"` sebagai string; nilainya menjadi masukan konfigurasi interaksi.
    const inputId = button.getAttribute("aria-controls");
    // Penjelasan: Menyimpan `input` dengan mengambil elemen DOM ber-ID `inputId` agar data atau event dapat dihubungkan ke komponen yang tepat.
    const input = inputId ? document.getElementById(inputId) : null;
    // Penjelasan: Memeriksa kondisi `!(button instanceof HTMLButtonElement) || !(input instanceof HTMLInputElement)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (!(button instanceof HTMLButtonElement) || !(input instanceof HTMLInputElement)) {
      // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
      return;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Mendefinisikan fungsi atau pemetaan `button.addEventListener("click", () => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    button.addEventListener("click", () => {
      // Penjelasan: Menyimpan `isVisible` dengan membaca nilai `input.type === "text"` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
      const isVisible = input.type === "text";
      // Penjelasan: Memperbarui `input.type` dengan memilih nilai melalui kondisi ternary `isVisible ? "password" : "text"`; cabang setelah ? dipakai ketika kondisi benar dan cabang setelah : ketika salah.
      input.type = isVisible ? "password" : "text";
      // Penjelasan: Menyimpan `label` dengan mengevaluasi `isVisible ? button.dataset.showLabel ?? "Show" : button.dataset.hideLabel ?? "Hide"`; operator ?? memakai nilai cadangan hanya ketika sisi kiri null atau undefined.
      const label = isVisible ? button.dataset.showLabel ?? "Show" : button.dataset.hideLabel ?? "Hide";
      // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"aria-label", label`; atribut mengendalikan presentasi atau aksesibilitas komponen.
      button.setAttribute("aria-label", label);
      // Penjelasan: Memperbarui `button.title` dengan membaca nilai `label` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
      button.title = label;
      // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"aria-pressed", isVisible ? "false" : "true"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
      button.setAttribute("aria-pressed", isVisible ? "false" : "true");
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `forms` dengan mencari seluruh elemen DOM yang cocok dengan selector `"form")).filter((form`; daftar ini menjadi sasaran perilaku antarmuka. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const forms = Array.from(document.querySelectorAll("form")).filter((form) => {
    // Penjelasan: Memeriksa kondisi `!(form instanceof HTMLFormElement)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (!(form instanceof HTMLFormElement)) {
      // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menonaktifkan flag Boolean dengan nilai false; eksekusi fungsi berakhir setelah nilai dihitung.
      return false;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Memeriksa kondisi `form.classList.contains("js-no-submit-lock")`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (form.classList.contains("js-no-submit-lock")) {
      // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menonaktifkan flag Boolean dengan nilai false; eksekusi fungsi berakhir setelah nilai dihitung.
      return false;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Memeriksa kondisi `form.closest(".nav-menu")`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (form.closest(".nav-menu")) {
      // Penjelasan: Mengembalikan hasil kepada pemanggil dengan menonaktifkan flag Boolean dengan nilai false; eksekusi fungsi berakhir setelah nilai dihitung.
      return false;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Mengembalikan hasil kepada pemanggil dengan mengaktifkan flag Boolean dengan nilai true; eksekusi fungsi berakhir setelah nilai dihitung.
    return true;
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Memeriksa kondisi `!forms.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!forms.length) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `defaultLoadingText` dengan menormalkan string menjadi huruf kecil sehingga pencocokan kunci tidak bergantung kapitalisasi masukan.
  const defaultLoadingText = document.documentElement.lang.toLowerCase().startsWith("en")
    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya benar, yaitu `"Processing..."`; nilai ini menjadi keluaran ekspresi pilihan.
    ? "Processing..."
    // Penjelasan: Menentukan hasil ketika kondisi ternary sebelumnya salah, yaitu `"Memproses...";`; nilai cadangan ini melengkapi ekspresi pilihan.
    : "Memproses...";

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `forms.forEach((form) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  forms.forEach((form) => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `form.addEventListener("submit", (event) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    form.addEventListener("submit", (event) => {
      // Penjelasan: Memeriksa kondisi `form.dataset.submitting === "true"`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (form.dataset.submitting === "true") {
        // Penjelasan: Melakukan operasi dengan membatalkan aksi bawaan browser untuk event ini sehingga interaksi ditangani oleh kode aplikasi.
        event.preventDefault();
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Menyimpan `submitButtons` dengan memanggil `Array.from(` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
      const submitButtons = Array.from(
        // Penjelasan: Melakukan operasi dengan mencari semua turunan elemen yang sesuai dengan selector `"button[type='submit'], input[type='submit']"`.
        form.querySelectorAll("button[type='submit'], input[type='submit']")
      // Penjelasan: Menutup daftar argumen yang sedang disusun; tanda titik koma mengakhiri pernyataan.
      );
      // Penjelasan: Memeriksa kondisi `!submitButtons.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
      if (!submitButtons.length) {
        // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
        return;
      // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
      }

      // Penjelasan: Memperbarui `form.dataset.submitting` dengan menggunakan literal `"true"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya.
      form.dataset.submitting = "true";
      // Penjelasan: Melakukan operasi dengan menambahkan kelas `"is-submitting"` sehingga aturan CSS terkait diaktifkan pada elemen.
      form.classList.add("is-submitting");

      // Penjelasan: Mendefinisikan fungsi atau pemetaan `submitButtons.forEach((submitButton) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
      submitButtons.forEach((submitButton) => {
        // Penjelasan: Memeriksa kondisi `!(submitButton instanceof HTMLButtonElement || submitButton instanceof HTMLInputElement)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        if (!(submitButton instanceof HTMLButtonElement || submitButton instanceof HTMLInputElement)) {
          // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
          return;
        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
        }

        // Penjelasan: Memeriksa kondisi `submitButton.disabled`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        if (submitButton.disabled) {
          // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
          return;
        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
        }

        // Penjelasan: Memperbarui `submitButton.disabled` dengan mengaktifkan flag Boolean dengan nilai true.
        submitButton.disabled = true;
        // Penjelasan: Melakukan operasi dengan menambahkan kelas `"is-loading"` sehingga aturan CSS terkait diaktifkan pada elemen.
        submitButton.classList.add("is-loading");
        // Penjelasan: Melakukan operasi dengan menetapkan atribut elemen sesuai pasangan nama dan nilai `"aria-disabled", "true"`; atribut mengendalikan presentasi atau aksesibilitas komponen.
        submitButton.setAttribute("aria-disabled", "true");

        // Penjelasan: Menyimpan `loadingText` dengan menghapus spasi pada awal dan akhir string sebelum nilai diperiksa atau ditampilkan.
        const loadingText = submitButton.dataset.loadingText?.trim() || defaultLoadingText;
        // Penjelasan: Memeriksa kondisi `submitButton instanceof HTMLButtonElement`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        if (submitButton instanceof HTMLButtonElement) {
          // Penjelasan: Memeriksa kondisi `!submitButton.dataset.originalText`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
          if (!submitButton.dataset.originalText) {
            // Penjelasan: Memperbarui `submitButton.dataset.originalText` dengan mengevaluasi `submitButton.textContent ?? ""`; operator ?? memakai nilai cadangan hanya ketika sisi kiri null atau undefined.
            submitButton.dataset.originalText = submitButton.textContent ?? "";
          // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
          }
          // Penjelasan: Memperbarui `submitButton.textContent` dengan membaca nilai `loadingText` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
          submitButton.textContent = loadingText;
        // Penjelasan: Memeriksa kondisi `submitButton instanceof HTMLInputElement`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
        } else if (submitButton instanceof HTMLInputElement) {
          // Penjelasan: Memeriksa kondisi `!submitButton.dataset.originalText`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
          if (!submitButton.dataset.originalText) {
            // Penjelasan: Memperbarui `submitButton.dataset.originalText` dengan membaca nilai `submitButton.value` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
            submitButton.dataset.originalText = submitButton.value;
          // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
          }
          // Penjelasan: Memperbarui `submitButton.value` dengan membaca nilai `loadingText` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
          submitButton.value = loadingText;
        // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
        }
      // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
      });
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();

// Penjelasan: Mendefinisikan fungsi atau pemetaan `(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
(() => {
  // Penjelasan: Menyimpan `tableWraps` dengan mencari seluruh elemen DOM yang cocok dengan selector `".table-wrap")`; daftar ini menjadi sasaran perilaku antarmuka.
  const tableWraps = Array.from(document.querySelectorAll(".table-wrap"));
  // Penjelasan: Memeriksa kondisi `!tableWraps.length`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
  if (!tableWraps.length) {
    // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
    return;
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  }

  // Penjelasan: Menyimpan `updateShadowState` dengan membaca nilai `(wrap) => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const updateShadowState = (wrap) => {
    // Penjelasan: Memeriksa kondisi `!(wrap instanceof HTMLElement)`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (!(wrap instanceof HTMLElement)) {
      // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
      return;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Menyimpan `isScrollable` dengan menghitung ekspresi `wrap.scrollWidth > wrap.clientWidth + 1` dengan urutan operator untuk memperoleh nilai turunan dari data masukan.
    const isScrollable = wrap.scrollWidth > wrap.clientWidth + 1;
    // Penjelasan: Melakukan operasi dengan membalik keberadaan kelas `"is-scrollable", isScrollable` dan menghasilkan status aktif setelah perubahan.
    wrap.classList.toggle("is-scrollable", isScrollable);
    // Penjelasan: Memeriksa kondisi `!isScrollable`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (!isScrollable) {
      // Penjelasan: Melakukan operasi dengan menghapus kelas `"is-scrolled-start"` sehingga status visual terkait dinonaktifkan.
      wrap.classList.remove("is-scrolled-start");
      // Penjelasan: Melakukan operasi dengan menghapus kelas `"is-scrolled-end"` sehingga status visual terkait dinonaktifkan.
      wrap.classList.remove("is-scrolled-end");
      // Penjelasan: Mengakhiri fungsi atau callback saat ini tanpa nilai hasil; langkah setelah return pada jalur ini dilewati.
      return;
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Menyimpan `maxScrollLeft` dengan mengambil nilai terbesar dari `0, wrap.scrollWidth - wrap.clientWidth` untuk menjaga hasil tidak di bawah batas minimum.
    const maxScrollLeft = Math.max(0, wrap.scrollWidth - wrap.clientWidth);
    // Penjelasan: Menyimpan `currentScrollLeft` dengan mengambil nilai terbesar dari `0, wrap.scrollLeft` untuk menjaga hasil tidak di bawah batas minimum.
    const currentScrollLeft = Math.max(0, wrap.scrollLeft);
    // Penjelasan: Melakukan operasi dengan membalik keberadaan kelas `"is-scrolled-start", currentScrollLeft > 2` dan menghasilkan status aktif setelah perubahan.
    wrap.classList.toggle("is-scrolled-start", currentScrollLeft > 2);
    // Penjelasan: Melakukan operasi dengan membalik keberadaan kelas `"is-scrolled-end", currentScrollLeft >= maxScrollLeft - 2` dan menghasilkan status aktif setelah perubahan.
    wrap.classList.toggle("is-scrolled-end", currentScrollLeft >= maxScrollLeft - 2);
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  };

  // Penjelasan: Mendefinisikan fungsi atau pemetaan `tableWraps.forEach((wrap) => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  tableWraps.forEach((wrap) => {
    // Penjelasan: Melakukan operasi dengan memanggil `updateShadowState(wrap)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    updateShadowState(wrap);
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `wrap.addEventListener("scroll", () => updateShadowState(wrap), { passive: true });`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    wrap.addEventListener("scroll", () => updateShadowState(wrap), { passive: true });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  });

  // Penjelasan: Menyimpan `resizeFrameId` dengan menggunakan konstanta numerik `0` sebagai nilai awal atau parameter perhitungan.
  let resizeFrameId = 0;
  // Penjelasan: Menyimpan `onResize` dengan membaca nilai `() => {` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya. Nilai berupa fungsi; badan callback dijalankan hanya ketika fungsi dipanggil.
  const onResize = () => {
    // Penjelasan: Memeriksa kondisi `resizeFrameId !== 0`; blok terkait hanya diproses bila kondisi benar sehingga jalur ini mengikuti keadaan data atau interaksi pengguna.
    if (resizeFrameId !== 0) {
      // Penjelasan: Melakukan operasi dengan memanggil `window.cancelAnimationFrame(resizeFrameId)` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
      window.cancelAnimationFrame(resizeFrameId);
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }

    // Penjelasan: Memperbarui `resizeFrameId` dengan memanggil `window.requestAnimationFrame(() => {` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
    resizeFrameId = window.requestAnimationFrame(() => {
      // Penjelasan: Mendefinisikan fungsi atau pemetaan `tableWraps.forEach((wrap) => updateShadowState(wrap));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
      tableWraps.forEach((wrap) => updateShadowState(wrap));
      // Penjelasan: Memperbarui `resizeFrameId` dengan menggunakan konstanta numerik `0` sebagai nilai awal atau parameter perhitungan.
      resizeFrameId = 0;
    // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
    });
  // Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
  };

  // Penjelasan: Melakukan operasi dengan mendaftarkan penangan event `"resize", onResize)`; fungsi callback akan dijalankan browser ketika event tersebut terjadi.
  window.addEventListener("resize", onResize);
  // Penjelasan: Mendefinisikan fungsi atau pemetaan `window.setTimeout(() => {`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
  window.setTimeout(() => {
    // Penjelasan: Mendefinisikan fungsi atau pemetaan `tableWraps.forEach((wrap) => updateShadowState(wrap));`; parameter di sisi kiri => dipakai badan di sisi kanan ketika fungsi dipanggil atau pola cocok.
    tableWraps.forEach((wrap) => updateShadowState(wrap));
  // Penjelasan: Melakukan operasi dengan membaca nilai `}, 280)` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
  }, 280);
// Penjelasan: Melakukan operasi dengan membaca nilai `})()` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya.
})();
