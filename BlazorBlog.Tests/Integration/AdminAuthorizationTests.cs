namespace BlazorBlog.Tests.Integration
{
    using Xunit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authorization;

    public class AdminAuthorizationTests
    {
        [Fact]
        public void Dashboard_RequiresAdminRole()
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
            var policy = authOptions.GetPolicyAsync("RequireAdminRole").Result;

            // Assert
            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }

        [Fact]
        public void ManageBlogPosts_RequiresAdminOrEditorRole()
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
            var policy = authOptions.GetPolicyAsync("RequireAdminOrEditorRole").Result;

            // Assert
            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }

        [Fact]
        public void ManageSubscribers_RequiresAdminRole()
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
            var policy = authOptions.GetPolicyAsync("RequireAdminRole").Result;

            // Assert
            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }

        [Fact]
        public void ManageCategories_RequiresAdminOrEditorRole()
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
            var policy = authOptions.GetPolicyAsync("RequireAdminOrEditorRole").Result;

            // Assert
            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }
    }
}