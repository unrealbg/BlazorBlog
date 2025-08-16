namespace BlazorBlog.Tests.Fakes
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.Models;

    public class FakeBlogPostService : IBlogPostService
    {
        public Task<BlogPostVm[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default) => Task.FromResult(Array.Empty<BlogPostVm>());

        public Task<BlogPostVm[]> GetPopularBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default) => Task.FromResult(Array.Empty<BlogPostVm>());

        public Task<BlogPostVm[]> GetRecentBlogPostsAsync(int count, int categoryId = 0, CancellationToken cancellationToken = default) => Task.FromResult(Array.Empty<BlogPostVm>());

        public Task<BlogPostVm[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId, CancellationToken cancellationToken = default) => Task.FromResult(Array.Empty<BlogPostVm>());

        public Task<DetailPageModel> GetBlogPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
            => Task.FromResult(new DetailPageModel(new BlogPostVm(), Array.Empty<BlogPostVm>()));
    }
}
