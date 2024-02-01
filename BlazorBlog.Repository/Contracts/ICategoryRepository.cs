namespace BlazorBlog.Repository.Contracts
{
    using BlazorBlog.Data.Entities;

    public interface ICategoryRepository
    {
        Task<Category[]> GetCategoriesAsync();

        Task<Category> SaveCategoryAsync(Category category);

        Task<bool> DeleteCategoryAsync(int id);

        Task<Category?> GetCategoryBySlugAsync(string slug);
    }

}
