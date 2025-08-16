namespace BlazorBlog.Tests.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Infrastructure;
    using BlazorBlog.Infrastructure.Contracts;
    using Moq;
    using Xunit;

    public class CategoryServiceTests
    {
        [Fact]
        public async Task SaveCategoryAsync_GeneratesSlug_ThenPersists()
        {
            var repo = new Mock<ICategoryRepository>(MockBehavior.Strict);
            var slug = new SlugService();
            var svc = new CategoryService(repo.Object, slug);

            var category = new Category { Name = "C# & .NET" };
            repo.Setup(r => r.SaveCategoryAsync(It.Is<Category>(c => c.Slug == "c-net"), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category c, CancellationToken _) => c);

            var saved = await svc.SaveCategoryAsync(category);
            Assert.Equal("c-net", saved.Slug);
            repo.VerifyAll();
        }

        [Fact]
        public async Task GetCategoriesAsync_ForwardsToRepo()
        {
            var repo = new Mock<ICategoryRepository>(MockBehavior.Strict);
            var slug = new SlugService();
            var svc = new CategoryService(repo.Object, slug);

            repo.Setup(r => r.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await svc.GetCategoriesAsync();
            Assert.Empty(result);
            repo.VerifyAll();
        }
    }
}
