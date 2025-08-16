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
