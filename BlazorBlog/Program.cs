namespace BlazorBlog
{
    using System.Net;
    using System.Text.Json;

    using Components.Account;
    using BlazorBlog.Health;
    using BlazorBlog.Infrastructure;
    using BlazorBlog.Infrastructure.Settings;
    using Ganss.Xss;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using BlazorBlog.Application;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.StaticFiles;
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

            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"])
                .AddCheck<DiskSpaceHealthCheck>("disk", tags: ["ready"]);

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
            app.Use(ApplyStaticAssetCacheHeaders);
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ConfigureStaticFileCacheHeaders
            });
            app.UseAntiforgery();

            app.MapStaticAssets();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapAdditionalIdentityEndpoints();

            app.MapGet("/health", (ILogger<Program> logger) =>
                {
                    logger.LogDebug("Liveness health check completed.");
                    return Results.Ok(new { status = "ok", timeUtc = DateTime.UtcNow });
                })
                .WithName("Health")
                .WithTags("Health");

            app.MapHealthChecks("/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready", StringComparer.OrdinalIgnoreCase),
                ResponseWriter = WriteHealthCheckResponseAsync
            })
                .WithName("Ready")
                .WithTags("Health");

            app.Run();

            static Task WriteHealthCheckResponseAsync(HttpContext context, HealthReport report)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                var logLevel = report.Status == HealthStatus.Healthy ? LogLevel.Information : LogLevel.Warning;

                logger.Log(
                    logLevel,
                    "Readiness health check completed with status {Status} in {ElapsedMilliseconds} ms.",
                    report.Status,
                    report.TotalDuration.TotalMilliseconds);

                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = report.Status.ToString(),
                    timeUtc = DateTime.UtcNow,
                    totalDurationMilliseconds = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
                    checks = report.Entries.ToDictionary(
                        entry => entry.Key,
                        entry => new
                        {
                            status = entry.Value.Status.ToString(),
                            entry.Value.Description,
                            durationMilliseconds = Math.Round(entry.Value.Duration.TotalMilliseconds, 2),
                            entry.Value.Data
                        })
                };

                return context.Response.WriteAsJsonAsync(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }

            static async Task ApplyStaticAssetCacheHeaders(HttpContext context, RequestDelegate next)
            {
                var path = context.Request.Path.Value ?? string.Empty;

                if (IsLongLivedAsset(path))
                {
                    var environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

                    context.Response.OnStarting(() =>
                    {
                        if (!context.Response.Headers.ContainsKey("Cache-Control"))
                        {
                            context.Response.Headers.CacheControl = environment.IsDevelopment()
                                ? "no-cache"
                                : "public,max-age=31536000,immutable";
                        }

                        return Task.CompletedTask;
                    });
                }

                await next(context);
            }

            static void ConfigureStaticFileCacheHeaders(StaticFileResponseContext context)
            {
                var environment = context.Context.RequestServices.GetRequiredService<IWebHostEnvironment>();
                var path = context.Context.Request.Path.Value ?? string.Empty;

                context.Context.Response.Headers.CacheControl = environment.IsDevelopment()
                    ? "no-cache"
                    : IsLongLivedAsset(path)
                        ? "public,max-age=31536000,immutable"
                        : "public,max-age=3600";
            }

            static bool IsLongLivedAsset(string path)
            {
                var extension = Path.GetExtension(path);

                return extension.Equals(".css", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".js", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".svg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".webp", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".avif", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".ico", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".woff", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".woff2", StringComparison.OrdinalIgnoreCase);
            }

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
