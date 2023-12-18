namespace BlazorBlog.Services.Contracts
{
    public interface IBlogPostAdminService
    {
        Task<PageResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize);

        Task<BlogPost?> GetPostByIdAsync(int id);

        Task<BlogPost> SaveBlogPostAsync(BlogPost blogPost, string userId);
    }
}
