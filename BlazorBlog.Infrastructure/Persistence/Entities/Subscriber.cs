namespace BlazorBlog.Infrastructure.Persistence.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Subscriber
    {
        public int Id { get; set; }

        [EmailAddress, Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

        [Required, MaxLength(25)]
    public string Name { get; set; } = string.Empty;

        public DateTime SubscribedOn { get; set; }
    }
}
