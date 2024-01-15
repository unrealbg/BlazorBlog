﻿namespace BlazorBlog.Services.Contracts
{
    public interface IBlogPostService
    {
        Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0);

        Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0);

        Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0);

        Task<DetailPageModel> GetBlogPostBySlugAsync(string slug);
    }
}