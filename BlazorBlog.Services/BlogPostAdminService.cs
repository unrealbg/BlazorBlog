namespace BlazorBlog.Services
{
    using Data.Models;
    using Repository.Contracts;

    public class BlogPostAdminService : IBlogPostAdminService
    {
        private readonly IBlogPostAdminRepository _blogPostAdminRepository;

        public BlogPostAdminService(IBlogPostAdminRepository blogPostAdminRepository)
        {
            _blogPostAdminRepository = blogPostAdminRepository;
        }

        public async Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize)
        {
            return await _blogPostAdminRepository.GetBlogPostsAsync(startIndex, pageSize);
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
        {
            return await _blogPostAdminRepository.GetBlogPostByIdAsync(id);
        }

        public async Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId)
        {
            return await _blogPostAdminRepository.SaveBlogPostAsync(blogPost, userId);
        }

        public async Task<bool> DeleteBlogPostAsync(int id)
        {
            return await _blogPostAdminRepository.DeleteBlogPostAsync(id);
        }
    }
}
