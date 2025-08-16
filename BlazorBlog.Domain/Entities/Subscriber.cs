namespace BlazorBlog.Domain.Entities
{
    public class Subscriber
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DateTime SubscribedOn { get; set; }
    }
}
