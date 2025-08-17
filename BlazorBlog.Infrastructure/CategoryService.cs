namespace BlazorBlog.Infrastructure
{
    using System.Threading;

    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Infrastructure.Contracts;

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISlugService _slugService;

        public CategoryService(ICategoryRepository categoryRepository, ISlugService slugService)
        {
            _categoryRepository = categoryRepository;
            _slugService = slugService;
        }

        public async Task<Category[]> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetCategoriesAsync(cancellationToken);
        }

        public async Task<Category> SaveCategoryAsync(Category category, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new InvalidOperationException("Category name is required.");
            }

            var baseSlug = _slugService.GenerateSlug(category.Name);
            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                baseSlug = "category"; 
            }

            var uniqueSlug = await EnsureUniqueSlugAsync(baseSlug, category.Id, cancellationToken);
            category.Slug = uniqueSlug;

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

        private async Task<string> EnsureUniqueSlugAsync(string baseSlug, int currentCategoryId, CancellationToken ct)
        {
            var slug = baseSlug;
            var suffix = 1;

            while (true)
            {
                var existing = await _categoryRepository.GetCategoryBySlugAsync(slug, ct);
                if (existing is null || existing.Id == currentCategoryId)
                {
                    return slug; 
                }

                slug = $"{baseSlug}-{suffix++}";
            }
        }
    }
}
