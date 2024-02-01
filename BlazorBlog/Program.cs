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

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
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
                .AddScoped<ISubscriberRepository, SubscriberRepository>();

            builder.Services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>(provider =>
            {
                var sanitizer = new HtmlSanitizer();
                return sanitizer;
            });

            var app = builder.Build();

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            static async Task SeedDataAsync(IServiceProvider services)
            {
                var scope = services.CreateScope();
                var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
                await seedService.SeedDataAsync();
            }
        }
    }
}