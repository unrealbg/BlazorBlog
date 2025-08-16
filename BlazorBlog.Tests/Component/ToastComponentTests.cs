namespace BlazorBlog.Tests.Component
{
    using Bunit;
    using Xunit;
    using BlazorBlog.Components.Shared;
    using BlazorBlog.Infrastructure;
    using BlazorBlog.Application.UI;
    using Microsoft.Extensions.DependencyInjection;

    public class ToastComponentTests
    {
        [Fact]
        public void Toast_Shows_And_Hides_Message()
        {
            using var ctx = new TestContext();
            ctx.Services.AddSingleton<BlazorBlog.Application.UI.IToastService, ToastService>();

            var cut = ctx.RenderComponent<Toast>();

            var svc = ctx.Services.GetRequiredService<BlazorBlog.Application.UI.IToastService>();
            svc.ShowToast(ToastLevel.Success, "Hello", "Hi");
            cut.Render();
            Assert.Contains("Hello", cut.Markup);
        }
    }
}
