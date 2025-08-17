namespace BlazorBlog.Components.Pages
{
    using BlazorBlog.Application.Utilities;

    public partial class BlogPostDetail
    {
        private BlogPostVm _blogPost = new BlogPostVm();
        private BlogPostVm[] _relatedPosts = [];
        private BlogPostVm[] _popularInCategory = [];
        private string _categorySlug = string.Empty;
        private string _categoryName = string.Empty;
        private string _authorName = string.Empty;
        private string _publishedAt = string.Empty;
        private string _readTime = string.Empty;
        private int _wordCount = 0;

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

            _categorySlug = _blogPost.CategorySlug ?? string.Empty;
            _categoryName = _blogPost.CategoryName ?? string.Empty;
            _authorName = _blogPost.AuthorName ?? string.Empty;
            _publishedAt = _blogPost.PublishedAtDisplay ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(_blogPost.ReadingTime))
            {
                _readTime = _blogPost.ReadingTime;
                _wordCount = _blogPost.WordCount;
            }
            else
            {
                var (readTime, wordCount) = ReadingTimeCalculator.Calculate(_blogPost.Content);
                _readTime = readTime;
                _wordCount = wordCount;
            }

            await LoadPopularInCategoryAsync();
        }

        private async Task LoadPopularInCategoryAsync()
        {
            var categoryId = _blogPost.CategoryId;
            if (categoryId > 0)
            {
                _popularInCategory = await BlogPostService.GetPopularBlogPostsAsync(4, categoryId, _cts.Token);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
