// --- Global share helpers so inline onclick handlers always find them ---
(function defineShareHelpers(){
  if (!window.share) {
    window.share = function(network){
      try {
        const url = location.href; const title = document.title;
        const map = {
          // Use X (formerly Twitter) share endpoint; keep key 'twitter' for back-compat with markup
          twitter: `https://x.com/intent/post?url=${encodeURIComponent(url)}&text=${encodeURIComponent(title)}`,
          linkedin: `https://www.linkedin.com/shareArticle?mini=true&url=${encodeURIComponent(url)}&title=${encodeURIComponent(title)}`
        };
        const target = map[network];
        if (target) window.open(target, '_blank', 'noopener,noreferrer');
      } catch { /* no-op */ }
    }
  }
  if (!window.copyLink) {
    window.copyLink = function(){
      try { navigator.clipboard.writeText(location.href); } catch { /* no-op */ }
      try { alert('Link copied!'); } catch { /* no-op */ }
    }
  }
})();

document.addEventListener("DOMContentLoaded", () => {
  // Mobile nav toggle for Tailwind header
  const menuBtn = document.getElementById("menuBtn");
  const mobileNav = document.getElementById("mobileNav");
  menuBtn?.addEventListener("click", () => mobileNav?.classList.toggle("hidden"));

  applyStoredTheme();

  ensureThemeConsistency();
  initStatusToasts();
  initPostEnhancements();
});

document.addEventListener("click", (e) => {
  const target = e.target instanceof Element ? e.target.closest('[data-theme-toggle]') : null;
  if (!target) return;
  toggleTheme();
});

document.addEventListener("click", (e) => {
  const button = e.target instanceof Element ? e.target.closest("[data-password-toggle]") : null;
  if (!button) return;

  const selector = button.getAttribute("data-password-toggle");
  const input = selector ? document.querySelector(selector) : null;

  if (!(input instanceof HTMLInputElement)) return;

  const showPassword = input.type === "password";
  input.type = showPassword ? "text" : "password";
  button.setAttribute("aria-pressed", String(showPassword));
  button.setAttribute("aria-label", showPassword ? "Hide password" : "Show password");
});

document.addEventListener('blazor:navigation-start', applyStoredTheme);
document.addEventListener('blazor:navigation-end', applyStoredTheme);
document.addEventListener('blazor:navigation-end', () => {
  applyStoredTheme();
  // Re-initialize page-specific enhancements after navigation
  initStatusToasts();
  initPostEnhancements();
});

window.addEventListener("storage", (e) => {
  if (e.key === "theme") {
    applyStoredTheme();
  }
});

try {
  const media = window.matchMedia('(prefers-color-scheme: dark)');
  media.addEventListener('change', () => {
    if (!getStoredTheme()) {
      applyStoredTheme();
    }
  });
} catch { /* no-op */ }

function getCookie(name) {
  return document.cookie.split('; ').find(x => x.startsWith(`${name}=`))?.split('=')[1];
}

function getStoredTheme() {
  return localStorage.getItem("theme") || getCookie('theme');
}

function applyStoredTheme() {
  const root = document.documentElement;
  const stored = getStoredTheme();
  const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
  const shouldDark = stored ? stored === "dark" : prefersDark;
  root.classList.toggle("dark", shouldDark);
}

function toggleTheme() {
  const root = document.documentElement;
  const isDark = root.classList.toggle("dark");
  const value = isDark ? "dark" : "light";
  try {
    localStorage.setItem("theme", value);
  } catch { /* no-op */ }
  try {
    document.cookie = `theme=${value}; path=/; max-age=${60 * 60 * 24 * 365}; SameSite=Lax`;
  } catch { /* no-op */ }
}

function ensureThemeConsistency() {
  const root = document.documentElement;
  const observer = new MutationObserver(() => {
    const stored = getStoredTheme();
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const desiredDark = stored ? stored === 'dark' : prefersDark;
    const hasDark = root.classList.contains('dark');
    if (hasDark !== desiredDark) {
      root.classList.toggle('dark', desiredDark);
    }
  });
  observer.observe(root, { attributes: true, attributeFilter: ['class'] });
}

