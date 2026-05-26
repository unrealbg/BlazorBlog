namespace BlazorBlog.Application.Models
{
    public sealed class SitemapPostVm
    {
        public string Slug { get; set; } = string.Empty;

        public string CategorySlug { get; set; } = string.Empty;

        public DateTime LastModifiedUtc { get; set; }
    }
}
