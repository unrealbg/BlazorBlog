namespace BlazorBlog.Components.Pages.Admin
{
    public partial class ManageCategories
    {
        private bool _isLoading;
        private string? _loadingText;

        private bool _showConfirmationModal = false;

        private Category? _operatingCategory;
        private IQueryable<Category> _categories = Enumerable.Empty<Category>().AsQueryable();

        protected override async Task OnInitializedAsync() => await LoadCategoriesAsync();

        [Inject]
        ICategoryService CategoryService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        IToastService ToastService { get; set; }

        private async Task HandleShowOnNavBarChanged(Category category)
        {
            _loadingText = "Saving changes";
            _isLoading = true;
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

                var isCategoryExisting = _operatingCategory.Id > 0;

                await CategoryService.SaveCategoryAsync(_operatingCategory);

                var operation = isCategoryExisting ? "updated" : "added";
                ToastService.ShowToast(ToastLevel.Success, $"Category {operation} successfully.", heading: "Success");

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

        private void ConfirmDeleteCategory(Category category)
        {
            _operatingCategory = category;
            _showConfirmationModal = true;
        }

        private async Task DeleteCategoryAsync(int id)
        {
            if (_operatingCategory is not null)
            {
                _loadingText = "Deleting category";
                _isLoading = true;
                var isDeleted = await CategoryService.DeleteCategoryAsync(id);

                if (!isDeleted)
                {
                    ToastService.ShowToast(ToastLevel.Error, "Failed to delete category", heading: "Error");
                    _isLoading = false;
                    return;
                }

                ToastService.ShowToast(ToastLevel.Success, "Category deleted successfully.", heading: "Success");

                _operatingCategory = null;

                _isLoading = false;

                await LoadCategoriesAsync();
            }
        }

        private async Task OnModalConfirm(bool isConfirmed)
        {
            _showConfirmationModal = false;
            if (isConfirmed)
            {
                if (_operatingCategory != null)
                {
                    await DeleteCategoryAsync(_operatingCategory.Id);
                }
            }
        }

        private void CancelDelete()
        {
            _showConfirmationModal = false;
        }
    }
}
