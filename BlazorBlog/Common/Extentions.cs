namespace BlazorBlog.Common
{
    using System.Security.Claims;

    public static class Extentions
    {
        public static string GetUserName(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(AppConstants.ClaimNames.FullName)!;

        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
