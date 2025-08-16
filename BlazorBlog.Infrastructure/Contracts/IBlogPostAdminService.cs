namespace BlazorBlog.Infrastructure.Contracts
{
    using System.Threading;

    using BlazorBlog.Application.Models;
    using BlazorBlog.Domain.Entities;

    public interface IBlogPostAdminService
    {
        Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default);

        Task<BlogPost?> GetBlogPostByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId, CancellationToken cancellationToken = default);

        Task<bool> DeleteBlogPostAsync(int id, CancellationToken cancellationToken = default);
    }
}
