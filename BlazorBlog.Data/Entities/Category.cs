namespace BlazorBlog.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(75)]
        public string Slug { get; set; }

        public bool ShowOnNavBar { get; set; }
    }
}
