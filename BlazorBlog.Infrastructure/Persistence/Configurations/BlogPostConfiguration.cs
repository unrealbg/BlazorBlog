namespace BlazorBlog.Infrastructure.Persistence.Configurations
{
    using BlazorBlog.Infrastructure.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.HasIndex(b => b.Slug).IsUnique();
            builder.HasIndex(b => new { b.IsPublished, b.CategoryId });
            builder.HasIndex(b => new { b.IsPublished, b.PublishedAt });
            builder.HasIndex(b => b.ViewCount);

            // Use PostgreSQL xmin as concurrency token via shadow property
            builder.Property<uint>("xmin").IsRowVersion();
        }
    }
}
