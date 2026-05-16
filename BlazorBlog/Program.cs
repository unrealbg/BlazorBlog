namespace BlazorBlog
{
    using System.Net;

    using Components.Account;
    using BlazorBlog.Infrastructure;
    using BlazorBlog.Infrastructure.Settings;
    using Ganss.Xss;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using BlazorBlog.Application;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using BlazorBlog.Infrastructure.Persistence;

    using Microsoft.AspNetCore.HttpOverrides;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Serilog: read configuration and set as host logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            builder.Host.UseSerilog(Log.Logger, true);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents(options => options.DetailedErrors = builder.Environment.IsDevelopment());

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            // Add authorization policies for role-based access control
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
                .AddPolicy("RequireEditorRole", policy => policy.RequireRole("Editor"))
                .AddPolicy("RequireAdminOrEditorRole", policy => policy.RequireRole("Admin", "Editor"));

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddApplication();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddInfrastructureServices();

            builder.Services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>(_ => new HtmlSanitizer());

            ValidateStartupConfiguration(builder);

            var app = builder.Build();

            ConfigureForwardedHeaders(app);

            if (ShouldApplyMigrations(app))
            {
                await ApplyMigrationsAsync(app.Services);
            }

            if (app.Configuration.GetValue("Database:SeedOnStartup", true))
            {
                await SeedDataAsync(app.Services);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapAdditionalIdentityEndpoints();

            // Lightweight health endpoint
            app.MapGet("/health", () => Results.Ok(new { status = "ok", timeUtc = DateTime.UtcNow }))
                .WithName("Health");

            app.Run();

            static async Task ApplyMigrationsAsync(IServiceProvider services)
            {
                await using var scope = services.CreateAsyncScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("Applying database migrations...");
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error applying database migrations.");
                    throw;
                }
            }

            static async Task SeedDataAsync(IServiceProvider services)
            {
                using var scope = services.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("Seeding initial data (if required)...");
                    var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
                    await seedService.SeedDataAsync();
                    logger.LogInformation("Seeding completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while seeding data.");
                    throw;
                }
            }

            static bool ShouldApplyMigrations(WebApplication app)
            {
                return app.Configuration.GetValue<bool?>("Database:ApplyMigrationsOnStartup")
                    ?? app.Environment.IsDevelopment();
            }

            static void ConfigureForwardedHeaders(WebApplication app)
            {
                if (app.Environment.IsDevelopment())
                {
                    return;
                }

                var knownProxies = app.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? [];
                if (knownProxies.Length == 0)
                {
                    app.Logger.LogInformation("Forwarded headers are disabled because no known proxies are configured.");
                    return;
                }

                var options = new ForwardedHeadersOptions
                {
                    ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor |
                        ForwardedHeaders.XForwardedProto |
                        ForwardedHeaders.XForwardedHost,
                    ForwardLimit = 1
                };

                foreach (var proxy in knownProxies)
                {
                    if (IPAddress.TryParse(proxy, out var address))
                    {
                        options.KnownProxies.Add(address);
                    }
                    else
                    {
                        app.Logger.LogWarning("Ignoring invalid forwarded header proxy address '{Proxy}'.", proxy);
                    }
                }

                if (options.KnownProxies.Count > 0)
                {
                    app.UseForwardedHeaders(options);
                }
            }

            static void ValidateStartupConfiguration(WebApplicationBuilder builder)
            {
                if (builder.Environment.IsDevelopment())
                {
                    return;
                }

                var adminSettings = builder.Configuration.GetSection("AdminUser").Get<AdminUserSettings>() ?? new AdminUserSettings();
                if (string.IsNullOrWhiteSpace(adminSettings.Password) ||
                    string.Equals(adminSettings.Password, "Admin@123", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("Production requires a non-default AdminUser:Password value.");
                }

                var emailSettings = builder.Configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>() ?? new EmailSettings();
                if (emailSettings.RequireConfiguredSender && !emailSettings.IsConfigured)
                {
                    throw new InvalidOperationException("Email:RequireConfiguredSender is true, but SMTP Email settings are incomplete.");
                }
            }
        }
    }
}
