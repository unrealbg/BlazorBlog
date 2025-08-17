namespace BlazorBlog.Components.Pages
{
    public partial class PagedTagPosts : IDisposable
    {
        private const int PageSize = 6;
        private string _tagName = string.Empty;
        private int _pageNumber = 1;
        private BlogPostVm[] _posts = [];
        private BlogPostVm[] _popular = [];

        private string PageTitle => $"#{_tagName} Posts {(_pageNumber > 1 ? $"(Page - {_pageNumber})" : "")}";

        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] IBlogPostService BlogPostService { get; set; } = default!;
        [Inject] BlazorBlog.Infrastructure.Contracts.ITagService TagService { get; set; } = default!;

        [Parameter] public string TagSlug { get; set; } = string.Empty;
        [Parameter] public int? UriPageNumber { get; set; }

        private readonly CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            if (UriPageNumber.HasValue)
            {
                if (UriPageNumber < 2)
                {
                    NavigationManager.NavigateTo($"/tag/{TagSlug}/all", replace: true);
                    return;
                }
                _pageNumber = UriPageNumber.Value;
            }

            try
            {
                var top = await TagService.GetTopTagsAsync(200, _cts.Token);
                var match = top.FirstOrDefault(t => string.Equals(t.Slug, TagSlug, StringComparison.OrdinalIgnoreCase));
                _tagName = match?.Name ?? TagSlug.Replace('-', ' ');
            }
            catch
            {
                _tagName = TagSlug.Replace('-', ' ');
            }

            var posts = await Task.WhenAll(
                BlogPostService.GetPopularBlogPostsByTagAsync(TagSlug, 4, _cts.Token),
                BlogPostService.GetBlogPostsByTagAsync(TagSlug, _pageNumber - 1, PageSize, _cts.Token)
            );

            _popular = posts[0];
            _posts = posts[1];
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
