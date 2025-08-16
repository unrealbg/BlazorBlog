namespace BlazorBlog.Application.Contracts
{
    using System.Threading;

    using BlazorBlog.Application.Models;

    public interface IBlogPostService
    {
        Task<BlogPostVm[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPostVm[]> GetPopularBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPostVm[]> GetRecentBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default);

        Task<BlogPostVm[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId, CancellationToken cancellationToken = default);

        Task<DetailPageModel> GetBlogPostBySlugAsync(string slug, CancellationToken cancellationToken = default);
    }
}
