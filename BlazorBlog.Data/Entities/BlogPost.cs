namespace BlazorBlog.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class BlogPost
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(125)]
        public string Slug { get; set; }

        [Required, MaxLength(100)]
        public string Image { get; set; }

        [Required, MaxLength(500)]
        public string Introduction { get; set; }

        [Required]
        public string Content { get; set; }

        public short CategoryId { get; set; }

        public string UserId { get; set; }

        public bool IsPublished { get; set; }

        public int ViewCount { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsDeleted { get; set; }

        public Category Category { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }
    }
}