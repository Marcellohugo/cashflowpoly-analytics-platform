(() => {
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const targets = Array.from(
    document.querySelectorAll(".section-shell, .stat-card, .table-wrap")
  );

  if (!targets.length) {
    return;
  }

  targets.forEach((el, index) => {
    el.classList.add("reveal");
    const delay = Math.min(index * 70, 350);
    el.style.setProperty("--delay", `${delay}ms`);
  });

  if (reduceMotion) {
    targets.forEach((el) => el.classList.add("is-visible"));
    return;
  }

  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (!entry.isIntersecting) {
          return;
        }
        entry.target.classList.add("is-visible");
        observer.unobserve(entry.target);
      });
    },
    { threshold: 0.15 }
  );

  targets.forEach((el) => observer.observe(el));
})();

(() => {
  const dropdowns = Array.from(document.querySelectorAll(".nav-dropdown"));
  if (!dropdowns.length) {
    return;
  }

  document.addEventListener("click", (event) => {
    const target = event.target;
    dropdowns.forEach((dropdown) => {
      if (dropdown.contains(target)) {
        return;
      }
      dropdown.removeAttribute("open");
    });
  });

  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") {
      return;
    }
    dropdowns.forEach((dropdown) => dropdown.removeAttribute("open"));
  });
})();
