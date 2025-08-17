namespace BlazorBlog.Application.Models
{
    public class TagVm
    {
        public required string Name { get; set; }

        public required string Slug { get; set; }

        public int Count { get; set; }
    }
}
