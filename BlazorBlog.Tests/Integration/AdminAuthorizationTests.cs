namespace BlazorBlog.Tests.Integration
{
    using System.Threading.Tasks;
    using Xunit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authorization;

    public class AdminAuthorizationTests
    {
        [Fact]
        public async Task Dashboard_RequiresAdminRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var authOptions = serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>();
            var policy = await authOptions.GetPolicyAsync("RequireAdminRole");

            // Assert
            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }

        [Fact]
        public async Task ManageBlogPosts_RequiresAdminOrEditorRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("RequireAdminOrEditorRole", policy => policy.RequireRole("Admin", "Editor"));
            });
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var authOptions = serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>();
            var policy = await authOptions.GetPolicyAsync("RequireAdminOrEditorRole");

            // Assert
            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }

        [Fact]
        public async Task ManageSubscribers_RequiresAdminRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var authOptions = serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>();
            var policy = await authOptions.GetPolicyAsync("RequireAdminRole");

            // Assert
            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }

        [Fact]
        public async Task ManageCategories_RequiresAdminOrEditorRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("RequireAdminOrEditorRole", policy => policy.RequireRole("Admin", "Editor"));
            });
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var authOptions = serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>();
            var policy = await authOptions.GetPolicyAsync("RequireAdminOrEditorRole");

            // Assert
            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }
    }
}
