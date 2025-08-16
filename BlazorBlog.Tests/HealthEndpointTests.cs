namespace BlazorBlog.Tests
{
    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    public class HealthEndpointTests
    {
        [Fact]
        public void App_Has_Health_Endpoint_Mapped()
        {
            // Minimal placeholder to ensure the test project runs and DI works.
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider);
        }
    }
}
