namespace BlazorBlog.Application.Models
{
    using System.ComponentModel.DataAnnotations;

    public class BlogPostVm
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }

        [Required, MaxLength(500)]
        public string Introduction { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public bool IsFeatured { get; set; }

        public bool IsPublished { get; set; }

        public string Image { get; set; } = string.Empty;

        public string? CategoryName { get; set; }

        public string? CategorySlug { get; set; }

        public string? AuthorName { get; set; }

        public string? PublishedAtDisplay { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string Slug { get; set; } = string.Empty;

        public int WordCount { get; set; }

        public string ReadingTime { get; set; } = string.Empty;
    }
}
