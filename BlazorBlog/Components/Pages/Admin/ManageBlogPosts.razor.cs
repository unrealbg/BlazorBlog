namespace BlazorBlog.Components.Pages.Admin
{
    public partial class ManageBlogPosts
    {
        private bool _isLoading;
        private string? _loadingText;

        private BlogPost _selectedBlogPost;
        private bool _showConfirmationModal = false;

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

        [Inject]
        private IToastService ToastService { get; set; }

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

        private async Task HandleDeleteBlogPost(BlogPost blogPost)
        {
            _loadingText = "Deleting blog post";
            _isLoading = true;

            var isDeleted = await BlogPostService.DeleteBlogPostAsync(blogPost.Id);

            if (isDeleted)
            {
                ToastService.ShowToast(ToastLevel.Success, "Blog post deleted successfully.", heading: "Success");
            }
            else
            {
                ToastService.ShowToast(ToastLevel.Error, "Something went wrong while deleting the blog post.", heading: "Error");
            }

            RefreshBlogPosts();

            _isLoading = false;
        }

        private void RefreshBlogPosts()
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

            StateHasChanged();
        }

        private void ConfirmDeleteBlogPost(BlogPost blogPost)
        {
            _selectedBlogPost = blogPost;
            _showConfirmationModal = true;
        }

        private async Task OnModalConfirm(bool isConfirmed)
        {
            _showConfirmationModal = false;
            if (isConfirmed)
            {
                if (_selectedBlogPost != null)
                {
                    await HandleDeleteBlogPost(_selectedBlogPost);
                }
            }
        }

        private void CancelDelete()
        {
            _showConfirmationModal = false;
        }
    }
}
