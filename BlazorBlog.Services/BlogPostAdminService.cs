namespace BlazorBlog.Services
{
    using System.Threading;

    using BlazorBlog.Services.Utilities;

    using Data.Models;

    using Repository.Contracts;

    public class BlogPostAdminService : IBlogPostAdminService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ISlugService _slugService;
        private readonly IBlogCacheSignal _signal;

        public BlogPostAdminService(IBlogPostRepository blogPostRepository, ISlugService slugService, IBlogCacheSignal signal)
        {
            _blogPostRepository = blogPostRepository;
            _slugService = slugService;
            _signal = signal;
        }

        public async Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _blogPostRepository.GetBlogPostsAsync(startIndex, pageSize, cancellationToken);
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _blogPostRepository.GetBlogPostByIdAsync(id, cancellationToken);
        }

        public async Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId, CancellationToken cancellationToken = default)
        {
            var isNew = blogPost.Id == 0;

            if (await _blogPostRepository.TitleExistsAsync(blogPost.Title, excludeId: isNew ? null : blogPost.Id, cancellationToken))
            {
                throw new InvalidOperationException("A blog post with this same title already exists.");
            }

            var desiredSlug = string.IsNullOrWhiteSpace(blogPost.Slug)
                ? _slugService.GenerateSlug(blogPost.Title)
                : _slugService.GenerateSlug(blogPost.Slug);

            var uniqueSlug = desiredSlug;
            var suffix = 1;
            while (await _blogPostRepository.SlugExistsAsync(uniqueSlug, excludeId: isNew ? null : blogPost.Id, cancellationToken))
            {
                uniqueSlug = $"{desiredSlug}-{suffix++}";
            }

            blogPost.Slug = uniqueSlug;

            if (isNew)
            {
                blogPost.CreatedAt = DateTime.UtcNow;
                blogPost.PublishedAt = blogPost.IsPublished ? DateTime.UtcNow : null;
            }
            else
            {
                if (blogPost.IsPublished && blogPost.PublishedAt is null)
                {
                    blogPost.PublishedAt = DateTime.UtcNow;
                }
            }

            var saved = await _blogPostRepository.SaveBlogPostAsync(blogPost, userId, cancellationToken);
            _signal.Bump();
            return saved;
        }

        public async Task<bool> DeleteBlogPostAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _blogPostRepository.DeleteBlogPostAsync(id, cancellationToken);
            if (result)
            {
                _signal.Bump();
            }
            return result;
        }
    }
}
