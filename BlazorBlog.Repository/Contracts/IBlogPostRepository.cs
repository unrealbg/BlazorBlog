namespace BlazorBlog.Repository.Contracts
{
    using Data.Models;
    using Data.Entities;
    using System.Threading;

    public interface IBlogPostRepository
    {
        Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize, CancellationToken cancellationToken = default);

        Task<BlogPost?> GetBlogPostByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId, CancellationToken cancellationToken = default);

        Task<bool> DeleteBlogPostAsync(int id, CancellationToken cancellationToken = default);

        Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId, CancellationToken cancellationToken = default);

        Task<DetailPageModel> GetBlogPostBySlugAsync(string slug, CancellationToken cancellationToken = default);

        Task<bool> TitleExistsAsync(string title, int? excludeId = null, CancellationToken cancellationToken = default);

        Task<bool> SlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}
