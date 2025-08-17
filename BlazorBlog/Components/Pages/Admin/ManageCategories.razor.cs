namespace BlazorBlog.Components.Pages.Admin
{
    using Category = BlazorBlog.Domain.Entities.Category;

    public partial class ManageCategories : IDisposable
    {
        private bool _isLoading;
        private string? _loadingText;

        private bool _showConfirmationModal = false;

        private Category? _operatingCategory; // for add/edit form only
        private Category? _categoryToDelete;  // for delete confirmation only
        private IQueryable<Category> _categories = Enumerable.Empty<Category>().AsQueryable();

        private readonly CancellationTokenSource _cts = new();

        private const int PageSize = 10;
        private PaginationState _paginationState = new PaginationState
        {
            ItemsPerPage = PageSize
        };

        private EditContext _editContext = default!;
        private ValidationMessageStore? _messageStore;
    private HashSet<int> _savingToggles = new();

    [Inject]
    IValidator<Category> Validator { get; set; } = default!;

        protected override async Task OnInitializedAsync() => await LoadCategoriesAsync();

    [Inject]
    ICategoryService CategoryService { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    IToastService ToastService { get; set; } = default!;

        private async Task HandleShowOnNavBarChanged(Category category)
        {
            var id = category.Id;
            var prev = !category.ShowOnNavBar; // bind already flipped it
            _savingToggles.Add(id);
            await InvokeAsync(StateHasChanged);

            try
            {
                await CategoryService.SaveCategoryAsync(category, _cts.Token);
            }
            catch (Exception ex)
            {
                category.ShowOnNavBar = prev; // revert on failure
                ToastService.ShowToast(ToastLevel.Error, ex.Message, heading: "Error", durationMs: 8000);
            }
            finally
            {
                _savingToggles.Remove(id);
                await InvokeAsync(StateHasChanged);
            }
        }

        private void StartAddCategory()
        {
            _operatingCategory = new Category();
            _editContext = new EditContext(_operatingCategory);
            _messageStore = new ValidationMessageStore(_editContext);
        }

        private void HandleEditCategory(Category category)
        {
            _operatingCategory = new Category { Id = category.Id, Name = category.Name, Slug = category.Slug, ShowOnNavBar = category.ShowOnNavBar };
            _editContext = new EditContext(_operatingCategory);
            _messageStore = new ValidationMessageStore(_editContext);
        }

        private void CancelEdit()
        {
            _operatingCategory = null;
            _editContext = new EditContext(new Category());
            _messageStore = new ValidationMessageStore(_editContext);
        }

        private async Task SaveCategoryAsync()
        {
            if (_operatingCategory is not null)
            {
                _messageStore!.Clear();

                var validation = await Validator.ValidateAsync(_operatingCategory, _cts.Token);
                if (!validation.IsValid)
                {
                    foreach (var error in validation.Errors)
                    {
                        var fi = new FieldIdentifier(_operatingCategory, error.PropertyName);
                        _messageStore.Add(fi, error.ErrorMessage);
                    }
                    _editContext.NotifyValidationStateChanged();
                    return;
                }

                _loadingText = "Saving changes";
                _isLoading = true;

                var isCategoryExisting = _operatingCategory.Id > 0;

                try
                {
                    var saved = await CategoryService.SaveCategoryAsync(_operatingCategory, _cts.Token);
                    _operatingCategory = saved;

                    var operation = isCategoryExisting ? "updated" : "added";
                    ToastService.ShowToast(ToastLevel.Success, $"Category {operation} successfully.", heading: "Success", durationMs: 5000);

                    _operatingCategory = null;

                    await LoadCategoriesAsync();
                }
                catch (InvalidOperationException ex)
                {
                    var nameField = new FieldIdentifier(_operatingCategory!, nameof(Category.Name));
                    _messageStore.Add(nameField, ex.Message);
                    _editContext.NotifyValidationStateChanged();

                    ToastService.ShowToast(ToastLevel.Warning, ex.Message, heading: "Validation", durationMs: 8000);
                }
                catch (Exception ex)
                {
                    ToastService.ShowToast(ToastLevel.Error, "Failed to save category. Please try again.", heading: "Error", durationMs: 10000);
                    Console.Error.WriteLine(ex);
                }
                finally
                {
                    _isLoading = false;
                }
            }
        }

        private async Task LoadCategoriesAsync()
        {
            _loadingText = "Fetching categories";
            _isLoading = true;
            _categories = (await CategoryService.GetCategoriesAsync(_cts.Token)).AsQueryable();
            _isLoading = false;
        }

        private void ConfirmDeleteCategory(Category category)
        {
            _categoryToDelete = category; // keep separate from editing
            _showConfirmationModal = true;
        }

        private async Task DeleteCategoryAsync(int id)
        {
            _loadingText = "Deleting category";
            _isLoading = true;

            var isDeleted = await CategoryService.DeleteCategoryAsync(id, _cts.Token);

            if (!isDeleted)
            {
                ToastService.ShowToast(ToastLevel.Error, "Failed to delete category", heading: "Error");
                _isLoading = false;
                return;
            }

            ToastService.ShowToast(ToastLevel.Success, "Category deleted successfully.", heading: "Success");

            _operatingCategory = null; // ensure form not rendered
            _categoryToDelete = null;

            _isLoading = false;

            await LoadCategoriesAsync();
        }

        private async Task OnModalConfirm(bool isConfirmed)
        {
            _showConfirmationModal = false;
            if (isConfirmed && _categoryToDelete is not null)
            {
                await DeleteCategoryAsync(_categoryToDelete.Id);
            }
            _categoryToDelete = null;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
