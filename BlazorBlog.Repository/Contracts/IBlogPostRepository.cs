namespace BlazorBlog.Repository.Contracts
{
    using Data.Models;
    using Data.Entities;

    public interface IBlogPostRepository
    {
        Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize);

        Task<BlogPost?> GetBlogPostByIdAsync(int id);

        Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId);

        Task<bool> DeleteBlogPostAsync(int id);

        Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0);

        Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0);

        Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0);

        Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId);

        Task<DetailPageModel> GetBlogPostBySlugAsync(string slug);
    }
}
