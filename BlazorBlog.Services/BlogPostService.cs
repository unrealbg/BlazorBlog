namespace BlazorBlog.Services
{
    using BlazorBlog.Data.Entities;
    using BlazorBlog.Data.Models;
    using BlazorBlog.Services.Contracts;
    using BlazorBlog.Services.Utilities;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using BlazorBlog.Repository.Contracts;
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

        public async Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
        {
            var key = ($"featured:{categoryId}:{count}:{_signal.Version}");
            if (_cache.TryGetValue(key, out BlogPost[]? cached) && cached is not null)
            {
                return cached;
            }

            var data = await _blogPostRepository.GetFeaturedBlogPostsAsync(count, categoryId, cancellationToken);
            _cache.Set(key, data, TimeSpan.FromMinutes(2));
            return data;
        }

        public async Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
        {
            var key = ($"popular:{categoryId}:{count}:{_signal.Version}");
            if (_cache.TryGetValue(key, out BlogPost[]? cached) && cached is not null)
            {
                return cached;
            }
            var data = await _blogPostRepository.GetPopularBlogPostsAsync(count, categoryId, cancellationToken);
            _cache.Set(key, data, TimeSpan.FromMinutes(2));
            return data;
        }

        public async Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
        {
            var key = ($"recent:{categoryId}:{count}:{_signal.Version}");
            if (_cache.TryGetValue(key, out BlogPost[]? cached) && cached is not null)
            {
                return cached;
            }
            var data = await _blogPostRepository.GetRecentBlogPostsAsync(count, categoryId, cancellationToken);
            _cache.Set(key, data, TimeSpan.FromMinutes(2));
            return data;
        }

        public Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId, CancellationToken cancellationToken = default)
            => _blogPostRepository.GetBlogPostsAsync(pageIndex, pageSize, categoryId, cancellationToken);

        public Task<DetailPageModel> GetBlogPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
            => _blogPostRepository.GetBlogPostBySlugAsync(slug, cancellationToken);
    }
}
