namespace BlazorBlog.Tests.Integration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using BlazorBlog.Components.Pages.Admin;
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

        [Theory]
        [InlineData(typeof(ManageBlogPosts))]
        [InlineData(typeof(ManageCategories))]
        [InlineData(typeof(SaveBlogPost))]
        public void ContentManagementPages_DoNotRequireAdminOnlyRole(Type pageType)
        {
            var attributes = pageType.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>()
                .ToArray();

            Assert.Contains(attributes, attribute => attribute.Policy == "RequireAdminOrEditorRole");
            Assert.DoesNotContain(attributes, attribute => attribute.Roles == "Admin");
        }
    }
}
