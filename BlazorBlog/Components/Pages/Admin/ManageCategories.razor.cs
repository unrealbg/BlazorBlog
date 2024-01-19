namespace BlazorBlog.Components.Pages.Admin
{
    public partial class ManageCategories
    {
        private bool _isLoading;
        private string? _loadingText;

        private Category? _operatingCategory;
        private IQueryable<Category> _categories = Enumerable.Empty<Category>().AsQueryable();

        protected override async Task OnInitializedAsync() => await LoadCategoriesAsync();

        [Inject] 
        ICategoryService CategoryService { get; set; }

        [Inject] 
        NavigationManager NavigationManager { get; set; }

        private async Task HandleShowOnNavBarChanged(Category category)
        {
            _loadingText = "Saving changes";
            _isLoading = true;
            category.ShowOnNavBar = !category.ShowOnNavBar;
            await CategoryService.SaveCategoryAsync(category);
            _isLoading = false;
            NavigationManager.Refresh();
        }

        private void HandleEditCategory(Category category) => _operatingCategory = category.Clone();

        private async Task SaveCategoryAsync()
        {
            if (_operatingCategory is not null)
            {
                _loadingText = "Saving changes";
                _isLoading = true;
                await CategoryService.SaveCategoryAsync(_operatingCategory);
                _operatingCategory = null;
                _isLoading = false;

                await LoadCategoriesAsync();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            _loadingText = "Fetching categories";
            _isLoading = true;
            _categories = (await CategoryService.GetCategoriesAsync()).AsQueryable();
            _isLoading = false;
        }
    }
}
