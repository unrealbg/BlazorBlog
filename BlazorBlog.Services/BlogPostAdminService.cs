namespace BlazorBlog.Services
{
    using Data.Models;
    using Repository.Contracts;

    public class BlogPostAdminService : IBlogPostAdminService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ISlugService _slugService;

        public BlogPostAdminService(IBlogPostRepository blogPostRepository, ISlugService slugService)
        {
            _blogPostRepository = blogPostRepository;
            _slugService = slugService;
        }

        public async Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize)
        {
            return await _blogPostRepository.GetBlogPostsAsync(startIndex, pageSize);
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
        {
            return await _blogPostRepository.GetBlogPostByIdAsync(id);
        }

        public async Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId)
        {
            // Business rules: validate uniqueness, generate slug, set timestamps
            var isNew = blogPost.Id == 0;

            // Title uniqueness
            if (await _blogPostRepository.TitleExistsAsync(blogPost.Title, excludeId: isNew ? null : blogPost.Id))
            {
                throw new InvalidOperationException("A blog post with this same title already exists.");
            }

            // Slug generation and uniqueness
            var desiredSlug = string.IsNullOrWhiteSpace(blogPost.Slug)
                ? _slugService.GenerateSlug(blogPost.Title)
                : _slugService.GenerateSlug(blogPost.Slug);

            var uniqueSlug = desiredSlug;
            var suffix = 1;
            while (await _blogPostRepository.SlugExistsAsync(uniqueSlug, excludeId: isNew ? null : blogPost.Id))
            {
                uniqueSlug = $"{desiredSlug}-{suffix++}";
            }
            blogPost.Slug = uniqueSlug;

            // Timestamps
            if (isNew)
            {
                blogPost.CreatedAt = DateTime.UtcNow;
                blogPost.PublishedAt = blogPost.IsPublished ? DateTime.UtcNow : null;
            }
            else
            {
                // If transitioning to published and wasn't previously, set PublishedAt now.
                if (blogPost.IsPublished && blogPost.PublishedAt is null)
                {
                    blogPost.PublishedAt = DateTime.UtcNow;
                }
            }

            return await _blogPostRepository.SaveBlogPostAsync(blogPost, userId);
        }

        public async Task<bool> DeleteBlogPostAsync(int id)
        {
            return await _blogPostRepository.DeleteBlogPostAsync(id);
        }
    }
}
