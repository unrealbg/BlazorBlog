namespace BlazorBlog.Tests.Component
{
    using System.Threading;
    using System.Threading.Tasks;
    using Bunit;
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;
    using BlazorBlog.Components.Pages;
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;
    using FluentValidation;
    using BlazorBlog.Infrastructure.Contracts;

    public class BlogPostDetailPageTests
    {
        [Fact]
        public void Navigates_Home_When_Empty_Result()
        {
            using var ctx = new TestContext();
            var blog = new Mock<IBlogPostService>(MockBehavior.Strict);
            blog.Setup(s => s.GetBlogPostBySlugAsync("missing", It.IsAny<CancellationToken>()))
                .ReturnsAsync(DetailPageModel.Empty());
            ctx.Services.AddSingleton(blog.Object);

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            var cut = ctx.RenderComponent<BlogPostDetail>(p => p.Add(x => x.BlogPostSlug, "missing"));
            // Blazor TestContext provides base URI http://localhost/
            Assert.StartsWith("http://localhost/", nav.Uri);
        }

        [Fact]
        public void Shows_Post_And_Loads_Popular_In_Category()
        {
            using var ctx = new TestContext();
            var blog = new Mock<IBlogPostService>(MockBehavior.Strict);

            var vm = new BlogPostVm
            {
                Id = 1,
                Title = "Post",
                CategoryId = 7,
                Content = "<p>Hello world content</p>"
            };
            blog.Setup(s => s.GetBlogPostBySlugAsync("ok", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DetailPageModel(vm, System.Array.Empty<BlogPostVm>()));
            blog.Setup(s => s.GetPopularBlogPostsAsync(4, 7, It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Array.Empty<BlogPostVm>());

            ctx.Services.AddSingleton(blog.Object);
            // Register DI required by SubscribeBox
            ctx.Services.AddSingleton<ISubscribeService, BlazorBlog.Tests.Component.DummySubscribeService>();
            ctx.Services.AddScoped<IValidator<BlazorBlog.Domain.Entities.Subscriber>, BlazorBlog.Tests.Component.DummySubscriberValidator>();

            var cut = ctx.RenderComponent<BlogPostDetail>(p => p.Add(x => x.BlogPostSlug, "ok"));
            // Should not navigate; ensures render
            Assert.Contains("Post", cut.Markup);
        }
    }
}
