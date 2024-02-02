namespace BlazorBlog.Repository.Contracts
{
    using Data.Entities;
    using Data.Models;
    using System.Threading.Tasks;

    public interface IBlogPostAdminRepository
    {
        Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize);

        Task<BlogPost?> GetBlogPostByIdAsync(int id);

        Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId);

        Task<bool> DeleteBlogPostAsync(int id);
    }
}
