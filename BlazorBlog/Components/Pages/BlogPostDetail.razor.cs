namespace BlazorBlog.Components.Pages
{
    public partial class BlogPostDetail
    {
        private BlogPostVm _blogPost = new BlogPostVm();
        private BlogPostVm[] _relatedPosts = [];
        private string _categorySlug = string.Empty;
        private string _categoryName = string.Empty;
        private string _authorName = string.Empty;
        private string _publishedAt = string.Empty;

    [Inject] 
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject] 
    IBlogPostService BlogPostService { get; set; } = default!;

    [Parameter]
    public string BlogPostSlug { get; set; } = string.Empty;

        private readonly CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            var result = await BlogPostService.GetBlogPostBySlugAsync(BlogPostSlug, _cts.Token);

            if (result.IsEmpty)
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            _blogPost = result.BlogPost;
            _relatedPosts = result.RelatedPosts;

            // Pull enriched fields from VM once Service populates them
            _categorySlug = _blogPost.CategorySlug ?? string.Empty;
            _categoryName = _blogPost.CategoryName ?? string.Empty;
            _authorName = _blogPost.AuthorName ?? string.Empty;
            _publishedAt = _blogPost.PublishedAtDisplay ?? string.Empty;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
