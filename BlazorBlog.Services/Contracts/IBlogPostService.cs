namespace BlazorBlog.Services.Contracts
{
    using System.Threading;

    using BlazorBlog.Data.Entities;
    using BlazorBlog.Data.Models;

    public interface IBlogPostService
    {
        Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId, CancellationToken cancellationToken = default);

        Task<DetailPageModel> GetBlogPostBySlugAsync(string slug, CancellationToken cancellationToken = default);
    }
}