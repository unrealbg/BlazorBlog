namespace BlazorBlog.Tests.Feeds
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using BlazorBlog.Application.Models;
    using BlazorBlog.Feeds;
    using Xunit;

    public class RssFeedDocumentTests
    {
        [Fact]
        public void Create_ProducesValidRssWithAbsoluteUrlsAndUtcDates()
        {
            var posts = new[]
            {
                new BlogPostVm
                {
                    Title = "Hello RSS",
                    Slug = "hello-rss",
                    Introduction = "Intro",
                    Content = "<p>Safe content</p>",
                    PublishedAt = new DateTime(2026, 5, 24, 10, 30, 0, DateTimeKind.Utc)
                }
            };

            var xml = RssFeedDocument.Create(
                posts,
                new Uri("https://example.com/blog/"),
                new Uri("https://example.com/blog/feed"),
                new FeedOptions { Title = "Test Blog", Description = "Latest posts" },
                summary => summary,
                content => content);

            var document = XDocument.Parse(xml);
            XNamespace contentNamespace = "http://purl.org/rss/1.0/modules/content/";
            XNamespace atomNamespace = "http://www.w3.org/2005/Atom";

            Assert.Equal("rss", document.Root?.Name.LocalName);
            Assert.Equal("2.0", document.Root?.Attribute("version")?.Value);

            var channel = document.Root?.Element("channel");
            Assert.NotNull(channel);
            Assert.Equal("Test Blog", channel!.Element("title")?.Value);
            Assert.Equal("https://example.com/blog/", channel.Element("link")?.Value);

            var selfLink = channel.Elements(atomNamespace + "link").Single();
            Assert.Equal("https://example.com/blog/feed", selfLink.Attribute("href")?.Value);

            var item = channel.Elements("item").Single();
            Assert.Equal("https://example.com/blog/posts/hello-rss", item.Element("link")?.Value);
            Assert.Equal("Sun, 24 May 2026 10:30:00 GMT", item.Element("pubDate")?.Value);
            Assert.Equal("<p>Safe content</p>", item.Element(contentNamespace + "encoded")?.Value);
        }

        [Fact]
        public void Create_SanitizesSummaryAndContentThroughCallbacks()
        {
            var posts = new[]
            {
                new BlogPostVm
                {
                    Title = "Unsafe",
                    Slug = "unsafe",
                    Introduction = "<script>alert(1)</script>Summary",
                    Content = "<script>alert(2)</script>Content",
                    PublishedAt = DateTime.UtcNow
                }
            };

            var xml = RssFeedDocument.Create(
                posts,
                new Uri("https://example.com/"),
                new Uri("https://example.com/feed"),
                new FeedOptions(),
                summary => summary.Replace("<script>alert(1)</script>", string.Empty, StringComparison.Ordinal),
                content => content.Replace("<script>alert(2)</script>", string.Empty, StringComparison.Ordinal));

            var document = XDocument.Parse(xml);
            XNamespace contentNamespace = "http://purl.org/rss/1.0/modules/content/";
            var item = document.Root!.Element("channel")!.Element("item")!;

            Assert.Equal("Summary", item.Element("description")?.Value);
            Assert.Equal("Content", item.Element(contentNamespace + "encoded")?.Value);
        }
    }
}
