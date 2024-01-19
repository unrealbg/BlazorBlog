namespace BlazorBlog.Components.Pages
{
    public partial class PagedCategoryPosts
    {
        private const int PageSize = 2;

        private string PageTitle => $"{_category?.Name} Posts {(_pageNumber > 1 ? $"(Page - {_pageNumber})" : "")}";
        private int _pageNumber = 1;
        private Category _category;
        private BlogPost[] _posts = [];
        private BlogPost[] _popular = [];

        [Inject] 
        NavigationManager NavigationManager { get; set; }

        [Inject] 
        IBlogPostService BlogPostService { get; set; }

        [Inject] 
        ICategoryService CategoryService { get; set; }

        [Parameter]
        public string CategorySlug { get; set; }

        [Parameter]
        public int? UriPageNumber { get; set; }

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

            var category = await CategoryService.GetCategoryBySlugAsync(CategorySlug);

            if (category is null)
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            _category = category;

            var posts = await Task.WhenAll(
                BlogPostService.GetPopularBlogPostsAsync(4, _category.Id),
                BlogPostService.GetBlogPostsAsync(_pageNumber - 1, PageSize, _category.Id)
            );

            _popular = posts[0];
            _posts = posts[1];
        }
    }
}
