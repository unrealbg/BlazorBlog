namespace BlazorBlog.Components.Pages.Admin
{
    public partial class ManageBlogPosts
    {
        private bool _isLoading;
        private string? _loadingText;

        private BlogPost _selectedBlogPost;
        private bool _showConfirmationModal = false;

        private List<BlogPost> _currentBlogPosts = new List<BlogPost>();

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
                _currentBlogPosts = pagedBlogs.Records.ToList();

                _isLoading = false;
                StateHasChanged();

                return GridItemsProviderResult.From(_currentBlogPosts, pagedBlogs.TotalCount);
            };
        }

        private async Task HandleFeaturedChanged(BlogPost blogPost)
        {
            await SaveChangesAsync(blogPost);
        }

        private async Task HandlePublishedChanged(BlogPost blogPost)
        {
            await SaveChangesAsync(blogPost);
        }

        private async Task SaveChangesAsync(BlogPost blogPost)
        {
            _loadingText = "Saving changes";
            _isLoading = true;

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.GetUserId();

            var updatedBlogPost = await BlogPostService.SaveBlogPostAsync(blogPost, userId);

            if (updatedBlogPost != null)
            {
                var index = _currentBlogPosts.FindIndex(b => b.Id == updatedBlogPost.Id);

                if (index != -1)
                {
                    _currentBlogPosts[index] = updatedBlogPost;
                }
            }

            _isLoading = false;
            StateHasChanged();
        }


        private async Task HandleDeleteBlogPost(BlogPost blogPost)
        {
            _loadingText = "Deleting blog post";
            _isLoading = true;

            var isDeleted = await BlogPostService.DeleteBlogPostAsync(blogPost.Id);

            if (isDeleted)
            {
                ToastService.ShowToast(ToastLevel.Success, $"Blog post <span style='color:yellow;'>*{blogPost.Title}*</span> deleted successfully.", heading: "Success");
                RemovePostFromLocalCollection(blogPost.Id);
            }
            else
            {
                ToastService.ShowToast(ToastLevel.Error, "Something went wrong while deleting the blog post.", heading: "Error");
            }

            _isLoading = false;
            StateHasChanged();
        }

        private void RemovePostFromLocalCollection(int postId)
        {
            var postToRemove = _currentBlogPosts.FirstOrDefault(p => p.Id == postId);
            if (postToRemove != null)
            {
                _currentBlogPosts.Remove(postToRemove);
            }
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
