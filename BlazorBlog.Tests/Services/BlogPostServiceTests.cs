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

        [Fact]
        public async Task GetFeaturedBlogPostsAsync_UsesCache_AndSignalInvalidates()
        {
            var posts = new[] { new BlogPostVm { Id = 2, Title = "F" } };
            var svc = CreateService(out var repo, out var signal);

            repo.Setup(r => r.GetFeaturedBlogPostsAsync(5, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts)
                .Verifiable();

            var first = await svc.GetFeaturedBlogPostsAsync(5);
            Assert.Single(first);

            var second = await svc.GetFeaturedBlogPostsAsync(5);
            Assert.Single(second);
            repo.Verify(r => r.GetFeaturedBlogPostsAsync(5, 0, It.IsAny<CancellationToken>()), Times.Once);

            signal.Bump();
            repo.Setup(r => r.GetFeaturedBlogPostsAsync(5, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts);
            var third = await svc.GetFeaturedBlogPostsAsync(5);
            Assert.Single(third);
            repo.Verify(r => r.GetFeaturedBlogPostsAsync(5, 0, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetPopularBlogPostsAsync_UsesCache_AndSignalInvalidates()
        {
            var posts = new[] { new BlogPostVm { Id = 3, Title = "P" } };
            var svc = CreateService(out var repo, out var signal);

            repo.Setup(r => r.GetPopularBlogPostsAsync(4, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts)
                .Verifiable();

            var first = await svc.GetPopularBlogPostsAsync(4);
            Assert.Single(first);

            var second = await svc.GetPopularBlogPostsAsync(4);
            Assert.Single(second);
            repo.Verify(r => r.GetPopularBlogPostsAsync(4, 0, It.IsAny<CancellationToken>()), Times.Once);

            signal.Bump();
            repo.Setup(r => r.GetPopularBlogPostsAsync(4, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts);
            var third = await svc.GetPopularBlogPostsAsync(4);
            Assert.Single(third);
            repo.Verify(r => r.GetPopularBlogPostsAsync(4, 0, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetRecentBlogPostsByTagAsync_UsesCache_AndSignalInvalidates()
        {
            var posts = new[] { new BlogPostVm { Id = 10, Title = "T" } };
            var svc = CreateService(out var repo, out var signal);

            repo.Setup(r => r.GetRecentBlogPostsByTagAsync("tag", 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts)
                .Verifiable();

            var first = await svc.GetRecentBlogPostsByTagAsync("tag", 2);
            Assert.Single(first);

            var second = await svc.GetRecentBlogPostsByTagAsync("tag", 2);
            Assert.Single(second);
            repo.Verify(r => r.GetRecentBlogPostsByTagAsync("tag", 2, It.IsAny<CancellationToken>()), Times.Once);

            signal.Bump();
            repo.Setup(r => r.GetRecentBlogPostsByTagAsync("tag", 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts);
            var third = await svc.GetRecentBlogPostsByTagAsync("tag", 2);
            Assert.Single(third);
            repo.Verify(r => r.GetRecentBlogPostsByTagAsync("tag", 2, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetPopularBlogPostsByTagAsync_UsesCache_AndSignalInvalidates()
        {
            var posts = new[] { new BlogPostVm { Id = 11, Title = "TP" } };
            var svc = CreateService(out var repo, out var signal);

            repo.Setup(r => r.GetPopularBlogPostsByTagAsync("tag", 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts)
                .Verifiable();

            var first = await svc.GetPopularBlogPostsByTagAsync("tag", 2);
            Assert.Single(first);

            var second = await svc.GetPopularBlogPostsByTagAsync("tag", 2);
            Assert.Single(second);
            repo.Verify(r => r.GetPopularBlogPostsByTagAsync("tag", 2, It.IsAny<CancellationToken>()), Times.Once);

            signal.Bump();
            repo.Setup(r => r.GetPopularBlogPostsByTagAsync("tag", 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(posts);
            var third = await svc.GetPopularBlogPostsByTagAsync("tag", 2);
            Assert.Single(third);
            repo.Verify(r => r.GetPopularBlogPostsByTagAsync("tag", 2, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetBlogPostsAsync_ForwardsToRepository()
        {
            var svc = CreateService(out var repo, out _);
            repo.Setup(r => r.GetBlogPostsAsync(1, 10, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<BlogPostVm>());
            var result = await svc.GetBlogPostsAsync(1, 10, 0);
            Assert.Empty(result);
            repo.VerifyAll();
        }

        [Fact]
        public async Task GetBlogPostsByTagAsync_ForwardsToRepository()
        {
            var svc = CreateService(out var repo, out _);
            repo.Setup(r => r.GetBlogPostsByTagAsync("tag", 0, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<BlogPostVm>());
            var result = await svc.GetBlogPostsByTagAsync("tag", 0, 10);
            Assert.Empty(result);
            repo.VerifyAll();
        }
    }
}
