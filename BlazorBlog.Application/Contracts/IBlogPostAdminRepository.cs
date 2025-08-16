namespace BlazorBlog.Application.Contracts
{
    using System.Threading;

    using BlazorBlog.Application.Models;
    using BlazorBlog.Domain.Entities;

    public interface IBlogPostAdminRepository
    {
        Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default);

        Task<BlogPost?> GetBlogPostByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId, CancellationToken cancellationToken = default);

        Task<bool> DeleteBlogPostAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> TitleExistsAsync(string title, int? excludeId = null, CancellationToken cancellationToken = default);

        Task<bool> SlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}
