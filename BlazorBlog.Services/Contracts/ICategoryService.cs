namespace BlazorBlog.Services.Contracts;

public interface ICategoryService
{
    Task<Category[]> GetCategoriesAsync();

    Task<Category> SaveCategoryAsync(Category category);

    Task<Category?> GetCategoryBySlugAsync(string slug);

    Task<bool> DeleteCategoryAsync(int id);
}