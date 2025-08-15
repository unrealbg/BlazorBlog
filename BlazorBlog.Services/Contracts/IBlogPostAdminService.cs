namespace BlazorBlog.Services.Contracts
{
    using System.Threading;

    using BlazorBlog.Data.Models;

    public interface IBlogPostAdminService
    {
        Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default);

        Task<BlogPost?> GetBlogPostByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId, CancellationToken cancellationToken = default);

        Task<bool> DeleteBlogPostAsync(int id, CancellationToken cancellationToken = default);
    }
}
