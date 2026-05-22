namespace BlazorBlog.Infrastructure.Persistence.Configurations
{
    using BlazorBlog.Infrastructure.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SiteSettingsConfiguration : IEntityTypeConfiguration<SiteSettings>
    {
        public void Configure(EntityTypeBuilder<SiteSettings> builder)
        {
            builder.Property(x => x.HomeHeroTitle).HasMaxLength(180).IsRequired();
            builder.Property(x => x.HomeHeroSubtitle).HasMaxLength(500).IsRequired();
            builder.Property(x => x.HomeHeroTags).HasMaxLength(180).IsRequired();
            builder.Property(x => x.HomeHeroPrimaryButtonText).HasMaxLength(60).IsRequired();
            builder.Property(x => x.HomeHeroPrimaryButtonUrl).HasMaxLength(300).IsRequired();
            builder.Property(x => x.HomeHeroSecondaryButtonText).HasMaxLength(60).IsRequired();
            builder.Property(x => x.HomeHeroSecondaryButtonUrl).HasMaxLength(300).IsRequired();
        }
    }
}
