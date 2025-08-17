namespace BlazorBlog.Tests.Services
{
    using System;
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
            repo.Setup(r => r.GetCategoryBySlugAsync("c-net", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);
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

        [Fact]
        public async Task SaveCategoryAsync_EmptyName_Throws()
        {
            var repo = new Mock<ICategoryRepository>(MockBehavior.Strict);
            var slug = new SlugService();
            var svc = new CategoryService(repo.Object, slug);
            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.SaveCategoryAsync(new Category { Name = "" }));
        }

        [Fact]
        public async Task SaveCategoryAsync_EmptySlugFromName_FallsBack_To_Default()
        {
            var repo = new Mock<ICategoryRepository>(MockBehavior.Strict);
            var slug = new SlugService();
            var svc = new CategoryService(repo.Object, slug);

            // Name that becomes empty after slugging (e.g., only symbols/spaces)
            var category = new Category { Id = 0, Name = "   ***   " };
            repo.Setup(r => r.GetCategoryBySlugAsync("category", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);
            repo.Setup(r => r.SaveCategoryAsync(It.Is<Category>(c => c.Slug == "category"), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category c, CancellationToken _) => c);

            var saved = await svc.SaveCategoryAsync(category);
            Assert.Equal("category", saved.Slug);
            repo.VerifyAll();
        }
    }
}
