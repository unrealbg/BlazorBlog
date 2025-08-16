namespace BlazorBlog.Infrastructure.Persistence.Configurations
{
    using BlazorBlog.Infrastructure.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SubscriberConfiguration : IEntityTypeConfiguration<Subscriber>
    {
        public void Configure(EntityTypeBuilder<Subscriber> builder)
        {
            builder.HasIndex(s => s.Email).IsUnique();
        }
    }
}
