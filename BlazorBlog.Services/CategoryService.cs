namespace BlazorBlog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public CategoryService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<Category[]> GetCategoriesAsync()
        {
            return await this.ExecuteOnContext(async context =>
            {
                var categories = await context.Categories
                    .AsNoTracking()
                    .ToArrayAsync();
                return categories;
            });
        }

        public async Task<Category> SaveCategoryAsync(Category category)
        {
            return await this.ExecuteOnContext(async context =>
            {
                if (category.Id == 0)
                {
                    if (await context.Categories
                            .AsNoTracking()
                            .AnyAsync(c => c.Name == category.Name))
                    {
                        throw new InvalidOperationException($"Category with the name {category.Name} already exist");
                    }

                    category.Slug = category.Name.ToSlug();
                    await context.Categories.AddAsync(category);
                    await context.SaveChangesAsync();
                }
                else
                {
                    if (await context.Categories
                            .AsNoTracking()
                            .AnyAsync(c => c.Name == category.Name && c.Id != category.Id))
                    {
                        throw new InvalidOperationException($"Category with the name {category.Name} already exist");
                    }

                    var dbCategory = await context.Categories.FindAsync(category.Id);

                    dbCategory.Name = category.Name;
                    dbCategory.ShowOnNavBar = category.ShowOnNavBar;

                    category.Slug = dbCategory.Slug;
                }

                await context.SaveChangesAsync();
                return category;
            });
        }

        public async Task<Category?> GetCategoryBySlugAsync(string slug) =>
            await ExecuteOnContext(async context =>
                await context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Slug == slug)
                );

        private async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
        {
            await using var context = this.contextFactory.CreateDbContext();
            return await query.Invoke(context);
        }
    }
}
