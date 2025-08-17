namespace BlazorBlog.Tests.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Infrastructure;
    using BlazorBlog.Infrastructure.Utilities;
    using Moq;
    using Xunit;

    public class BlogPostAdminServiceForwardersTests
    {
        [Fact]
        public async Task GetBlogPostsAsync_ForwardsToRepository()
        {
            var repo = new Mock<IBlogPostAdminRepository>(MockBehavior.Strict);
            var svc = new BlogPostAdminService(repo.Object, new SlugService(), new BlogCacheSignal());
            repo.Setup(r => r.GetBlogPostsAsync(0, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BlazorBlog.Application.Models.PageResult<BlogPost>(System.Array.Empty<BlogPost>(), 0));
            var result = await svc.GetBlogPostsAsync(0, 10);
            Assert.Equal(0, result.TotalCount);
            repo.VerifyAll();
        }

        [Fact]
        public async Task GetBlogPostByIdAsync_ForwardsToRepository()
        {
            var repo = new Mock<IBlogPostAdminRepository>(MockBehavior.Strict);
            var svc = new BlogPostAdminService(repo.Object, new SlugService(), new BlogCacheSignal());
            repo.Setup(r => r.GetBlogPostByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BlogPost?)null);
            var result = await svc.GetBlogPostByIdAsync(5);
            Assert.Null(result);
            repo.VerifyAll();
        }

        [Fact]
        public async Task DeleteBlogPostAsync_Forwards_And_Bumps_Signal_When_Deleted()
        {
            var repo = new Mock<IBlogPostAdminRepository>(MockBehavior.Strict);
            var signal = new BlogCacheSignal();
            var svc = new BlogPostAdminService(repo.Object, new SlugService(), signal);
            var before = signal.Version;
            repo.Setup(r => r.DeleteBlogPostAsync(7, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var ok = await svc.DeleteBlogPostAsync(7);
            Assert.True(ok);
            Assert.True(signal.Version > before);
            repo.VerifyAll();
        }
    }
}
