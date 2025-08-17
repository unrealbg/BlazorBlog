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
  initPostEnhancements();
});

document.addEventListener("click", (e) => {
  const target = e.target instanceof Element ? e.target.closest('[data-theme-toggle]') : null;
  if (!target) return;
  toggleTheme();
});

document.addEventListener('blazor:navigation-start', applyStoredTheme);
document.addEventListener('blazor:navigation-end', applyStoredTheme);
document.addEventListener('blazor:navigation-end', () => {
  applyStoredTheme();
  // Re-initialize page-specific enhancements after navigation
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
