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
  const toggles = Array.from(document.querySelectorAll(".js-nav-toggle"));
  if (!toggles.length) {
    return;
  }

  const navShell = document.querySelector(".nav-shell");
  if (!navShell) {
    return;
  }

  const closeMenu = () => {
    navShell.classList.remove("is-open");
    toggles.forEach((toggle) => toggle.setAttribute("aria-expanded", "false"));
  };

  toggles.forEach((toggle) => {
    toggle.addEventListener("click", () => {
      const isOpen = navShell.classList.toggle("is-open");
      toggle.setAttribute("aria-expanded", isOpen ? "true" : "false");
    });
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
      closeMenu();
    }
  });

  window.addEventListener("resize", () => {
    if (window.innerWidth > 768) {
      closeMenu();
    }
  });

  const navLinks = Array.from(document.querySelectorAll(".nav-link, .nav-menu-item"));
  navLinks.forEach((link) => {
    link.addEventListener("click", () => {
      if (window.innerWidth <= 768) {
        closeMenu();
      }
    });
  });
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

(() => {
  const switches = Array.from(document.querySelectorAll(".js-auth-switch"));
  const card = document.querySelector(".auth-split-card");

  if (!switches.length || !card) {
    return;
  }

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  switches.forEach((link) => {
    link.addEventListener("click", (event) => {
      if (reduceMotion) {
        return;
      }

      if (event.defaultPrevented || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey) {
        return;
      }

      const href = link.getAttribute("href");
      if (!href) {
        return;
      }

      event.preventDefault();
      const direction = link.dataset.authSwitchDirection === "right" ? "right" : "left";
      card.classList.remove("auth-switching-left", "auth-switching-right");
      card.classList.add(direction === "right" ? "auth-switching-right" : "auth-switching-left");

      window.setTimeout(() => {
        window.location.assign(href);
      }, 240);
    });
  });
})();
