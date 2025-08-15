namespace BlazorBlog
{
    using Services.BlazorBlog.Services;
    using Components.Account;
    using Data;
    using Services;
    using Ganss.Xss;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Repository.Contracts;
    using Repository;
    using Microsoft.Extensions.Logging;
    using Serilog;

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

            // Add services to the container.
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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            builder.Services.AddScoped<IToastService, ToastService>();

            builder.Services
                .AddScoped<ISeedService, SeedService>()
                .AddScoped<ICategoryService, CategoryService>()
                .AddScoped<IBlogPostAdminService, BlogPostAdminService>()
                .AddScoped<IBlogPostService, BlogPostService>()
                .AddScoped<ISubscribeService, SubscribeService>()
                .AddScoped<IBlogPostRepository, BlogPostRepository>()
                .AddScoped<ICategoryRepository, CategoryRepository>()
                .AddScoped<ISubscriberRepository, SubscriberRepository>()
                .AddScoped<ISlugService, SlugService>();

            builder.Services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>(provider =>
            {
                var sanitizer = new HtmlSanitizer();
                return sanitizer;
            });

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

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();

            static async Task ApplyMigrationsAsync(IServiceProvider services)
            {
                await using var scope = services.CreateAsyncScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("Applying database migrations...");
                    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                    await using var db = await factory.CreateDbContextAsync();
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