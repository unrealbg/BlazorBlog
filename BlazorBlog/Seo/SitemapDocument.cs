namespace BlazorBlog.Seo
{
    using System.Globalization;
    using System.Xml.Linq;
    using BlazorBlog.Application.Models;

    public static class SitemapDocument
    {
        private static readonly XNamespace SitemapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

        public static string Create(Uri baseUri, IEnumerable<SitemapPostVm> posts, DateTime generatedAtUtc)
        {
            var normalizedPosts = posts
                .Where(post => !string.IsNullOrWhiteSpace(post.Slug))
                .Select(post => new SitemapPostVm
                {
                    Slug = post.Slug.Trim('/'),
                    CategorySlug = post.CategorySlug.Trim('/'),
                    LastModifiedUtc = EnsureUtc(post.LastModifiedUtc)
                })
                .ToArray();

            var latestPostChange = normalizedPosts.Length > 0
                ? normalizedPosts.Max(post => post.LastModifiedUtc)
                : EnsureUtc(generatedAtUtc);

            var urls = BuildKeyPages(latestPostChange)
                .Concat(BuildCategoryPages(normalizedPosts))
                .Concat(BuildPostPages(normalizedPosts))
                .Select(url => new XElement(
                    SitemapNamespace + "url",
                    new XElement(SitemapNamespace + "loc", BuildAbsoluteUrl(baseUri, url.Location)),
                    new XElement(SitemapNamespace + "lastmod", FormatLastModified(url.LastModifiedUtc)),
                    new XElement(SitemapNamespace + "changefreq", url.ChangeFrequency),
                    new XElement(SitemapNamespace + "priority", url.Priority)));

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(SitemapNamespace + "urlset", urls));

            return document.ToString(SaveOptions.DisableFormatting);
        }

        private static IEnumerable<SitemapUrl> BuildKeyPages(DateTime lastModifiedUtc)
        {
            yield return new SitemapUrl(string.Empty, lastModifiedUtc, "daily", "1.0");
            yield return new SitemapUrl("posts", lastModifiedUtc, "daily", "0.9");
            yield return new SitemapUrl("privacy", lastModifiedUtc, "yearly", "0.3");
            yield return new SitemapUrl("terms", lastModifiedUtc, "yearly", "0.3");
        }

        private static IEnumerable<SitemapUrl> BuildCategoryPages(IEnumerable<SitemapPostVm> posts)
            => posts
                .Where(post => !string.IsNullOrWhiteSpace(post.CategorySlug))
                .GroupBy(post => post.CategorySlug, StringComparer.OrdinalIgnoreCase)
                .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
                .Select(group => new SitemapUrl(
                    $"{group.Key}-posts",
                    group.Max(post => post.LastModifiedUtc),
                    "weekly",
                    "0.7"));

        private static IEnumerable<SitemapUrl> BuildPostPages(IEnumerable<SitemapPostVm> posts)
            => posts
                .OrderByDescending(post => post.LastModifiedUtc)
                .ThenBy(post => post.Slug, StringComparer.OrdinalIgnoreCase)
                .Select(post => new SitemapUrl($"posts/{post.Slug}", post.LastModifiedUtc, "monthly", "0.8"));

        private static string BuildAbsoluteUrl(Uri baseUri, string relativePath)
            => string.IsNullOrWhiteSpace(relativePath)
                ? baseUri.AbsoluteUri
                : new Uri(baseUri, relativePath).AbsoluteUri;

        private static string FormatLastModified(DateTime value)
            => EnsureUtc(value).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);

        private static DateTime EnsureUtc(DateTime value)
            => value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
    }
}
