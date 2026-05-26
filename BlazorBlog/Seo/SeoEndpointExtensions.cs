namespace BlazorBlog.Seo
{
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Infrastructure.Utilities;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    public static class SeoEndpointExtensions
    {
        public static IEndpointRouteBuilder MapSeoEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(
                    "/sitemap.xml",
                    async (
                        HttpContext context,
                        IBlogPostService blogPostService,
                        IMemoryCache cache,
                        IBlogCacheSignal cacheSignal,
                        IOptions<SitemapOptions> options,
                        CancellationToken cancellationToken) =>
                    {
                        var sitemapOptions = Normalize(options.Value);
                        var baseUri = GetBaseUri(context.Request);
                        var cacheKey = BuildSitemapCacheKey(baseUri, cacheSignal.Version);

                        var xml = await cache.GetOrCreateAsync(
                            cacheKey,
                            async entry =>
                            {
                                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(sitemapOptions.CacheMinutes);

                                var posts = await blogPostService.GetSitemapPostsAsync(cancellationToken);
                                return SitemapDocument.Create(baseUri, posts, DateTime.UtcNow);
                            });

                        return Results.Content(xml ?? string.Empty, "application/xml; charset=utf-8");
                    })
                .WithName("Sitemap")
                .WithTags("SEO");

            endpoints.MapGet(
                    "/robots.txt",
                    (HttpContext context) =>
                    {
                        var baseUri = GetBaseUri(context.Request);
                        return Results.Text(RobotsTextDocument.Create(baseUri), "text/plain; charset=utf-8");
                    })
                .WithName("RobotsTxt")
                .WithTags("SEO");

            return endpoints;
        }

        public static string BuildSitemapCacheKey(Uri baseUri, long signalVersion)
            => $"sitemap:xml:{baseUri.AbsoluteUri}:{signalVersion}";

        public static SitemapOptions Normalize(SitemapOptions options)
            => new()
            {
                CacheMinutes = Math.Clamp(options.CacheMinutes, 1, 1440)
            };

        private static Uri GetBaseUri(HttpRequest request)
        {
            var pathBase = request.PathBase.HasValue ? request.PathBase.Value : string.Empty;
            return new Uri($"{request.Scheme}://{request.Host}{pathBase}/");
        }
    }
}
