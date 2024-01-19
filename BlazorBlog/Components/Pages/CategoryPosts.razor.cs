namespace BlazorBlog.Components.Pages
{
    public partial class CategoryPosts
    {
        private Category _category;

        private BlogPost? _featured;
        private BlogPost[] _popular = [];
        private BlogPost[] _recent = [];

        private bool HasPosts => _featured is not null;

        [Inject] NavigationManager NavigationManager { get; set; }

        [Inject] IBlogPostService BlogPostService { get; set; }

        [Inject] ICategoryService CategoryService { get; set; }

        [Parameter]
        public string CategorySlug { get; set; }

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
