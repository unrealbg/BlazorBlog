namespace BlazorBlog.Tests.Component
{
    using System.Threading;
    using System.Threading.Tasks;
    using Bunit;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;
    using BlazorBlog.Components.Pages;
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Application.UI;
    using BlazorBlog.Infrastructure;
    using BlazorBlog.Infrastructure.Contracts;
    using FluentValidation;

    public class AllPostsPageTests
    {
        [Fact]
        public void AllPosts_Renders_NoPosts_State()
        {
            using var ctx = new TestContext();

            var blogMock = new Mock<IBlogPostService>(MockBehavior.Strict);
            blogMock.Setup(s => s.GetBlogPostsAsync(0, 8, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<BlogPostVm>());
            blogMock.Setup(s => s.GetPopularBlogPostsAsync(5, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<BlogPostVm>());

            var catMock = new Mock<ICategoryService>(MockBehavior.Strict);
            catMock.Setup(s => s.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<Category>());

            ctx.Services.AddSingleton(blogMock.Object);
            ctx.Services.AddSingleton(catMock.Object);
            ctx.Services.AddSingleton<IToastService, ToastService>();
            ctx.Services.AddSingleton<ISubscribeService, BlazorBlog.Tests.Component.DummySubscribeService>();
            ctx.Services.AddScoped<IValidator<BlazorBlog.Domain.Entities.Subscriber>, BlazorBlog.Tests.Component.DummySubscriberValidator>();

            var cut = ctx.RenderComponent<AllPosts>();
            cut.WaitForAssertion(() => Assert.Contains("All Posts", cut.Markup));
            Assert.Contains("No posts yet", cut.Markup);
        }

        [Fact]
        public async Task AllPosts_Changing_PageNumber_Loads_New_Page()
        {
            using var ctx = new TestContext();

            var blogMock = new Mock<IBlogPostService>(MockBehavior.Strict);
            // Initial load for page 1
            blogMock.Setup(s => s.GetBlogPostsAsync(0, 8, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<BlogPostVm>())
                .Verifiable();
            blogMock.Setup(s => s.GetPopularBlogPostsAsync(5, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<BlogPostVm>());
            var catMock = new Mock<ICategoryService>(MockBehavior.Strict);
            catMock.Setup(s => s.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<Category>());

            // When page changes to 2, pageIndex becomes 1
            blogMock.Setup(s => s.GetBlogPostsAsync(1, 8, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<BlogPostVm>())
                .Verifiable();

            ctx.Services.AddSingleton(blogMock.Object);
            ctx.Services.AddSingleton(catMock.Object);
            ctx.Services.AddSingleton<IToastService, ToastService>();
            ctx.Services.AddSingleton<ISubscribeService, BlazorBlog.Tests.Component.DummySubscribeService>();
            ctx.Services.AddScoped<IValidator<BlazorBlog.Domain.Entities.Subscriber>, BlazorBlog.Tests.Component.DummySubscriberValidator>();

            var cut = ctx.RenderComponent<AllPosts>(p => p.Add(x => x.PageNumber, 1));
            await cut.InvokeAsync(() => cut.SetParametersAndRender(parameters => parameters.Add(x => x.PageNumber, 2)));

            blogMock.VerifyAll();
        }
    }
}
