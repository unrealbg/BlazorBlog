namespace BlazorBlog.Infrastructure.Persistence.Entities
{
    public class SiteSettings
    {
        public int Id { get; set; }

        public string HomeHeroTitle { get; set; } = string.Empty;

        public string HomeHeroSubtitle { get; set; } = string.Empty;

        public string HomeHeroTags { get; set; } = string.Empty;

        public string HomeHeroPrimaryButtonText { get; set; } = string.Empty;

        public string HomeHeroPrimaryButtonUrl { get; set; } = string.Empty;

        public string HomeHeroSecondaryButtonText { get; set; } = string.Empty;

        public string HomeHeroSecondaryButtonUrl { get; set; } = string.Empty;

        public string HomeAboutTitle { get; set; } = string.Empty;

        public string HomeAboutDescription { get; set; } = string.Empty;

        public string HomeAboutLinkText { get; set; } = string.Empty;

        public string HomeAboutLinkUrl { get; set; } = string.Empty;
    }
}
