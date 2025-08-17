namespace BlazorBlog.Infrastructure
{
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.UI;
    using BlazorBlog.Infrastructure.Contracts;
    using BlazorBlog.Infrastructure.Persistence.Repositories;
    using BlazorBlog.Infrastructure.Utilities;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Cross-cutting
            services.AddMemoryCache();
            services.AddSingleton<IBlogCacheSignal, BlogCacheSignal>();

            // App services
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<ISeedService, SeedService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBlogPostAdminService, BlogPostAdminService>();
            services.AddScoped<IBlogPostService, BlogPostService>();
            services.AddScoped<ISubscribeService, SubscribeService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ISlugService, SlugService>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options
                    .UseSqlServer(connectionString)
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
                options
                    .UseSqlServer(connectionString)
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            // Identity and stores
            services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddSingleton<IEmailSender<ApplicationUser>, NoOpEmailSender>();

            // Repositories
            services.AddScoped<IBlogPostAdminRepository, BlogPostRepository>();
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISubscriberRepository, SubscriberRepository>();
            services.AddScoped<ITagRepository, TagRepository>();

            return services;
        }
    }
}
