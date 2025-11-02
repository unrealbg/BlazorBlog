namespace BlazorBlog.Tests.Authorization
{


    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    public class AuthorizationPolicyTests
    {
        [Fact]
        public async Task AdminPolicy_AllowsAdminRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationBuilder()
                 .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
               {
            new Claim(ClaimTypes.Name, "TestUser"),
  new Claim(ClaimTypes.Role, "Admin")
      }, "TestAuthentication"));

            // Act
            var result = await authService.AuthorizeAsync(user, "RequireAdminRole");

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task AdminPolicy_DeniesEditorRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Editor")
        }, "TestAuthentication"));

            // Act
            var result = await authService.AuthorizeAsync(user, "RequireAdminRole");

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task EditorPolicy_AllowsEditorRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationBuilder()
    .AddPolicy("RequireEditorRole", policy => policy.RequireRole("Editor"));
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
              {
        new Claim(ClaimTypes.Name, "TestUser"),
  new Claim(ClaimTypes.Role, "Editor")
      }, "TestAuthentication"));

            // Act
            var result = await authService.AuthorizeAsync(user, "RequireEditorRole");

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task AdminOrEditorPolicy_AllowsAdminRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminOrEditorRole", policy => policy.RequireRole("Admin", "Editor"));
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "TestAuthentication"));

            // Act
            var result = await authService.AuthorizeAsync(user, "RequireAdminOrEditorRole");

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task AdminOrEditorPolicy_AllowsEditorRole()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminOrEditorRole", policy => policy.RequireRole("Admin", "Editor"));
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
          {
   new Claim(ClaimTypes.Name, "TestUser"),
    new Claim(ClaimTypes.Role, "Editor")
        }, "TestAuthentication"));

            // Act
            var result = await authService.AuthorizeAsync(user, "RequireAdminOrEditorRole");

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task AdminOrEditorPolicy_DeniesRegularUser()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationBuilder()
             .AddPolicy("RequireAdminOrEditorRole", policy => policy.RequireRole("Admin", "Editor"));
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
          {
       new Claim(ClaimTypes.Name, "TestUser"),
     new Claim(ClaimTypes.Role, "User")
        }, "TestAuthentication"));

            // Act
            var result = await authService.AuthorizeAsync(user, "RequireAdminOrEditorRole");

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task AllPolicies_DenyUnauthenticatedUser()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorizationBuilder()
         .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
          .AddPolicy("RequireEditorRole", policy => policy.RequireRole("Editor"))
       .AddPolicy("RequireAdminOrEditorRole", policy => policy.RequireRole("Admin", "Editor"));
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated

            // Act & Assert
            var adminResult = await authService.AuthorizeAsync(user, "RequireAdminRole");
            Assert.False(adminResult.Succeeded);

            var editorResult = await authService.AuthorizeAsync(user, "RequireEditorRole");
            Assert.False(editorResult.Succeeded);

            var adminOrEditorResult = await authService.AuthorizeAsync(user, "RequireAdminOrEditorRole");
            Assert.False(adminOrEditorResult.Succeeded);
        }
    }
}