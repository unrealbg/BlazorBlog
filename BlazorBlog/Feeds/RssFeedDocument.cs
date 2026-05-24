namespace BlazorBlog.Feeds
{
    using System.Globalization;
    using System.Xml.Linq;
    using BlazorBlog.Application.Models;

    public static class RssFeedDocument
    {
        private static readonly XNamespace ContentNamespace = "http://purl.org/rss/1.0/modules/content/";

        public static string Create(
            IEnumerable<BlogPostVm> posts,
            Uri baseUri,
            Uri feedUri,
            FeedOptions options,
            Func<string, string> sanitizeSummary,
            Func<string, string> renderSafeContent)
        {
            var channel = new XElement(
                "channel",
                new XElement("title", options.Title),
                new XElement("link", baseUri.AbsoluteUri),
                new XElement("description", options.Description),
                new XElement("language", "en"),
                new XElement("lastBuildDate", FormatRssDate(DateTime.UtcNow)));

            foreach (var post in posts)
            {
                var postUri = new Uri(baseUri, $"posts/{Uri.EscapeDataString(post.Slug)}");
                var publishedAt = (post.PublishedAt ?? DateTime.UtcNow).ToUniversalTime();
                var safeSummary = sanitizeSummary(post.Introduction ?? string.Empty);
                var safeContent = renderSafeContent(post.Content ?? string.Empty);

                channel.Add(
                    new XElement(
                        "item",
                        new XElement("title", post.Title),
                        new XElement("link", postUri.AbsoluteUri),
                        new XElement("guid", new XAttribute("isPermaLink", "true"), postUri.AbsoluteUri),
                        new XElement("pubDate", FormatRssDate(publishedAt)),
                        new XElement("description", new XCData(safeSummary)),
                        new XElement(ContentNamespace + "encoded", new XCData(safeContent))));
            }

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(
                    "rss",
                    new XAttribute("version", "2.0"),
                    new XAttribute(XNamespace.Xmlns + "content", ContentNamespace),
                    new XAttribute(XNamespace.Xmlns + "atom", "http://www.w3.org/2005/Atom"),
                    channel));

            channel.AddFirst(
                new XElement(
                    XName.Get("link", "http://www.w3.org/2005/Atom"),
                    new XAttribute("href", feedUri.AbsoluteUri),
                    new XAttribute("rel", "self"),
                    new XAttribute("type", "application/rss+xml")));

            return document.ToString(SaveOptions.DisableFormatting);
        }

        private static string FormatRssDate(DateTime value)
            => value.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);
    }
}
