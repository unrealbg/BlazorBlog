namespace BlazorBlog.Components.Pages
{
    public partial class Home
    {
        private BlogPost[] _featured = [];
        private BlogPost[] _popular = [];
        private BlogPost[] _recent = [];

        private BlogPost _firstFeatured = default!;

        [Inject] 
        IBlogPostService BlogPostService { get; set; }

        private readonly CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            var posts = await Task.WhenAll(
                BlogPostService.GetFeaturedBlogPostsAsync(5, cancellationToken: _cts.Token),
                BlogPostService.GetPopularBlogPostsAsync(4, cancellationToken: _cts.Token),
                BlogPostService.GetRecentBlogPostsAsync(5, cancellationToken: _cts.Token)
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
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
