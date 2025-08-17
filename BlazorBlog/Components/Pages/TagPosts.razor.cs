namespace BlazorBlog.Components.Pages
{
    public partial class TagPosts
    {
        private string _tagName = string.Empty;
        private BlogPostVm[] _recent = [];
        private BlogPostVm[] _popular = [];

        private bool HasPosts => _recent.Length > 0;

        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] IBlogPostService BlogPostService { get; set; } = default!;
        [Inject] ITagService TagService { get; set; } = default!;

        [Parameter] public string TagSlug { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrWhiteSpace(TagSlug))
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            try
            {
                var top = await TagService.GetTopTagsAsync(200);
                var match = top.FirstOrDefault(t => string.Equals(t.Slug, TagSlug, StringComparison.OrdinalIgnoreCase));
                _tagName = match?.Name ?? TagSlug.Replace('-', ' ');
            }
            catch
            {
                _tagName = TagSlug.Replace('-', ' ');
            }

            var posts = await Task.WhenAll(
                BlogPostService.GetPopularBlogPostsByTagAsync(TagSlug, 4),
                BlogPostService.GetRecentBlogPostsByTagAsync(TagSlug, 5)
            );

            _popular = posts[0];
            _recent = posts[1];
        }
    }
}
