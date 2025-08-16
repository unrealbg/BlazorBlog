namespace BlazorBlog.Application.Models
{
    public sealed class DetailPageModel
    {
        public BlogPostVm BlogPost { get; }
        public BlogPostVm[] RelatedPosts { get; }
        public bool IsEmpty => BlogPost.Id == 0;

        public DetailPageModel(BlogPostVm blogPost, BlogPostVm[] relatedPosts)
        {
            BlogPost = blogPost;
            RelatedPosts = relatedPosts;
        }

        public static DetailPageModel Empty() => new(new BlogPostVm(), Array.Empty<BlogPostVm>());
    }
}
