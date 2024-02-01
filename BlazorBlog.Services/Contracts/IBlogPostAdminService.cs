using BlazorBlog.Data.Models;

namespace BlazorBlog.Services.Contracts
{
    public interface IBlogPostAdminService
    {
        Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize);

        Task<BlogPost?> GetBlogPostByIdAsync(int id);

        Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId);

        Task<bool> DeleteBlogPostAsync(int id);
    }
}
