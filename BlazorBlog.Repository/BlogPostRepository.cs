namespace BlazorBlog.Repository
{
    using Data.Entities;
    using Data.Models;
    using Data;
    using Contracts;
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Threading.Tasks;
    using System.Threading;

    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BlogPostRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            await using var context = _contextFactory.CreateDbContext();
            var query = context.BlogPosts.AsNoTracking();

            var count = await query.CountAsync(cancellationToken);
            var results = await query.Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .Skip(startIndex)
                .Take(pageSize)
                .ToArrayAsync(cancellationToken);

            return new PageResult<BlogPost>(results, count);
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            if (blogPost.Id == 0)
            {
                blogPost.UserId = userId;
                await context.BlogPosts.AddAsync(blogPost, cancellationToken);
            }
            else
            {
                var existingBlogPost = await context.BlogPosts.FirstOrDefaultAsync(x => x.Id == blogPost.Id, cancellationToken);
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
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("The blog post was modified by another user. Please refresh and try again.", ex);
            }

            return blogPost;
        }

        public async Task<bool> DeleteBlogPostAsync(int id, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var blogPost = await context.BlogPosts.FindAsync([id], cancellationToken);
            if (blogPost == null) return false;

            context.BlogPosts.Remove(blogPost);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
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
                .ToArrayAsync(cancellationToken);

            return posts;
        }

        public async Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var baseQuery = context.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Where(p => p.IsPublished && p.IsFeatured);

            if (categoryId > 0)
            {
                baseQuery = baseQuery.Where(p => p.CategoryId == categoryId);
            }

            var total = await baseQuery.CountAsync(cancellationToken);
            if (total == 0) return Array.Empty<BlogPost>();

            var take = Math.Min(count, total);
            var rnd = Random.Shared.Next(Math.Max(1, total - take + 1));

            var records = await baseQuery
                .OrderByDescending(p => p.PublishedAt)
                .Skip(rnd)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return records;
        }

        public async Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
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
                .ToArrayAsync(cancellationToken);
        }

        public async Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
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
                .ToArrayAsync(cancellationToken);
        }

        public async Task<DetailPageModel> GetBlogPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            await using var context = _contextFactory.CreateDbContext();
            var blogPost = await context.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished, cancellationToken);

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
                .ToArrayAsync(cancellationToken);

            return new DetailPageModel(blogPost, relatedPosts);
        }

        public async Task<bool> TitleExistsAsync(string title, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.BlogPosts.AsNoTracking().Where(b => b.Title == title);
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var query = context.BlogPosts.AsNoTracking().Where(b => b.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }
            return await query.AnyAsync(cancellationToken);
        }
    }
}
