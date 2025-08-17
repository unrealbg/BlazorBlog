namespace BlazorBlog.Components.Pages
{
    using Category = BlazorBlog.Domain.Entities.Category;

    public partial class AllPosts : IDisposable
    {
        private const int PageSize = 8;

        private bool _isLoading = true;
        private BlogPostVm[] _posts = [];
        private BlogPostVm[] _popularPosts = [];
        private Category[] _categories = [];
        
        private int _currentPage = 1;
        private int _totalPages = 1;

        private readonly CancellationTokenSource _cts = new();

        [Inject]
        IBlogPostService BlogPostService { get; set; } = default!;

        [Inject]
        ICategoryService CategoryService { get; set; } = default!;

        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        IToastService ToastService { get; set; } = default!;

        [Parameter]
        public int? PageNumber { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _currentPage = PageNumber ?? 1;
            if (_currentPage < 1)
            {
                NavigationManager.NavigateTo("/posts", replace: true);
                return;
            }

            await LoadDataAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            var newPage = PageNumber ?? 1;
            if (newPage != _currentPage && newPage >= 1)
            {
                _currentPage = newPage;
                _isLoading = true;
                await LoadPostsAsync();
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task LoadDataAsync()
        {
            _isLoading = true;

            try
            {
                var tasks = new Task[]
                {
                    LoadPostsAsync(),
                    LoadPopularPostsAsync(),
                    LoadCategoriesAsync()
                };

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "Error loading posts", "An error occurred while loading the blog posts. Please try again later.");
                Console.Error.WriteLine($"Error loading posts: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadPostsAsync()
        {
            var pageIndex = _currentPage - 1;
            _posts = await BlogPostService.GetBlogPostsAsync(pageIndex, PageSize, categoryId: 0, _cts.Token);
            
            if (_posts.Length == PageSize)
            {
                _totalPages = _currentPage + 1; 
            }
            else
            {
                _totalPages = _currentPage;
            }
        }

        private async Task LoadPopularPostsAsync()
        {
            _popularPosts = await BlogPostService.GetPopularBlogPostsAsync(5, cancellationToken: _cts.Token);
        }

        private async Task LoadCategoriesAsync()
        {
            _categories = await CategoryService.GetCategoriesAsync(_cts.Token);
        }

        private string GetPageUrl(int pageNumber)
        {
            return pageNumber == 1 ? "/posts" : $"/posts/{pageNumber}";
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}