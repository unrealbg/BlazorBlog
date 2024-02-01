namespace BlazorBlog.Services
{
    using Data.Entities;
    using Contracts;
    using Data.Models;
    using global::BlazorBlog.Repository.Contracts;

    namespace BlazorBlog.Services
    {
        public class BlogPostService : IBlogPostService
        {
            private readonly IBlogPostRepository _blogPostRepository;

            public BlogPostService(IBlogPostRepository blogPostRepository)
            {
                _blogPostRepository = blogPostRepository;
            }

            public async Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0)
            {
                return await _blogPostRepository.GetFeaturedBlogPostsAsync(count, categoryId);
            }

            public async Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0)
            {
                return await _blogPostRepository.GetPopularBlogPostsAsync(count, categoryId);
            }

            public async Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0)
            {
                return await _blogPostRepository.GetRecentBlogPostsAsync(count, categoryId);
            }

            public async Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId)
            {
                return await _blogPostRepository.GetBlogPostsAsync(pageIndex, pageSize, categoryId);
            }

            public async Task<DetailPageModel> GetBlogPostBySlugAsync(string slug)
            {
                return await _blogPostRepository.GetBlogPostBySlugAsync(slug);
            }
        }
    }

}
