namespace BlazorBlog.Tests.Fakes
{
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeSiteSettingsService : ISiteSettingsService
    {
        public Task<SiteSettingsVm> GetSiteSettingsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(SiteSettingsVm.CreateDefault());

        public Task<SiteSettingsVm> SaveSiteSettingsAsync(SiteSettingsVm settings, CancellationToken cancellationToken = default)
            => Task.FromResult(settings);
    }
}
