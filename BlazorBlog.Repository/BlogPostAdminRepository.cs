namespace BlazorBlog.Repository
{
    using Data.Entities;
    using Data.Models;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class BlogPostAdminRepository : IBlogPostAdminRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BlogPostAdminRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.BlogPosts.AsNoTracking();

            var count = await query.CountAsync();
            var results = await query.Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .Skip(startIndex)
                .Take(pageSize)
                .ToListAsync(); 

            return new PageResult<BlogPost>(results.ToArray(), count);
        }


        public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            if (blogPost.Id == 0)
            {
                blogPost.CreatedAt = DateTime.UtcNow;
                blogPost.UserId = userId; 

                await context.BlogPosts.AddAsync(blogPost);
            }
            else
            {
                var existingBlogPost = await context.BlogPosts.FindAsync(blogPost.Id);
                if (existingBlogPost == null) throw new InvalidOperationException("Blog post not found.");

                existingBlogPost.Title = blogPost.Title;
                existingBlogPost.Content = blogPost.Content;

                context.BlogPosts.Update(existingBlogPost);
            }

            await context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<bool> DeleteBlogPostAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var blogPost = await context.BlogPosts.FindAsync(id);
            if (blogPost == null) return false;

            context.BlogPosts.Remove(blogPost);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
