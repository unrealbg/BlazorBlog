namespace BlazorBlog.Components.Pages
{
    using Category = BlazorBlog.Domain.Entities.Category;

    public partial class PagedCategoryPosts
    {
        private const int PageSize = 2;

        private string PageTitle => $"{_category?.Name} Posts {(_pageNumber > 1 ? $"(Page - {_pageNumber})" : "")}";
        private int _pageNumber = 1;
    private Category _category = new();
        private BlogPostVm[] _posts = [];
        private BlogPostVm[] _popular = [];

    [Inject] 
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject] 
    IBlogPostService BlogPostService { get; set; } = default!;

    [Inject] 
    ICategoryService CategoryService { get; set; } = default!;

    [Parameter]
    public string CategorySlug { get; set; } = string.Empty;

        [Parameter]
        public int? UriPageNumber { get; set; }

        private readonly CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            if (UriPageNumber.HasValue)
            {
                if (UriPageNumber < 2)
                {
                    NavigationManager.NavigateTo($"{CategorySlug}-posts/all", replace: true);
                    return;
                }

                _pageNumber = UriPageNumber.Value;
            }

            var category = await CategoryService.GetCategoryBySlugAsync(CategorySlug, _cts.Token);

            if (category is null)
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            _category = category;

            var posts = await Task.WhenAll(
                BlogPostService.GetPopularBlogPostsAsync(4, _category.Id, _cts.Token),
                BlogPostService.GetBlogPostsAsync(_pageNumber - 1, PageSize, _category.Id, _cts.Token)
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
