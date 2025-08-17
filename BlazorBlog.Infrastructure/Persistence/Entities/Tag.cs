namespace BlazorBlog.Infrastructure.Persistence.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Tag
    {
        public int Id { get; set; }

        [Required, MaxLength(40)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Slug { get; set; } = string.Empty;

        public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
    }

    public class BlogPostTag
    {
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
