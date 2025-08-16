namespace BlazorBlog.Infrastructure.Persistence
{
    using BlazorBlog.Infrastructure.Persistence.Configurations;
    using BlazorBlog.Infrastructure.Persistence.Entities;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new BlogPostConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new SubscriberConfiguration());
        }
    }
}
