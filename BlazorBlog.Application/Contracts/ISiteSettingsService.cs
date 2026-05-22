namespace BlazorBlog.Application.Contracts
{
    using BlazorBlog.Application.Models;

    public interface ISiteSettingsService
    {
        Task<SiteSettingsVm> GetSiteSettingsAsync(CancellationToken cancellationToken = default);

        Task<SiteSettingsVm> SaveSiteSettingsAsync(SiteSettingsVm settings, CancellationToken cancellationToken = default);
    }
}
