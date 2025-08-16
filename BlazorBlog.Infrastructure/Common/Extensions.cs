namespace BlazorBlog.Infrastructure.Common
{
    using System.Security.Claims;
    using System.Text.RegularExpressions;

    public static class Extensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
            => user?.Identity?.Name ?? "User";

        public static string GetUserId(this ClaimsPrincipal user)
            => user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        public static string ToSlug(this string text)
        {
            text = text.ToLowerInvariant().Replace(' ', '-');
            text = Regex.Replace(text, "[^0-9a-z_-]", "-");
            return text.Replace("--", "-").Trim('-');
        }

        public static string ToDisplayDate(this DateTime? date)
            => date?.ToString("MMM dd") ?? string.Empty;
    }
}
