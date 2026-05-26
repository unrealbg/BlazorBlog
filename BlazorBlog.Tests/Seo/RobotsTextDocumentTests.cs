namespace BlazorBlog.Tests.Seo
{
    using System;
    using BlazorBlog.Seo;
    using Xunit;

    public class RobotsTextDocumentTests
    {
        [Fact]
        public void Create_IncludesSitemapLink()
        {
            var robots = RobotsTextDocument.Create(new Uri("https://example.com/blog/"));

            Assert.Contains("User-agent: *", robots);
            Assert.Contains("Allow: /", robots);
            Assert.Contains("Sitemap: https://example.com/blog/sitemap.xml", robots);
        }
    }
}
