using System.ComponentModel.DataAnnotations;

namespace BlazorBlog.Data.Models
{
    public class BlogPostDto
    {
        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }

        [Required, MaxLength(500)]
        public string Introduction { get; set; }

        public string Content { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsPublished { get; set; }

        public string UserId { get; set; }

        [MaxLength(100)]
        public string Image { get; set; }
    }
}
