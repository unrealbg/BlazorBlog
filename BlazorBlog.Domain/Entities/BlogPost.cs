namespace BlazorBlog.Domain.Entities
{
    public class BlogPost
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string Introduction { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        public int ViewCount { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}
