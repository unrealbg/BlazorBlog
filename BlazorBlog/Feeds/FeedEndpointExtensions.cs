namespace BlazorBlog.Feeds
{
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Infrastructure.Utilities;
    using BlazorBlog.Utilities;
    using Ganss.Xss;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    public static class FeedEndpointExtensions
    {
        public static IEndpointRouteBuilder MapFeedEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(
                    "/feed",
                    async (
                        HttpContext context,
                        IBlogPostService blogPostService,
                        IHtmlSanitizer htmlSanitizer,
                        IMemoryCache cache,
                        IBlogCacheSignal cacheSignal,
                        IOptions<FeedOptions> options,
                        CancellationToken cancellationToken) =>
                    {
                        var feedOptions = Normalize(options.Value);
                        var baseUri = GetBaseUri(context.Request);
                        var feedUri = new Uri(baseUri, "feed");
                        var cacheKey = BuildCacheKey(baseUri, feedOptions.ItemCount, cacheSignal.Version);

                        var xml = await cache.GetOrCreateAsync(
                            cacheKey,
                            async entry =>
                            {
                                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(feedOptions.CacheMinutes);

                                var posts = await blogPostService.GetRecentBlogPostsAsync(
                                    feedOptions.ItemCount,
                                    cancellationToken: cancellationToken);

                                return RssFeedDocument.Create(
                                    posts,
                                    baseUri,
                                    feedUri,
                                    feedOptions,
                                    summary => htmlSanitizer.Sanitize(summary),
                                    content => BlogContentRenderer.RenderSafeHtml(content, htmlSanitizer));
                            });

                        return Results.Content(xml ?? string.Empty, "application/rss+xml; charset=utf-8");
                    })
                .WithName("Feed")
                .WithTags("SEO");

            return endpoints;
        }

        public static string BuildCacheKey(Uri baseUri, int itemCount, long signalVersion)
            => $"feed:rss:{baseUri.AbsoluteUri}:{itemCount}:{signalVersion}";

        public static FeedOptions Normalize(FeedOptions options)
            => new()
            {
                Title = string.IsNullOrWhiteSpace(options.Title) ? "Blazor Blog" : options.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(options.Description)
                    ? "Latest published posts from Blazor Blog."
                    : options.Description.Trim(),
                ItemCount = Math.Clamp(options.ItemCount, 1, 100),
                CacheMinutes = Math.Clamp(options.CacheMinutes, 1, 1440)
            };

        private static Uri GetBaseUri(HttpRequest request)
        {
            var pathBase = request.PathBase.HasValue ? request.PathBase.Value : string.Empty;
            return new Uri($"{request.Scheme}://{request.Host}{pathBase}/");
        }
    }
}
