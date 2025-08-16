namespace BlazorBlog.Infrastructure.Persistence
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(25)]
    public string Name { get; set; } = string.Empty;
    }
}
