namespace BlazorBlog.Tests.Seo
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using BlazorBlog.Application.Models;
    using BlazorBlog.Seo;
    using Xunit;

    public class SitemapDocumentTests
    {
        [Fact]
        public void Create_IncludesKeyPagesCategoriesAndPublishedPosts()
        {
            var posts = new[]
            {
                new SitemapPostVm
                {
                    Slug = "hello-world",
                    CategorySlug = "blazor",
                    LastModifiedUtc = new DateTime(2026, 5, 24, 10, 30, 0, DateTimeKind.Utc)
                },
                new SitemapPostVm
                {
                    Slug = "second-post",
                    CategorySlug = "csharp",
                    LastModifiedUtc = new DateTime(2026, 5, 20, 8, 0, 0, DateTimeKind.Utc)
                }
            };

            var xml = SitemapDocument.Create(
                new Uri("https://example.com/blog/"),
                posts,
                new DateTime(2026, 5, 25, 12, 0, 0, DateTimeKind.Utc));

            var document = XDocument.Parse(xml);
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var urls = document.Root!.Elements(ns + "url").ToArray();
            var locations = urls.Select(url => url.Element(ns + "loc")?.Value).ToArray();

            Assert.Equal("urlset", document.Root.Name.LocalName);
            Assert.Contains("https://example.com/blog/", locations);
            Assert.Contains("https://example.com/blog/posts", locations);
            Assert.Contains("https://example.com/blog/blazor-posts", locations);
            Assert.Contains("https://example.com/blog/csharp-posts", locations);
            Assert.Contains("https://example.com/blog/posts/hello-world", locations);
            Assert.Contains("https://example.com/blog/posts/second-post", locations);

            var firstPost = urls.Single(url => url.Element(ns + "loc")?.Value == "https://example.com/blog/posts/hello-world");
            Assert.Equal("2026-05-24T10:30:00Z", firstPost.Element(ns + "lastmod")?.Value);
            Assert.Equal("monthly", firstPost.Element(ns + "changefreq")?.Value);
            Assert.Equal("0.8", firstPost.Element(ns + "priority")?.Value);
        }

        [Fact]
        public void Create_SkipsPostsWithoutSlugs()
        {
            var posts = new[]
            {
                new SitemapPostVm
                {
                    Slug = "",
                    CategorySlug = "drafts",
                    LastModifiedUtc = DateTime.UtcNow
                }
            };

            var xml = SitemapDocument.Create(new Uri("https://example.com/"), posts, DateTime.UtcNow);
            var document = XDocument.Parse(xml);
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var locations = document.Root!.Elements(ns + "url")
                .Select(url => url.Element(ns + "loc")?.Value)
                .ToArray();

            Assert.DoesNotContain("https://example.com/posts/", locations);
            Assert.DoesNotContain("https://example.com/drafts-posts", locations);
        }
    }
}
