namespace BlazorBlog.Tests.Component
{
    using Bunit;
    using Xunit;
    using BlazorBlog.Components.Pages;
    using BlazorBlog.Tests.Fakes;

    using Microsoft.Extensions.DependencyInjection;

    public class HomeComponentTests
    {
        [Fact]
        public void Home_Render_DoesNotThrow()
        {
            using var ctx = new TestContext();
            // Minimal DI to render; the component renders before OnInitializedAsync completes
            ctx.Services.AddSingleton<BlazorBlog.Application.Contracts.IBlogPostService>(new FakeBlogPostService());

            var cut = ctx.RenderComponent<Home>();
            Assert.NotNull(cut.Instance);
        }
    }
}
