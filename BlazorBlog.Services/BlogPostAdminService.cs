namespace BlazorBlog.Services
{
    public class BlogPostAdminService : IBlogPostAdminService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BlogPostAdminService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this._contextFactory = contextFactory;
        }

        public async Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize)
        {
            return await this.ExecuteOnContext(async context =>
            {
                var query = context.BlogPosts
                    .AsNoTracking();

                var count = await query
                    .CountAsync();

                var results = await query
                    .Include(b => b.Category)
                    .OrderByDescending(b => b.CreatedAt)
                    .Skip(startIndex)
                    .Take(pageSize)
                    .ToArrayAsync();

                return new PageResult<BlogPost>(results, count);
            });
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(int id) =>
            await this.ExecuteOnContext(async context =>
                await context.BlogPosts
                    .AsNoTracking()
                    .Include(b => b.Category)
                    .FirstOrDefaultAsync(b => b.Id == id));

        public async Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId)
        {
            return await ExecuteOnContext(async context =>
            {
                if (blogPost.Id == 0)
                {
                    var isTitleExist = await context.BlogPosts
                        .AsNoTracking()
                        .AnyAsync(b => b.Title == blogPost.Title);

                    if (isTitleExist)
                    {
                        throw new InvalidOperationException($"A blog post with this same title already exists.");
                    }

                    blogPost.Slug = await this.GenerateSlugAsync(blogPost);

                    blogPost.CreatedAt = DateTime.UtcNow;
                    blogPost.UserId = userId;

                    if (blogPost.IsPublished)
                    {
                        blogPost.PublishedAt = DateTime.UtcNow;
                    }

                    await context.BlogPosts.AddAsync(blogPost);
                }
                else
                {
                    var isTitleExist = await context.BlogPosts
                        .AsNoTracking()
                        .AnyAsync(b => b.Title == blogPost.Title && b.Id != blogPost.Id);

                    if (isTitleExist)
                    {
                        throw new InvalidOperationException($"A blog post with this same title already exists.");
                    }

                    var dbBlog = await context.BlogPosts.FindAsync(blogPost.Id);

                    dbBlog.Title = blogPost.Title;
                    dbBlog.Image = blogPost.Image;
                    dbBlog.Introduction = blogPost.Introduction;
                    dbBlog.Content = blogPost.Content;
                    dbBlog.CategoryId = blogPost.CategoryId;

                    dbBlog.IsPublished = blogPost.IsPublished;
                    dbBlog.IsFeatured = blogPost.IsFeatured;

                    if (blogPost.IsPublished)
                    {
                        if (!dbBlog.IsPublished)
                        {
                            blogPost.PublishedAt = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        blogPost.PublishedAt = null;
                    }
                }

                await context.SaveChangesAsync();
                return blogPost;
            });
        }

        public async Task<bool> DeleteBlogPostAsync(int id)
        {
            bool result = await this.ExecuteOnContext(async context =>
            {
                var blogPost = await context.BlogPosts.FindAsync(id);
                if (blogPost is null) return false;
                context.BlogPosts.Remove(blogPost);
                await context.SaveChangesAsync();
                return true;

            });

            return result;
        }

        private async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
        {
            await using var context = this._contextFactory.CreateDbContext();
            return await query.Invoke(context);
        }

        private async Task<string> GenerateSlugAsync(BlogPost blogPost)
        {
            return await this.ExecuteOnContext(async context =>
            {
                var currentSlug = blogPost.Title.ToSlug();
                var slug = currentSlug;
                var count = 1;

                while (await context.BlogPosts.AsNoTracking().AnyAsync(b => b.Slug == slug))
                {
                    slug = $"{currentSlug}-{count++}";
                }

                return slug;
            });
        }
    }
}
