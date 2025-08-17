namespace BlazorBlog.Tests.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;
    using BlazorBlog.Infrastructure;
    using BlazorBlog.Infrastructure.Utilities;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    public class BlogPostServiceTests
    {
        private static BlogPostService CreateService(out Mock<IBlogPostRepository> repoMock, out IBlogCacheSignal signal, IMemoryCache? cache = null)
        {
            repoMock = new Mock<IBlogPostRepository>(MockBehavior.Strict);
            signal = new BlogCacheSignal();
            cache ??= new MemoryCache(new MemoryCacheOptions());
            var svc = new BlogPostService(repoMock.Object, cache, signal, NullLogger<BlogPostService>.Instance);
            return svc;
        }

        [Fact]
        public async Task GetRecentBlogPostsAsync_UsesCache_WithSignalVersion()
        {
            var posts = new[] { new BlogPostVm { Id = 1, Title = "A" } };
            var svc = CreateService(out var repo, out var signal);

            repo.Setup(r => r.GetRecentBlogPostsAsync(3, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts)
                .Verifiable();

            var first = await svc.GetRecentBlogPostsAsync(3);
            Assert.Single(first);

            // Second call returns from cache; repository should not be hit again
            var second = await svc.GetRecentBlogPostsAsync(3);
            Assert.Single(second);
            repo.Verify(r => r.GetRecentBlogPostsAsync(3, 0, It.IsAny<CancellationToken>()), Times.Once);

            // Bump signal -> cache key changes -> repo called again
            signal.Bump();
            repo.Setup(r => r.GetRecentBlogPostsAsync(3, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts);
            var third = await svc.GetRecentBlogPostsAsync(3);
            Assert.Single(third);
            repo.Verify(r => r.GetRecentBlogPostsAsync(3, 0, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetBySlug_ForwardsToRepository()
        {
            var expected = new DetailPageModel(new BlogPostVm { Id = 123 }, Array.Empty<BlogPostVm>());
            var svc = CreateService(out var repo, out _);
            repo.Setup(r => r.GetBlogPostBySlugAsync("slug", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await svc.GetBlogPostBySlugAsync("slug");
            Assert.Equal(123, result.BlogPost.Id);
            repo.VerifyAll();
        }
    }
}
