namespace BlazorBlog.Services
{
    using Data.Entities;
    using Contracts;

    public class BlogPostService : IBlogPostService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BlogPostService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private async Task<TResult> QueryOnContextAsync<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
        {
            using var context = _contextFactory.CreateDbContext();
            return await query(context);
        }

        public async Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0)
        {
            return await QueryOnContextAsync(async context =>
            {
                var query = context.BlogPosts
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .Where(p => p.IsPublished);

                if (categoryId > 0)
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }

                var records = await query
                    .Where(p => p.IsFeatured)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(count)
                    .ToArrayAsync();

                if (count > records.Length)
                {
                    var additionalRecords = await context.BlogPosts
                        .Where(p => !p.IsFeatured)
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(count - records.Length)
                        .ToArrayAsync();

                    records = [.. records, .. additionalRecords];
                }

                return records;
            });
        }

        public async Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0)
        {
            return await QueryOnContextAsync(async context =>
            {
                var query = context.BlogPosts
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .Where(p => p.IsPublished);

                if (categoryId > 0)
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }

                return await query
                    .OrderByDescending(p => p.ViewCount)
                    .Take(count)
                    .ToArrayAsync();
            });
        }

        public async Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0) => await GetPostsAsync(0, count, categoryId);

        public async Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId) => await GetPostsAsync(pageIndex * pageSize, pageSize, categoryId);

        public async Task<DetailPageModel> GetBlogPostBySlugAsync(string slug)
        {
            return await QueryOnContextAsync(async context =>
            {
                var blogPost = await context.BlogPosts
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

                if (blogPost is null)
                {
                    return DetailPageModel.Empty();
                }

                var relatedPosts = await context.BlogPosts
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .Where(p => p.CategoryId == blogPost.CategoryId && p.IsPublished)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(4)
                    .ToArrayAsync();

                return new DetailPageModel(blogPost, relatedPosts);
            });
        }

        private async Task<BlogPost[]> GetPostsAsync(int skip, int take, int categoryId = 0)
        {
            return await QueryOnContextAsync(async context =>
            {
                var query = context.BlogPosts
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .Where(p => p.IsPublished);

                if (categoryId > 0)
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }

                return await query
                    .OrderByDescending(p => p.PublishedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToArrayAsync();
            });
        }
    }
}
