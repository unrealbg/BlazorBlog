namespace BlazorBlog.Services
{
    using System.Threading;

    using Repository.Contracts;

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category[]> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetCategoriesAsync(cancellationToken);
        }

        public async Task<Category> SaveCategoryAsync(Category category, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.SaveCategoryAsync(category, cancellationToken);
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.DeleteCategoryAsync(id, cancellationToken);
        }

        public async Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetCategoryBySlugAsync(slug, cancellationToken);
        }
    }
}
