/* Fungsi file: Menyediakan skrip frontend global untuk perilaku interaktif umum pada UI. */
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

  const sectionShells = targets.filter((el) => el.classList.contains("section-shell"));
  const observedTargets = targets.filter((el) => !el.classList.contains("section-shell"));

  // Keep primary page containers visible from first paint.
  sectionShells.forEach((el) => el.classList.add("is-visible"));

  if (reduceMotion) {
    targets.forEach((el) => el.classList.add("is-visible"));
    return;
  }

  if (!observedTargets.length) {
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
    { threshold: 0.01 }
  );

  observedTargets.forEach((el) => observer.observe(el));

  // Failsafe: never leave elements hidden if observer callback misses.
  window.setTimeout(() => {
    observedTargets.forEach((el) => {
      if (el.classList.contains("is-visible")) {
        return;
      }

      el.classList.add("is-visible");
      observer.unobserve(el);
    });
  }, 1200);
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

  const navDropdowns = Array.from(navShell.querySelectorAll(".nav-dropdown"));
  const closeDropdowns = () => {
    navDropdowns.forEach((dropdown) => dropdown.removeAttribute("open"));
  };

  const closeMenu = () => {
    navShell.classList.remove("is-open");
    toggles.forEach((toggle) => toggle.setAttribute("aria-expanded", "false"));
    closeDropdowns();
  };

  toggles.forEach((toggle) => {
    toggle.addEventListener("click", () => {
      const isOpen = navShell.classList.toggle("is-open");
      toggle.setAttribute("aria-expanded", isOpen ? "true" : "false");
      if (!isOpen) {
        closeDropdowns();
      }
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

  dropdowns.forEach((dropdown) => {
    dropdown.addEventListener("toggle", () => {
      if (!dropdown.open) {
        return;
      }

      dropdowns.forEach((otherDropdown) => {
        if (otherDropdown === dropdown) {
          return;
        }

        otherDropdown.removeAttribute("open");
      });
    });
  });

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

(() => {
  const forms = Array.from(document.querySelectorAll("form.auth-form"));
  if (!forms.length) {
    return;
  }

  const focusableSelector = "input, select, textarea";
  const isNavigableField = (field) => {
    if (!(field instanceof HTMLElement)) {
      return false;
    }

    if (field.hasAttribute("disabled")) {
      return false;
    }

    if (field.getAttribute("type") === "hidden") {
      return false;
    }

    if (field.getAttribute("readonly") !== null) {
      return false;
    }

    return true;
  };

  forms.forEach((form) => {
    form.addEventListener("keydown", (event) => {
      if (event.key !== "Enter" || event.shiftKey || event.isComposing) {
        return;
      }

      const target = event.target;
      if (!(target instanceof HTMLElement)) {
        return;
      }

      if (target.tagName === "TEXTAREA") {
        return;
      }

      const fields = Array.from(form.querySelectorAll(focusableSelector)).filter(isNavigableField);
      if (!fields.length) {
        return;
      }

      const currentIndex = fields.indexOf(target);
      if (currentIndex < 0) {
        return;
      }

      const isLastField = currentIndex >= fields.length - 1;
      if (isLastField) {
        const submitButton = form.querySelector("button[type='submit'], input[type='submit']");
        if (!(submitButton instanceof HTMLElement)) {
          return;
        }

        const enterLastSelectMode = (form.dataset.enterLastSelect ?? "").toLowerCase();
        const isSelectField = target.tagName === "SELECT";
        if (isSelectField && enterLastSelectMode === "focus-submit") {
          event.preventDefault();
          submitButton.focus();
          return;
        }

        event.preventDefault();
        submitButton.click();
        return;
      }

      const nextField = fields[currentIndex + 1];
      if (!(nextField instanceof HTMLElement)) {
        return;
      }

      event.preventDefault();
      nextField.focus();
    });
  });
})();

(() => {
  const forms = Array.from(document.querySelectorAll("form")).filter((form) => {
    if (!(form instanceof HTMLFormElement)) {
      return false;
    }

    if (form.classList.contains("js-no-submit-lock")) {
      return false;
    }

    if (form.closest(".nav-menu")) {
      return false;
    }

    return true;
  });

  if (!forms.length) {
    return;
  }

  const defaultLoadingText = document.documentElement.lang.toLowerCase().startsWith("en")
    ? "Processing..."
    : "Memproses...";

  forms.forEach((form) => {
    form.addEventListener("submit", (event) => {
      if (form.dataset.submitting === "true") {
        event.preventDefault();
        return;
      }

      const submitButtons = Array.from(
        form.querySelectorAll("button[type='submit'], input[type='submit']")
      );
      if (!submitButtons.length) {
        return;
      }

      form.dataset.submitting = "true";
      form.classList.add("is-submitting");

      submitButtons.forEach((submitButton) => {
        if (!(submitButton instanceof HTMLButtonElement || submitButton instanceof HTMLInputElement)) {
          return;
        }

        if (submitButton.disabled) {
          return;
        }

        submitButton.disabled = true;
        submitButton.classList.add("is-loading");
        submitButton.setAttribute("aria-disabled", "true");

        const loadingText = submitButton.dataset.loadingText?.trim() || defaultLoadingText;
        if (submitButton instanceof HTMLButtonElement) {
          if (!submitButton.dataset.originalText) {
            submitButton.dataset.originalText = submitButton.textContent ?? "";
          }
          submitButton.textContent = loadingText;
        } else if (submitButton instanceof HTMLInputElement) {
          if (!submitButton.dataset.originalText) {
            submitButton.dataset.originalText = submitButton.value;
          }
          submitButton.value = loadingText;
        }
      });
    });
  });
})();

(() => {
  const tableWraps = Array.from(document.querySelectorAll(".table-wrap"));
  if (!tableWraps.length) {
    return;
  }

  const updateShadowState = (wrap) => {
    if (!(wrap instanceof HTMLElement)) {
      return;
    }

    const isScrollable = wrap.scrollWidth > wrap.clientWidth + 1;
    wrap.classList.toggle("is-scrollable", isScrollable);
    if (!isScrollable) {
      wrap.classList.remove("is-scrolled-start");
      wrap.classList.remove("is-scrolled-end");
      return;
    }

    const maxScrollLeft = Math.max(0, wrap.scrollWidth - wrap.clientWidth);
    const currentScrollLeft = Math.max(0, wrap.scrollLeft);
    wrap.classList.toggle("is-scrolled-start", currentScrollLeft > 2);
    wrap.classList.toggle("is-scrolled-end", currentScrollLeft >= maxScrollLeft - 2);
  };

  tableWraps.forEach((wrap) => {
    updateShadowState(wrap);
    wrap.addEventListener("scroll", () => updateShadowState(wrap), { passive: true });
  });

  let resizeFrameId = 0;
  const onResize = () => {
    if (resizeFrameId !== 0) {
      window.cancelAnimationFrame(resizeFrameId);
    }

    resizeFrameId = window.requestAnimationFrame(() => {
      tableWraps.forEach((wrap) => updateShadowState(wrap));
      resizeFrameId = 0;
    });
  };

  window.addEventListener("resize", onResize);
  window.setTimeout(() => {
    tableWraps.forEach((wrap) => updateShadowState(wrap));
  }, 280);
})();
