namespace BlazorBlog.Repository
{
    using Data.Entities;
    using Data;
    using Contracts;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;

    public static class SlugHelper
    {
        public static string ToSlug(string text)
        {
            text = text.ToLowerInvariant().Replace(' ', '-');
            text = System.Text.RegularExpressions.Regex.Replace(text, "[^0-9a-z_]", "-");
            return text.Replace("--", "-").Trim('-');
        }
    }

    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CategoryRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Category[]> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Categories.AsNoTracking().ToArrayAsync(cancellationToken);
        }

        public async Task<Category> SaveCategoryAsync(Category category, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            if (category.Id == 0)
            {
                var existingCategory = await context.Categories.AsNoTracking()
                    .AnyAsync(c => c.Name == category.Name, cancellationToken);
                if (existingCategory)
                {
                    throw new InvalidOperationException($"Category with the name {category.Name} already exists.");
                }
                category.Slug = SlugHelper.ToSlug(category.Name);
                await context.Categories.AddAsync(category, cancellationToken);
            }
            else
            {
                var dbCategory = await context.Categories.FindAsync(new object[] { category.Id }, cancellationToken);
                if (dbCategory != null)
                {
                    dbCategory.Name = category.Name;
                    dbCategory.Slug = SlugHelper.ToSlug(category.Name);
                    dbCategory.ShowOnNavBar = category.ShowOnNavBar;
                }
            }
            await context.SaveChangesAsync(cancellationToken);
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var category = await context.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (category == null) return false;
            context.Categories.Remove(category);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
        }
    }
}
