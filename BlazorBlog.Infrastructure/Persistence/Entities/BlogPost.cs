namespace BlazorBlog.Infrastructure.Persistence.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class BlogPost
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

        [MaxLength(125)]
    public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "The image is required.")]
        [MaxLength(100)]
    public string Image { get; set; } = string.Empty;

        [Required, MaxLength(500)]
    public string Introduction { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }

    public string UserId { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        public int ViewCount { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsDeleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