function initStatusToasts() {
  document.querySelectorAll("[data-status-toast]:not([data-toast-shown='true'])").forEach((element) => {
    element.setAttribute("data-toast-shown", "true");

    showClientToast({
      level: element.getAttribute("data-toast-level") || "Info",
      heading: element.getAttribute("data-toast-heading") || "",
      message: element.getAttribute("data-toast-message") || "",
      durationMs: Number(element.getAttribute("data-toast-duration-ms")) || undefined
    });
  });
}

function showClientToast({ level = "Info", heading = "", message = "", durationMs } = {}) {
  if (!message && !heading) return;

  const normalizedLevel = ["Success", "Warning", "Error", "Info"].includes(level) ? level : "Info";
  const timeoutMs = durationMs || getClientToastDuration(normalizedLevel);
  const container = getClientToastContainer();
  const toast = document.createElement("div");

  toast.className = "pointer-events-auto w-[360px] max-w-[92vw] rounded-md border border-slate-200/70 bg-white/95 p-0 text-slate-800 shadow-lg backdrop-blur transition dark:border-slate-800 dark:bg-slate-900/95 dark:text-slate-100";
  toast.setAttribute("role", "status");
  toast.style.animation = "toast-slide-in .22s ease-out";

  const accentClass = getClientToastAccentClass(normalizedLevel);
  const displayHeading = heading || normalizedLevel;

  const content = document.createElement("div");
  content.className = "flex items-start gap-3 p-3";

  const icon = document.createElement("div");
  icon.className = `mt-0.5 inline-flex h-5 w-5 shrink-0 items-center justify-center rounded-full text-white shadow ${accentClass}`;
  icon.setAttribute("aria-hidden", "true");
  icon.innerHTML = getClientToastIcon(normalizedLevel);

  const text = document.createElement("div");
  text.className = "min-w-0 flex-1";

  const header = document.createElement("div");
  header.className = "flex items-start justify-between gap-3";

  const title = document.createElement("p");
  title.className = "m-0 truncate text-sm font-semibold";
  title.textContent = displayHeading;

  const close = document.createElement("button");
  close.type = "button";
  close.className = "-m-1 inline-flex h-7 w-7 items-center justify-center rounded-md text-slate-400 transition hover:text-slate-600 focus:outline-none focus:ring-2 focus:ring-brand-500 dark:text-slate-400 dark:hover:text-slate-200";
  close.title = "Dismiss";
  close.setAttribute("aria-label", "Dismiss notification");
  close.innerHTML = "<svg viewBox='0 0 24 24' width='16' height='16' fill='currentColor' aria-hidden='true'><path d='M18.3 5.7 12 12l6.3 6.3-1.4 1.4L10.6 13.4 4.3 19.7 2.9 18.3 9.2 12 2.9 5.7 4.3 4.3l6.3 6.3 6.3-6.3z'/></svg>";
  close.addEventListener("click", () => removeClientToast(toast));

  const body = document.createElement("div");
  body.className = "mt-1 text-sm text-slate-600 dark:text-slate-300";
  body.textContent = message || displayHeading;

  const progressWrap = document.createElement("div");
  progressWrap.className = "h-0.5 w-full overflow-hidden bg-slate-200 dark:bg-slate-800";

  const progress = document.createElement("i");
  progress.className = `block h-full w-full origin-left ${accentClass}`;
  progress.style.animation = `toast-progress ${timeoutMs}ms linear forwards`;

  header.append(title, close);
  text.append(header, body);
  content.append(icon, text);
  progressWrap.append(progress);
  toast.append(content, progressWrap);
  container.prepend(toast);

  window.setTimeout(() => removeClientToast(toast), timeoutMs);
}

function getClientToastContainer() {
  let container = document.getElementById("client-toast-container");

  if (!container) {
    container = document.createElement("div");
    container.id = "client-toast-container";
    container.className = "pointer-events-none fixed top-4 right-4 z-[9999]";
    container.setAttribute("aria-live", "polite");
    container.setAttribute("aria-atomic", "true");

    const stack = document.createElement("div");
    stack.className = "flex max-w-[92vw] flex-col items-end gap-2";
    container.append(stack);
    document.body.append(container);
  }

  return container.firstElementChild || container;
}

