namespace BlazorBlog.Services
{
    using Data.Models;
    using Repository.Contracts;

    public class BlogPostAdminService : IBlogPostAdminService
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogPostAdminService(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
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
            return await _blogPostRepository.SaveBlogPostAsync(blogPost, userId);
        }

        public async Task<bool> DeleteBlogPostAsync(int id)
        {
            return await _blogPostRepository.DeleteBlogPostAsync(id);
        }
    }
}
