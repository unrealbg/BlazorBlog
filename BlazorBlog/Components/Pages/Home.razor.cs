namespace BlazorBlog.Components.Pages
{
    public partial class Home
    {
        private BlogPostVm[] _featured = [];
        private BlogPostVm[] _popular = [];
        private BlogPostVm[] _recent = [];
        private TagVm[] _tags = Array.Empty<BlazorBlog.Application.Models.TagVm>();
        private SiteSettingsVm _siteSettings = SiteSettingsVm.CreateDefault();

        private BlogPostVm _firstFeatured = default!;

        [Inject]
        IBlogPostService BlogPostService { get; set; } = default!;
        [Inject]
        BlazorBlog.Infrastructure.Contracts.ITagService TagService { get; set; } = default!;
        [Inject]
        ISiteSettingsService SiteSettingsService { get; set; } = default!;

        private readonly CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            var posts = await Task.WhenAll(
                BlogPostService.GetFeaturedBlogPostsAsync(5, cancellationToken: _cts.Token),
                BlogPostService.GetPopularBlogPostsAsync(4, cancellationToken: _cts.Token),
                BlogPostService.GetRecentBlogPostsAsync(6, cancellationToken: _cts.Token)
            );

            _featured = posts[0];
            _popular = posts[1];
            _recent = posts[2];
            _siteSettings = await SiteSettingsService.GetSiteSettingsAsync(_cts.Token);

            if (_featured.Length == 0)
            {
                return;
            }

            _firstFeatured = _featured[0];
            _featured = _featured.Skip(1).ToArray();

            try
            {
                _tags = await TagService.GetTopTagsAsync(20, _cts.Token);
            }
            catch
            {
                // ignored
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        private string[] HomeHeroTags => _siteSettings.HomeHeroTags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private bool ShowHomeHeroSecondaryButton =>
            !string.IsNullOrWhiteSpace(_siteSettings.HomeHeroSecondaryButtonText) &&
            !string.IsNullOrWhiteSpace(_siteSettings.HomeHeroSecondaryButtonUrl);

        private bool IsHomeHeroSecondaryGitHubLink =>
            Uri.TryCreate(_siteSettings.HomeHeroSecondaryButtonUrl, UriKind.Absolute, out var url) &&
            string.Equals(url.Host, "github.com", StringComparison.OrdinalIgnoreCase);

        private bool ShowHomeAboutLink =>
            !string.IsNullOrWhiteSpace(_siteSettings.HomeAboutLinkText) &&
            !string.IsNullOrWhiteSpace(_siteSettings.HomeAboutLinkUrl);

        private bool IsHomeAboutExternalLink =>
            Uri.TryCreate(_siteSettings.HomeAboutLinkUrl, UriKind.Absolute, out var url) &&
            (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps);

        private bool IsHomeAboutGitHubLink =>
            Uri.TryCreate(_siteSettings.HomeAboutLinkUrl, UriKind.Absolute, out var url) &&
            string.Equals(url.Host, "github.com", StringComparison.OrdinalIgnoreCase);

        private string? HomeAboutLinkTarget => IsHomeAboutExternalLink ? "_blank" : null;

        private string? HomeAboutLinkRel => IsHomeAboutExternalLink ? "noopener noreferrer" : null;
    }
}
