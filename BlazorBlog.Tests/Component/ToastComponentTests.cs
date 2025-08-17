namespace BlazorBlog.Tests.Component
{
    using System.Threading.Tasks;
    using Bunit;
    using Xunit;
    using BlazorBlog.Components.Shared;
    using BlazorBlog.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    public class ToastComponentTests
    {
        [Fact]
        public async Task Toast_Shows_And_Hides_Message()
        {
            using var ctx = new TestContext();
            ctx.Services.AddSingleton<BlazorBlog.Application.UI.IToastService, ToastService>();

            var cut = ctx.RenderComponent<Toast>();

            var svc = ctx.Services.GetRequiredService<BlazorBlog.Application.UI.IToastService>();
            await cut.InvokeAsync(() => svc.ShowToast(BlazorBlog.Application.UI.ToastLevel.Success, "Hello", "Hi"));

            cut.Render();
            Assert.Contains("Hello", cut.Markup);
        }
    }
}
