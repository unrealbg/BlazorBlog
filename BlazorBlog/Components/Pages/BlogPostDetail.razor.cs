namespace BlazorBlog.Components.Pages
{
    using System.Text.RegularExpressions;

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

            // Pull enriched fields from VM once Service populates them
            _categorySlug = _blogPost.CategorySlug ?? string.Empty;
            _categoryName = _blogPost.CategoryName ?? string.Empty;
            _authorName = _blogPost.AuthorName ?? string.Empty;
            _publishedAt = _blogPost.PublishedAtDisplay ?? string.Empty;

            var (readTime, wordCount) = ComputeReadTimeAndCount(_blogPost.Content);
            _readTime = readTime;
            _wordCount = wordCount;

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

        private static (string readTime, int wordCount) ComputeReadTimeAndCount(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return (string.Empty, 0);
            var text = Regex.Replace(html, "<[^>]+>", " ");
            var words = Regex.Matches(text, @"\b[\p{L}\p{M}\w']+\b", RegexOptions.Multiline).Count;
            if (words == 0) return (string.Empty, 0);
            var minutes = Math.Max(1, (int)Math.Ceiling(words / 200.0)); // ~200 wpm
            return ($"{minutes} min read", words);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
