namespace BlazorBlog.Infrastructure.Persistence.Repositories
{
    using BlazorBlog.Application.Models;
    using BlazorBlog.Application.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading;

    using BlogPostEntity = BlazorBlog.Infrastructure.Persistence.Entities.BlogPost;
    using DomainBlogPost = BlazorBlog.Domain.Entities.BlogPost;

    public class BlogPostRepository : IBlogPostRepository, IBlogPostAdminRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BlogPostRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private static BlogPostVm Map(BlogPostEntity p) => new()
        {
            Id = p.Id,
            Title = p.Title,
            Slug = p.Slug,
            Image = p.Image,
            Introduction = p.Introduction,
            Content = p.Content,
            CategoryId = p.CategoryId,
            IsFeatured = p.IsFeatured,
            IsPublished = p.IsPublished,
            PublishedAt = p.PublishedAt,
            PublishedAtDisplay = p.PublishedAt.HasValue ? p.PublishedAt.Value.ToString("dd-MMM-yyyy") : string.Empty,
            CategoryName = p.Category?.Name,
            CategorySlug = p.Category?.Slug,
            AuthorName = p.User?.Name
        };

        private static DomainBlogPost MapToDomain(BlogPostEntity e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Slug = e.Slug,
            Image = e.Image,
            Introduction = e.Introduction,
            Content = e.Content,
            CategoryId = e.CategoryId,
            UserId = e.UserId,
            IsPublished = e.IsPublished,
            ViewCount = e.ViewCount,
            IsFeatured = e.IsFeatured,
            IsDeleted = e.IsDeleted,
            CreatedAt = e.CreatedAt,
            PublishedAt = e.PublishedAt
        };

        private static void MapToEntity(DomainBlogPost src, BlogPostEntity dest)
        {
            dest.Title = src.Title;
            dest.Slug = src.Slug;
            dest.Image = src.Image;
            dest.Introduction = src.Introduction;
            dest.Content = src.Content;
            dest.CategoryId = src.CategoryId;
            dest.IsFeatured = src.IsFeatured;
            dest.IsPublished = src.IsPublished;
            dest.PublishedAt = src.PublishedAt;
        }

        public async Task<PageResult<DomainBlogPost>> GetBlogPostsAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            await using var context = _contextFactory.CreateDbContext();
            var query = context.BlogPosts.AsNoTracking();

            var count = await query.CountAsync(cancellationToken);
            var results = await query.Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .Skip(startIndex)
                .Take(pageSize)
                .ToArrayAsync(cancellationToken);

            return new PageResult<DomainBlogPost>(results.Select(MapToDomain).ToArray(), count);
        }

        public async Task<DomainBlogPost?> GetBlogPostByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var e = await context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            return e is null ? null : MapToDomain(e);
        }

        public async Task<DomainBlogPost> SaveBlogPostAsync(DomainBlogPost blogPost, string userId, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            BlogPostEntity entity;
            if (blogPost.Id == 0)
            {
                entity = new BlogPostEntity();
                entity.UserId = userId;
                MapToEntity(blogPost, entity);
                await context.BlogPosts.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity = await context.BlogPosts.FirstOrDefaultAsync(x => x.Id == blogPost.Id, cancellationToken)
                    ?? throw new InvalidOperationException($"Blog post not found.");

                MapToEntity(blogPost, entity);
            }

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("The blog post was modified by another user. Please refresh and try again.", ex);
            }

            return MapToDomain(entity);
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

        public async Task<BlogPostVm[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId, CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            IQueryable<BlogPostEntity> query = context.BlogPosts
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

            return posts.Select(Map).ToArray();
        }

        public async Task<BlogPostVm[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
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
            if (total == 0) return Array.Empty<BlogPostVm>();

            var take = Math.Min(count, total);
            var rnd = Random.Shared.Next(Math.Max(1, total - take + 1));

            var records = await baseQuery
                .OrderByDescending(p => p.PublishedAt)
                .Skip(rnd)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return records.Select(Map).ToArray();
        }

        public async Task<BlogPostVm[]> GetPopularBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
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

            var entities = await query
                .OrderByDescending(p => p.ViewCount)
                .Take(count)
                .ToArrayAsync(cancellationToken);

            return entities.Select(Map).ToArray();
        }

        public async Task<BlogPostVm[]> GetRecentBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default)
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

            var entities = await query
                .OrderByDescending(p => p.PublishedAt)
                .Take(count)
                .ToArrayAsync(cancellationToken);

            return entities.Select(Map).ToArray();
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

            var vm = Map(blogPost);
            var related = relatedPosts.Select(Map).ToArray();
            return new DetailPageModel(vm, related);
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
