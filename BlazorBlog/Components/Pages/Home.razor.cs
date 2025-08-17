namespace BlazorBlog.Components.Pages
{
    public partial class Home
    {
        private BlogPostVm[] _featured = [];
        private BlogPostVm[] _popular = [];
        private BlogPostVm[] _recent = [];
        private TagVm[] _tags = Array.Empty<BlazorBlog.Application.Models.TagVm>();

        private BlogPostVm _firstFeatured = default!;

        [Inject]
        IBlogPostService BlogPostService { get; set; } = default!;
        [Inject]
        BlazorBlog.Infrastructure.Contracts.ITagService TagService { get; set; } = default!;

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
    }
}
