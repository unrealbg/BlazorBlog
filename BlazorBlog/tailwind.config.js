/***** Tailwind CSS configuration for BlazorBlog *****/
/** @type {import('tailwindcss').Config} */
module.exports = {
  // Limit scan scope for performance and to avoid node_modules
  content: [
    "./Components/**/*.razor",
    "./**/*.cshtml",
    "./**/*.html",
    "./wwwroot/assets/js/**/*.js",
  ],
  // Ensure dynamic classes used from C# strings are not purged
  safelist: [
    "fixed",
    "pointer-events-none",
    "top-4",
    "bottom-4",
    "right-4",
    "z-[9999]",
    // post detail enhancements
    "z-[60]",
    "bg-brand-600",
    "prose",
    "prose-slate",
    "dark:prose-invert",
    // New classes for the redesigned home page
    "line-clamp-3",
    "aspect-video",
    "bg-slate-800",
    "bg-slate-700",
    "bg-slate-900",
    "text-white",
    "text-slate-300",
    "text-slate-400",
    "text-brand-400",
    "text-brand-300",
    "hover:bg-slate-700",
    "hover:text-brand-300",
    "bg-gradient-to-br",
    "from-slate-700",
    "to-slate-800",
    // Image positioning classes
    "object-cover",
    "object-center",
    "transition-transform",
    "duration-300",
    "group-hover:scale-105",
    "overflow-hidden",
    // Flexbox layout classes for consistent card heights
    "flex",
    "h-full",
    "flex-col",
    "flex-1",
    "mt-auto",
  ],
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        brand: {
          50: "#eef2ff",
          100: "#e0e7ff",
          200: "#c7d2fe",
          300: "#a5b4fc",
          400: "#818cf8",
          500: "#6366f1",
          600: "#4f46e5",
          700: "#4338ca",
          800: "#3730a3",
          900: "#312e81",
        },
        accent: { 400: "#f59e0b", 500: "#f59e0b", 600: "#d97706" },
      },
      boxShadow: { soft: "0 10px 30px -12px rgba(0,0,0,0.25)" },
    },
  },
  plugins: [require("@tailwindcss/typography")],
};
