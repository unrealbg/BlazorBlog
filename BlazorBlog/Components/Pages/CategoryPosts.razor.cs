namespace BlazorBlog.Components.Pages
{
    using Category = BlazorBlog.Domain.Entities.Category;

    public partial class CategoryPosts
    {
        private Category _category = new();

        private BlogPostVm? _featured;
        private BlogPostVm[] _popular = [];
        private BlogPostVm[] _recent = [];

        private bool HasPosts => _featured is not null;

    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    [Inject] IBlogPostService BlogPostService { get; set; } = default!;

    [Inject] ICategoryService CategoryService { get; set; } = default!;

        [Parameter]
    public string CategorySlug { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var category = await CategoryService.GetCategoryBySlugAsync(CategorySlug);

            if (category is null)
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            _category = category;

            var posts = await Task.WhenAll(
                BlogPostService.GetFeaturedBlogPostsAsync(1, _category.Id),
                BlogPostService.GetPopularBlogPostsAsync(4, _category.Id),
                BlogPostService.GetRecentBlogPostsAsync(5, _category.Id)
            );

            _featured = posts[0].FirstOrDefault();
            _popular = posts[1];
            _recent = posts[2];
        }
    }
}
