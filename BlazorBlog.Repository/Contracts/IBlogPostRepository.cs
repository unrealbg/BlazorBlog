﻿namespace BlazorBlog.Repository.Contracts
{
    using Data.Models;
    using Data.Entities;

    public interface IBlogPostRepository
    {
        Task<BlogPost[]> GetFeaturedBlogPostsAsync(int count, int categoryId = 0);

        Task<BlogPost[]> GetPopularBlogPostsAsync(int count, int categoryId = 0);

        Task<BlogPost[]> GetRecentBlogPostsAsync(int count, int categoryId = 0);

        Task<BlogPost[]> GetBlogPostsAsync(int pageIndex, int pageSize, int categoryId);

        Task<DetailPageModel> GetBlogPostBySlugAsync(string slug);
    }
}
