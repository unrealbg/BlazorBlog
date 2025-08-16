namespace BlazorBlog
{
    using Components.Account;
    using BlazorBlog.Infrastructure;
    using Ganss.Xss;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using BlazorBlog.Application;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using BlazorBlog.Infrastructure.Persistence;


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
                .AddInteractiveServerComponents(options => options.DetailedErrors = true);

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

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddApplication();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddInfrastructureServices();

            builder.Services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>(_ => new HtmlSanitizer());

            var app = builder.Build();

            // Apply pending EF Core migrations automatically at startup.
            await ApplyMigrationsAsync(app.Services);

            // Seed the database.
            await SeedDataAsync(app.Services);

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
        }
    }
}