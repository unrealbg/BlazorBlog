namespace BlazorBlog.Seo
{
    public sealed class SitemapOptions
    {
        public const string SectionName = "Sitemap";

        public int CacheMinutes { get; set; } = 10;
    }
}
