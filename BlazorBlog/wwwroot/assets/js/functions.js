document.addEventListener("DOMContentLoaded", () => {
  // Mobile nav toggle for Tailwind header
  const menuBtn = document.getElementById("menuBtn");
  const mobileNav = document.getElementById("mobileNav");
  menuBtn?.addEventListener("click", () =>
    mobileNav?.classList.toggle("hidden")
  );

  // Theme toggle (dark/light)
  const themeBtn = document.getElementById("themeToggle");
  themeBtn?.addEventListener("click", () => {
    const root = document.documentElement;
    const isDark = root.classList.toggle("dark");
    localStorage.setItem("theme", isDark ? "dark" : "light");
  });
});
