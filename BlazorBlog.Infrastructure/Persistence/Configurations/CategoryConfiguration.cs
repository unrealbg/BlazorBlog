namespace BlazorBlog.Infrastructure.Persistence.Configurations
{
    using BlazorBlog.Infrastructure.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasIndex(c => c.Slug).IsUnique();
        }
    }
}
