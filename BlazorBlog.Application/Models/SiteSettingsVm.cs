namespace BlazorBlog.Application.Models
{
    using System.ComponentModel.DataAnnotations;

    public class SiteSettingsVm : IValidatableObject
    {
        public int Id { get; set; }

        [Required, MaxLength(180)]
        public string HomeHeroTitle { get; set; } = "Modern Blazor Blog Platform with Clean Architecture";

        [Required, MaxLength(500)]
        public string HomeHeroSubtitle { get; set; } =
            "Professional blog project built with .NET 10, Blazor Server, Entity Framework Core and Tailwind CSS. Demonstrates modern development techniques and Clean Architecture principles.";

        [MaxLength(180)]
        public string HomeHeroTags { get; set; } = "Blazor, .NET 10, Clean Architecture";

        [Required, MaxLength(60)]
        public string HomeHeroPrimaryButtonText { get; set; } = "Browse Posts";

        [Required, MaxLength(300)]
        public string HomeHeroPrimaryButtonUrl { get; set; } = "/posts";

        [MaxLength(60)]
        public string HomeHeroSecondaryButtonText { get; set; } = "GitHub Repository";

        [MaxLength(300)]
        public string HomeHeroSecondaryButtonUrl { get; set; } = "https://github.com/unrealbg/BlazorBlog";

        public static SiteSettingsVm CreateDefault() => new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in ValidateLink(
                HomeHeroPrimaryButtonText,
                HomeHeroPrimaryButtonUrl,
                nameof(HomeHeroPrimaryButtonText),
                nameof(HomeHeroPrimaryButtonUrl),
                required: true))
            {
                yield return result;
            }

            foreach (var result in ValidateLink(
                HomeHeroSecondaryButtonText,
                HomeHeroSecondaryButtonUrl,
                nameof(HomeHeroSecondaryButtonText),
                nameof(HomeHeroSecondaryButtonUrl),
                required: false))
            {
                yield return result;
            }
        }

        private static IEnumerable<ValidationResult> ValidateLink(
            string? text,
            string? url,
            string textMemberName,
            string urlMemberName,
            bool required)
        {
            var hasText = !string.IsNullOrWhiteSpace(text);
            var hasUrl = !string.IsNullOrWhiteSpace(url);

            if (required || hasText || hasUrl)
            {
                if (!hasText)
                {
                    yield return new ValidationResult("Button text is required.", [textMemberName]);
                }

                if (!hasUrl)
                {
                    yield return new ValidationResult("Button URL is required.", [urlMemberName]);
                    yield break;
                }

                if (!IsSafeLink(url!))
                {
                    yield return new ValidationResult("Use a relative path or an http/https URL.", [urlMemberName]);
                }
            }
        }

        private static bool IsSafeLink(string url)
        {
            if (url.StartsWith("/", StringComparison.Ordinal) &&
                !url.StartsWith("//", StringComparison.Ordinal) &&
                !url.StartsWith("/\\", StringComparison.Ordinal))
            {
                return true;
            }

            return Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri) &&
                (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