function removeClientToast(toast) {
  toast.style.opacity = "0";
  toast.style.transform = "translate3d(16px,-8px,0)";
  window.setTimeout(() => toast.remove(), 150);
}

function getClientToastDuration(level) {
  switch (level) {
    case "Success": return 5000;
    case "Warning": return 10000;
    case "Error": return 15000;
    default: return 10000;
  }
}

function getClientToastAccentClass(level) {
  switch (level) {
    case "Success": return "bg-emerald-600";
    case "Warning": return "bg-amber-500";
    case "Error": return "bg-rose-600";
    default: return "bg-brand-600";
  }
}

function getClientToastIcon(level) {
  switch (level) {
    case "Success":
      return "<svg viewBox='0 0 24 24' width='14' height='14' fill='currentColor' aria-hidden='true'><path d='M9 16.2 4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4z'/></svg>";
    case "Warning":
      return "<svg viewBox='0 0 24 24' width='14' height='14' fill='currentColor' aria-hidden='true'><path d='M1 21h22L12 2 1 21zm12-3h-2v2h2v-2zm0-8h-2v6h2V10z'/></svg>";
    case "Error":
      return "<svg viewBox='0 0 24 24' width='14' height='14' fill='currentColor' aria-hidden='true'><path d='M12 2C6.5 2 2 6.5 2 12s4.5 10 10 10 10-4.5 10-10S17.5 2 12 2zm5 13.6L15.6 17 12 13.4 8.4 17 7 15.6 10.6 12 7 8.4 8.4 7 12 10.6 15.6 7 17 8.4 13.4 12 17 15.6z'/></svg>";
    default:
      return "<svg viewBox='0 0 24 24' width='14' height='14' fill='currentColor' aria-hidden='true'><path d='M11 7h2v6h-2zm0 8h2v2h-2z'/><path d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z'/></svg>";
  }
}

// --- Post page enhancements: reading progress, ToC ---
function initPostEnhancements() {
  const body = document.getElementById('post-body');
  const progressBar = document.getElementById('readProgress');
  const toc = document.getElementById('toc');

  // Clean up previous listeners/observers if any
  if (window.__postScrollHandler) {
    document.removeEventListener('scroll', window.__postScrollHandler);
  }
  if (window.__postObserver && typeof window.__postObserver.disconnect === 'function') {
    window.__postObserver.disconnect();
  }

  if (!body) {
    // Not a post page; nothing to setup
    return;
  }

  // Build ToC
  if (toc) {
    toc.innerHTML = '';
    const headings = Array.from(body.querySelectorAll('h2, h3'));
    const makeId = (s) => s.toLowerCase().replace(/[^a-z0-9]+/g,'-').replace(/(^-|-$)/g,'')
;
    headings.forEach(h => { if (!h.id) h.id = makeId(h.textContent || ''); });

    const frag = document.createDocumentFragment();
    headings.forEach(h => {
      const a = document.createElement('a');
      a.href = `#${h.id}`;
      a.textContent = h.textContent || '';
      a.className = `block rounded-lg px-3 py-1 hover:bg-slate-100 dark:hover:bg-slate-800 ${h.tagName === 'H3' ? 'ml-3 text-slate-600' : 'font-medium'}`;
      frag.appendChild(a);
    });
    toc.appendChild(frag);

    // Active section highlight
    try {
      const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
          const link = toc.querySelector(`a[href="#${entry.target.id}"]`);
          if (!link) return;
          if (entry.isIntersecting) {
            toc.querySelectorAll('a').forEach(a => a.classList.remove('text-brand-600'));
            link.classList.add('text-brand-600');
          }
        });
      }, { rootMargin: '0px 0px -80% 0px', threshold: 0 });
      headings.forEach(h => observer.observe(h));
      window.__postObserver = observer;
    } catch { /* no-op */ }
  }

  // Reading progress
  if (progressBar) {
    const onScroll = () => {
      const el = document.documentElement;
      const h = el.scrollHeight - el.clientHeight;
      const p = h ? (el.scrollTop / h) * 100 : 0;
      progressBar.style.width = p + '%';
    };
    window.__postScrollHandler = onScroll;
    document.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
  }
}
