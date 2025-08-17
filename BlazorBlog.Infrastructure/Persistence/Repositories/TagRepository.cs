namespace BlazorBlog.Infrastructure.Persistence.Repositories
{
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Infrastructure.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;

    public class TagRepository : ITagRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ISlugService _slugService;

        public TagRepository(IDbContextFactory<ApplicationDbContext> contextFactory, ISlugService slugService)
        {
            _contextFactory = contextFactory;
            _slugService = slugService;
        }

        public async Task<BlazorBlog.Application.Models.TagVm[]> GetTopTagsAsync(int count, CancellationToken cancellationToken = default)
        {
            await using var ctx = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var data = await ctx.BlogPostTags
                .AsNoTracking()
                .GroupBy(bpt => new { bpt.TagId, bpt.Tag.Name, bpt.Tag.Slug })
                .Select(g => new { g.Key.Name, g.Key.Slug, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Name)
                .Take(count)
                .ToArrayAsync(cancellationToken);

            return data.Select(x => new BlazorBlog.Application.Models.TagVm
            {
                Name = x.Name,
                Slug = x.Slug,
                Count = x.Count
            }).ToArray();
        }

        public async Task<string[]> GetTagsForPostAsync(int postId, CancellationToken cancellationToken = default)
        {
            await using var ctx = await _contextFactory.CreateDbContextAsync(cancellationToken);
            return await ctx.BlogPostTags
                .AsNoTracking()
                .Where(x => x.BlogPostId == postId)
                .Select(x => x.Tag.Name)
                .ToArrayAsync(cancellationToken);
        }

        public async Task SetTagsForPostAsync(int postId, string[] tags, CancellationToken cancellationToken = default)
        {
            await using var ctx = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var normalized = tags
                .Select(t => t?.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))!
                .Select(t => t!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var existing = await ctx.BlogPostTags.Where(x => x.BlogPostId == postId).ToListAsync(cancellationToken);
            if (existing.Count > 0)
            {
                ctx.BlogPostTags.RemoveRange(existing);
            }

            if (normalized.Length == 0)
            {
                await ctx.SaveChangesAsync(cancellationToken);
                return;
            }

            var existingTags = await ctx.Tags
                .Where(t => normalized.Contains(t.Name))
                .ToListAsync(cancellationToken);

            var missingNames = normalized
                .Where(n => !existingTags.Any(et => et.Name.Equals(n, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            foreach (var name in missingNames)
            {
                var tag = new Tag { Name = name, Slug = _slugService.GenerateSlug(name) };
                ctx.Tags.Add(tag);
                existingTags.Add(tag);
            }

            await ctx.SaveChangesAsync(cancellationToken);

            foreach (var tag in existingTags)
            {
                ctx.BlogPostTags.Add(new BlogPostTag { BlogPostId = postId, TagId = tag.Id });
            }

            await ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
