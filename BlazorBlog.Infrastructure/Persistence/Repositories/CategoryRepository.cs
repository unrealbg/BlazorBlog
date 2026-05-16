namespace BlazorBlog.Infrastructure.Persistence.Repositories
{
    using BlazorBlog.Domain.Entities;
    using BlazorBlog.Application.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;

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
            var data = await context.Categories.AsNoTracking().ToArrayAsync(cancellationToken);
            return data.Select(MapToDomain).ToArray();
        }

        public async Task<Category> SaveCategoryAsync(Category category, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            BlazorBlog.Infrastructure.Persistence.Entities.Category entity;
            var normalizedName = category.Name.Trim();
            var normalizedSlug = category.Slug.Trim();
            var normalizedNameLower = normalizedName.ToLowerInvariant();
            var normalizedSlugLower = normalizedSlug.ToLowerInvariant();

            if (category.Id == 0)
            {
                var exists = await context.Categories.AsNoTracking()
                    .AnyAsync(c => c.Name.ToLower() == normalizedNameLower || c.Slug.ToLower() == normalizedSlugLower, cancellationToken);
                if (exists)
                {
                    throw new InvalidOperationException($"Category with the name {normalizedName} already exists.");
                }
                entity = new BlazorBlog.Infrastructure.Persistence.Entities.Category
                {
                    Name = normalizedName,
                    Slug = normalizedSlug,
                    ShowOnNavBar = category.ShowOnNavBar
                };
                await context.Categories.AddAsync(entity, cancellationToken);
            }
            else
            {
                var exists = await context.Categories.AsNoTracking()
                    .AnyAsync(c => c.Id != category.Id && (c.Name.ToLower() == normalizedNameLower || c.Slug.ToLower() == normalizedSlugLower), cancellationToken);
                if (exists)
                {
                    throw new InvalidOperationException($"Category with the name {normalizedName} already exists.");
                }

                entity = await context.Categories.FindAsync(new object[] { category.Id }, cancellationToken)
                    ?? throw new InvalidOperationException("Category not found.");

                entity.Name = normalizedName;
                entity.Slug = normalizedSlug;
                entity.ShowOnNavBar = category.ShowOnNavBar;
            }

            await context.SaveChangesAsync(cancellationToken);
            return MapToDomain(entity);
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await context.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (entity is null) return false;

            var hasPosts = await context.BlogPosts.AnyAsync(p => p.CategoryId == id && !p.IsDeleted, cancellationToken);
            if (hasPosts)
            {
                throw new InvalidOperationException("Cannot delete a category that contains blog posts.");
            }

            context.Categories.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
            return entity is null ? null : MapToDomain(entity);
        }

        private static Category MapToDomain(BlazorBlog.Infrastructure.Persistence.Entities.Category e) => new()
        {
            Id = e.Id,
            Name = e.Name,
            Slug = e.Slug,
            ShowOnNavBar = e.ShowOnNavBar
        };
    }
}
