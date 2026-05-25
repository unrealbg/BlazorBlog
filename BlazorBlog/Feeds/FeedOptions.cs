namespace BlazorBlog.Feeds
{
    public sealed class FeedOptions
    {
        public const string SectionName = "Feed";

        public string Title { get; set; } = "Blazor Blog";

        public string Description { get; set; } = "Latest published posts from Blazor Blog.";

        public int ItemCount { get; set; } = 20;

        public int CacheMinutes { get; set; } = 10;
    }
}
