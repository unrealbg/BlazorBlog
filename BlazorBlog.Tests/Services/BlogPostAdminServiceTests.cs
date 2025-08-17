namespace BlazorBlog.Tests.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Infrastructure;
    using BlazorBlog.Infrastructure.Contracts;
    using BlazorBlog.Infrastructure.Utilities;
    using Moq;
    using Xunit;

    public class BlogPostAdminServiceTests
    {
        private static BlogPostAdminService CreateService(out Mock<IBlogPostAdminRepository> repo, out ISlugService slug, out IBlogCacheSignal signal)
        {
            repo = new Mock<IBlogPostAdminRepository>(MockBehavior.Strict);
            slug = new SlugService();
            signal = new BlogCacheSignal();
            return new BlogPostAdminService(repo.Object, slug, signal);
        }

        [Fact]
        public async Task SaveBlogPostAsync_GeneratesUniqueSlug_AndBumpsSignal()
        {
            var svc = CreateService(out var repo, out var slug, out var signal);
            var post = new BlogPost { Id = 0, Title = "Hello World!", IsPublished = true };

            repo.Setup(r => r.TitleExistsAsync("Hello World!", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            // First attempt collides, second attempt unique
            repo.Setup(r => r.SlugExistsAsync("hello-world", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            repo.Setup(r => r.SlugExistsAsync("hello-world-1", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            repo.Setup(r => r.SaveBlogPostAsync(It.IsAny<BlogPost>(), "u1", It.IsAny<CancellationToken>()))
                .ReturnsAsync((BlogPost b, string _, CancellationToken __) => b);

            var before = signal.Version;
            var saved = await svc.SaveBlogPostAsync(post, "u1");
            var after = signal.Version;

            Assert.Equal("hello-world-1", saved.Slug);
            Assert.True(saved.IsPublished);
            Assert.NotNull(saved.PublishedAt);
            Assert.True(after > before);

            repo.VerifyAll();
        }

        [Fact]
        public async Task SaveBlogPostAsync_Throws_WhenTitleExists()
        {
            var svc = CreateService(out var repo, out var _, out var _signal);
            var post = new BlogPost { Id = 0, Title = "Duplicate" };
            repo.Setup(r => r.TitleExistsAsync("Duplicate", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.SaveBlogPostAsync(post, "u1"));
        }

        [Fact]
        public async Task DeleteBlogPostAsync_BumpsSignalOnSuccess()
        {
            var svc = CreateService(out var repo, out var _, out var signal);
            repo.Setup(r => r.DeleteBlogPostAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var before = signal.Version;
            var ok = await svc.DeleteBlogPostAsync(5);
            var after = signal.Version;
            Assert.True(ok);
            Assert.True(after > before);
        }
    }
}
