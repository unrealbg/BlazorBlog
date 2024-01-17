
namespace BlazorBlog.Services.Common
{
    using System.Security.Claims;
    using System.Text.RegularExpressions;

    public static partial class Extensions
    {
        public static string GetUserName(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(AppConstants.ClaimNames.FullName)!;

        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public static string ToSlug(this string text)
        {
            text = text.ToLowerInvariant()
                .Replace(' ', '-');

            text = SlugRegex().Replace(text, "-");

            return text.Replace("--", "-")
                .Trim('-');
        }

        [GeneratedRegex(@"[^0-9a-z_]")]
        private static partial Regex SlugRegex();

        public static string ToDisplayDate(this DateTime? date) =>
			date?.ToString("MMM dd") ?? string.Empty;
    }
}
