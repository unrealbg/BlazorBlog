namespace BlazorBlog.Infrastructure
{
    using BlazorBlog.Application.Models;
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Infrastructure.Utilities;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using System.Threading;

    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IMemoryCache _cache;
        private readonly IBlogCacheSignal _signal;
        private readonly ILogger<BlogPostService> _logger;

        public BlogPostService(IBlogPostRepository blogPostRepository, IMemoryCache cache, IBlogCacheSignal signal, ILogger<BlogPostService> logger)
        {
            _blogPostRepository = blogPostRepository;
            _cache = cache;
            _signal = signal;
            _logger = logger;
        }

        private async Task<T> CacheGetOrCreateAsync<T>(string key, Func<Task<T>> factory)
        {
            if (_cache.TryGetValue(key, out T? cached) && cached is not null)
            {
                return cached;
            }

            var value = await factory();
            _cache.Set(key, value, TimeSpan.FromMinutes(2));
            return value;
        }

        public Task<BlogPostVm[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
            => CacheGetOrCreateAsync($"featured:{categoryId}:{count}:{_signal.Version}", () => _blogPostRepository.GetFeaturedBlogPostsAsync(count, categoryId, cancellationToken));

        public Task<BlogPostVm[]> GetPopularBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
            => CacheGetOrCreateAsync($"popular:{categoryId}:{count}:{_signal.Version}", () => _blogPostRepository.GetPopularBlogPostsAsync(count, categoryId, cancellationToken));

        public Task<BlogPostVm[]> GetRecentBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
            => CacheGetOrCreateAsync($"recent:{categoryId}:{count}:{_signal.Version}", () => _blogPostRepository.GetRecentBlogPostsAsync(count, categoryId, cancellationToken));

        public Task<BlogPostVm[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId, CancellationToken cancellationToken = default)
            => _blogPostRepository.GetBlogPostsAsync(pageIndex, pageSize, categoryId, cancellationToken);

        public Task<DetailPageModel> GetBlogPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
            => _blogPostRepository.GetBlogPostBySlugAsync(slug, cancellationToken);
    }
}
