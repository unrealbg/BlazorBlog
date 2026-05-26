namespace BlazorBlog.Seo
{
    public static class RobotsTextDocument
    {
        public static string Create(Uri baseUri)
        {
            var sitemapUri = new Uri(baseUri, "sitemap.xml");

            return string.Join(
                "\n",
                "User-agent: *",
                "Allow: /",
                string.Empty,
                $"Sitemap: {sitemapUri.AbsoluteUri}",
                string.Empty);
        }
    }
}
