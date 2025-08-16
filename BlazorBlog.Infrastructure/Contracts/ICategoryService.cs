namespace BlazorBlog.Infrastructure.Contracts
{
    using System.Threading;
    using BlazorBlog.Domain.Entities;

    public interface ICategoryService
    {
        Task<Category[]> GetCategoriesAsync(CancellationToken cancellationToken = default);

        Task<Category> SaveCategoryAsync(Category category, CancellationToken cancellationToken = default);

        Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default);

        Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    }
}