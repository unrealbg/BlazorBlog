namespace BlazorBlog.Seo
{
    public sealed record SitemapUrl(string Location, DateTime LastModifiedUtc, string ChangeFrequency, string Priority);
}
