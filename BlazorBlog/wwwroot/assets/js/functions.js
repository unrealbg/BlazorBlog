document.addEventListener("DOMContentLoaded", () => {
  // Mobile nav toggle for Tailwind header
  const menuBtn = document.getElementById("menuBtn");
  const mobileNav = document.getElementById("mobileNav");
  menuBtn?.addEventListener("click", () => mobileNav?.classList.toggle("hidden"));

  applyStoredTheme();

  ensureThemeConsistency();
});

document.addEventListener("click", (e) => {
  const target = e.target instanceof Element ? e.target.closest('[data-theme-toggle]') : null;
  if (!target) return;
  toggleTheme();
});

document.addEventListener('blazor:navigation-start', applyStoredTheme);
document.addEventListener('blazor:navigation-end', applyStoredTheme);

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
