namespace BlazorBlog.Infrastructure
{
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;
    using Microsoft.EntityFrameworkCore;

    public class SiteSettingsService : ISiteSettingsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SiteSettingsService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<SiteSettingsVm> GetSiteSettingsAsync(CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var settings = await context.SiteSettings
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return settings is null ? SiteSettingsVm.CreateDefault() : Map(settings);
        }

        public async Task<SiteSettingsVm> SaveSiteSettingsAsync(SiteSettingsVm settings, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(settings);

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await context.SiteSettings
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                entity = new Persistence.Entities.SiteSettings();
                await context.SiteSettings.AddAsync(entity, cancellationToken);
            }

            entity.HomeHeroTitle = settings.HomeHeroTitle.Trim();
            entity.HomeHeroSubtitle = settings.HomeHeroSubtitle.Trim();
            entity.HomeHeroTags = settings.HomeHeroTags.Trim();
            entity.HomeHeroPrimaryButtonText = settings.HomeHeroPrimaryButtonText.Trim();
            entity.HomeHeroPrimaryButtonUrl = settings.HomeHeroPrimaryButtonUrl.Trim();
            entity.HomeHeroSecondaryButtonText = settings.HomeHeroSecondaryButtonText.Trim();
            entity.HomeHeroSecondaryButtonUrl = settings.HomeHeroSecondaryButtonUrl.Trim();

            await context.SaveChangesAsync(cancellationToken);
            return Map(entity);
        }

        private static SiteSettingsVm Map(Persistence.Entities.SiteSettings settings) => new()
        {
            Id = settings.Id,
            HomeHeroTitle = settings.HomeHeroTitle,
            HomeHeroSubtitle = settings.HomeHeroSubtitle,
            HomeHeroTags = settings.HomeHeroTags,
            HomeHeroPrimaryButtonText = settings.HomeHeroPrimaryButtonText,
            HomeHeroPrimaryButtonUrl = settings.HomeHeroPrimaryButtonUrl,
            HomeHeroSecondaryButtonText = settings.HomeHeroSecondaryButtonText,
            HomeHeroSecondaryButtonUrl = settings.HomeHeroSecondaryButtonUrl
        };
    }
}
