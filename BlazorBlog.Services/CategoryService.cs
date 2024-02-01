namespace BlazorBlog.Services
{
    using Repository.Contracts;

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category[]> GetCategoriesAsync()
        {
            return await _categoryRepository.GetCategoriesAsync();
        }

        public async Task<Category> SaveCategoryAsync(Category category)
        {
            return await _categoryRepository.SaveCategoryAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<Category?> GetCategoryBySlugAsync(string slug)
        {
            return await _categoryRepository.GetCategoryBySlugAsync(slug);
        }
    }
}
