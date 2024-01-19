namespace BlazorBlog.Components.Pages.Admin
{
    public partial class ManageBlogPosts
    {
        private bool _isLoading;
        private string? _loadingText;

        private const int PageSize = 10;

        private PaginationState _paginationState = new PaginationState
        {
            ItemsPerPage = PageSize
        };

        private GridItemsProvider<BlogPost> _blogPostProvider { get; set; }

        [Inject]
        private IBlogPostAdminService BlogPostService { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override void OnInitialized()
        {
            _blogPostProvider = async request =>
            {
                _isLoading = true;
                _loadingText = "Fetching blog posts";

                StateHasChanged();

                var pagedBlogs = await BlogPostService.GetBlogPostsAsync(request.StartIndex, request.Count ?? PageSize);

                _isLoading = false;
                StateHasChanged();

                return GridItemsProviderResult.From(pagedBlogs.Records, pagedBlogs.TotalCount);
            };
        }

        private async Task HandleFeaturedChanged(BlogPost blogPost)
        {
            blogPost.IsFeatured = !blogPost.IsFeatured;
            await SaveChangesAsync(blogPost);
        }

        private async Task HandlePublishedChanged(BlogPost blogPost)
        {
            blogPost.IsPublished = !blogPost.IsPublished;
            await SaveChangesAsync(blogPost);
        }

        private async Task SaveChangesAsync(BlogPost blogPost)
        {
            _loadingText = "Saving changes";
            _isLoading = true;

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.GetUserId();

            await BlogPostService.SaveBlogPostAsync(blogPost, userId);
            _isLoading = false;
        }
    }
}
