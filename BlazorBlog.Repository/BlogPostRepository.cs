namespace BlazorBlog.Repository
{
    using Data.Entities;
    using Data.Models;
    using Data;
    using Contracts;
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Threading.Tasks;

    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BlogPostRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize)
        {
            await using var context = _contextFactory.CreateDbContext();
            var query = context.BlogPosts.AsNoTracking();

            var count = await query.CountAsync();
            var results = await query.Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .Skip(startIndex)
                .Take(pageSize)
                .ToArrayAsync();

            return new PageResult<BlogPost>(results, count);
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
                blogPost.UserId = userId;
                await context.BlogPosts.AddAsync(blogPost);
            }
            else
            {
                var existingBlogPost = await context.BlogPosts.FirstOrDefaultAsync(x => x.Id == blogPost.Id);
                if (existingBlogPost == null)
                {
                    throw new InvalidOperationException($"Blog post not found.");
                }

                if (blogPost.RowVersion != null)
                {
                    context.Entry(existingBlogPost).Property(p => p.RowVersion).OriginalValue = blogPost.RowVersion;
                }

                existingBlogPost.Title = blogPost.Title;
                existingBlogPost.Image = blogPost.Image;
                existingBlogPost.Introduction = blogPost.Introduction;
                existingBlogPost.Content = blogPost.Content;
                existingBlogPost.CategoryId = blogPost.CategoryId;
                existingBlogPost.IsFeatured = blogPost.IsFeatured;
                existingBlogPost.IsPublished = blogPost.IsPublished;
                existingBlogPost.Slug = blogPost.Slug;
                existingBlogPost.PublishedAt = blogPost.PublishedAt;
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("The blog post was modified by another user. Please refresh and try again.", ex);
            }

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

        public async Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            IQueryable<BlogPost> query = context.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Where(p => p.IsPublished);

            if (categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            var posts = await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return posts;
        }

        public async Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Where(p => p.IsPublished && p.IsFeatured);

            if (categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            var records = await query
                .OrderByDescending(p => p.PublishedAt)
                .Take(count)
                .ToArrayAsync();

            return records;
        }

        public async Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0)
        {
            await using var context = _contextFactory.CreateDbContext();
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
        }

        public async Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0)
        {
            await using var context = _contextFactory.CreateDbContext();
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
                .Take(count)
                .ToArrayAsync();
        }

        public async Task<DetailPageModel> GetBlogPostBySlugAsync(string slug)
        {
            await using var context = _contextFactory.CreateDbContext();
            var blogPost = await context.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

            if (blogPost == null)
            {
                return DetailPageModel.Empty();
            }

            var relatedPosts = await context.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Where(p => p.CategoryId == blogPost.CategoryId && p.Id != blogPost.Id && p.IsPublished)
                .OrderByDescending(p => p.PublishedAt)
                .Take(4)
                .ToArrayAsync();

            return new DetailPageModel(blogPost, relatedPosts);
        }

        public async Task<bool> TitleExistsAsync(string title, int? excludeId = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.BlogPosts.AsNoTracking().Where(b => b.Title == title);
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.BlogPosts.AsNoTracking().Where(b => b.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
