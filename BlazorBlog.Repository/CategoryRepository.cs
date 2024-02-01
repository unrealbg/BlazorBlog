namespace BlazorBlog.Repository
{
    using Common;
    using Data.Entities;
    using Data;
    using Contracts;
    using Microsoft.EntityFrameworkCore;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CategoryRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Category[]> GetCategoriesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Categories.AsNoTracking().ToArrayAsync();
        }

        public async Task<Category> SaveCategoryAsync(Category category)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            if (category.Id == 0)
            {
                var existingCategory = await context.Categories.AsNoTracking()
                    .AnyAsync(c => c.Name == category.Name);
                if (existingCategory)
                {
                    throw new InvalidOperationException($"Category with the name {category.Name} already exists.");
                }
                category.Slug = category.Name.ToSlug();
                await context.Categories.AddAsync(category);
            }
            else
            {
                var dbCategory = await context.Categories.FindAsync(category.Id);
                if (dbCategory != null)
                {
                    dbCategory.Name = category.Name;
                    dbCategory.Slug = category.Name.ToSlug();
                    dbCategory.ShowOnNavBar = category.ShowOnNavBar;
                }
            }
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var category = await context.Categories.FindAsync(id);
            if (category == null) return false;
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Category?> GetCategoryBySlugAsync(string slug)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug);
        }
    }
}
