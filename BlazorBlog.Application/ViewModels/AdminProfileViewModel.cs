namespace BlazorBlog.Application.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class AdminProfileViewModel
    {
  [Required(ErrorMessage = "Name is required")]
        [StringLength(25, ErrorMessage = "Name cannot exceed 25 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
      public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }
    }
}
