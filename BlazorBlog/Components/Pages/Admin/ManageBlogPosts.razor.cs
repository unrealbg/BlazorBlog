namespace BlazorBlog.Components.Pages.Admin
{
    using BlogPost = BlazorBlog.Domain.Entities.BlogPost;

    public partial class ManageBlogPosts
    {
        private bool _isLoading;
        private string? _loadingText;
    private bool _showLoader;

    private BlogPost _selectedBlogPost = new();
        private bool _showConfirmationModal = false;

        private List<BlogPost> _currentBlogPosts = new List<BlogPost>();

        private const int PageSize = 10;

        private PaginationState _paginationState = new PaginationState
        {
            ItemsPerPage = PageSize
        };

    private GridItemsProvider<BlogPost> _blogPostProvider { get; set; } = default!;
        private QuickGrid<BlogPost>? _grid;

        private readonly CancellationTokenSource _cts = new();

        private Dictionary<int, string> _categoryNames = new();
    private HashSet<int> _savingToggles = new();

    [Inject]
    private IBlogPostAdminService BlogPostService { get; set; } = default!;

    [Inject]
    private ICategoryService CategoryService { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Inject]
    private IToastService ToastService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Preload categories for CategoryId -> Name mapping
            var categories = await CategoryService.GetCategoriesAsync(_cts.Token);
            _categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);

            _blogPostProvider = async request =>
            {
                _isLoading = true;
                _loadingText = "Fetching blog posts";
        _ = Task.Run(async () =>
                {
                    await Task.Delay(250, _cts.Token);
                    if (_isLoading)
                    {
                        _showLoader = true;
            await InvokeAsync(StateHasChanged);
                    }
                });

                var pagedBlogs = await BlogPostService.GetBlogPostsAsync(request.StartIndex, request.Count ?? PageSize, _cts.Token);
                _currentBlogPosts = pagedBlogs.Records.ToList();

                _isLoading = false;
                _showLoader = false;
                StateHasChanged();

                return GridItemsProviderResult.From(_currentBlogPosts, pagedBlogs.TotalCount);
            };
        }

        private string GetCategoryName(int categoryId)
            => _categoryNames.TryGetValue(categoryId, out var name) ? name : $"#{categoryId}";

        private async Task HandleFeaturedChanged(BlogPost blogPost)
        {
            var id = blogPost.Id;
            var prev = !blogPost.IsFeatured; // because bind already flipped it
            _savingToggles.Add(id);
            await InvokeAsync(StateHasChanged);

            bool success = false;
            try
            {
                success = await SaveChangesAsync(blogPost);
                if (!success)
                {
                    blogPost.IsFeatured = prev;
                }
            }
            finally
            {
                _savingToggles.Remove(id);
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task HandlePublishedChanged(BlogPost blogPost)
        {
            var id = blogPost.Id;
            var prev = !blogPost.IsPublished; // because bind already flipped it
            _savingToggles.Add(id);
            await InvokeAsync(StateHasChanged);

            bool success = false;
            try
            {
                success = await SaveChangesAsync(blogPost);
                if (!success)
                {
                    blogPost.IsPublished = prev;
                }
            }
            finally
            {
                _savingToggles.Remove(id);
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task RefreshGridAsync()
        {
            if (_grid is not null)
            {
                await _grid.RefreshDataAsync();
            }
        }

        private async Task<bool> SaveChangesAsync(BlogPost blogPost)
        {
            // Avoid full-screen loader for quick toggle; keep UI responsive

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.GetUserId();

            BlazorBlog.Domain.Entities.BlogPost? updatedBlogPost = null;
            try
            {
                updatedBlogPost = await BlogPostService.SaveBlogPostAsync(blogPost, userId, _cts.Token);
            }
            catch
            {
                updatedBlogPost = null;
            }

            if (updatedBlogPost != null)
            {
                var index = _currentBlogPosts.FindIndex(b => b.Id == updatedBlogPost.Id);
                if (index != -1)
                {
                    _currentBlogPosts[index] = updatedBlogPost;
                }
                return true;
            }

            ToastService.ShowToast(ToastLevel.Error, "Failed to save changes.", heading: "Error");
            return false;
        }

        private async Task HandleDeleteBlogPost(BlogPost blogPost)
        {
            _loadingText = "Deleting blog post";
            _isLoading = true;
            _showLoader = true;

            var isDeleted = await BlogPostService.DeleteBlogPostAsync(blogPost.Id, _cts.Token);

            if (isDeleted)
            {
                ToastService.ShowToast(ToastLevel.Success, $"Blog post <span style='color:yellow;'>*{blogPost.Title}*</span> deleted successfully.", heading: "Success");
                _currentBlogPosts.Remove(blogPost);
                await RefreshGridAsync();
            }

            _isLoading = false;
            _showLoader = false;
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
                await HandleDeleteBlogPost(_selectedBlogPost);
            }
        }
    }
}
