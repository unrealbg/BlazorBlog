using System.ComponentModel.DataAnnotations;
using BlazorBlog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace BlazorBlog.Components.Account.Pages
{
    public partial class ResetPassword
    {
        private string? _statusMessage;

        [SupplyParameterFromQuery] public string? Email { get; set; }
        [SupplyParameterFromQuery] public string? Code { get; set; }

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; } = default!;

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrEmpty(Email)) Input.Email = Email;
            if (!string.IsNullOrEmpty(Code)) Input.Code = Code;
        }

        private sealed class InputModel
        {
            [Required] public string Email { get; set; } = string.Empty;
            [Required] public string Code { get; set; } = string.Empty;
            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
            [Compare(nameof(Password))]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        private async Task ResetAsync()
        {
            var user = await UserManager.FindByEmailAsync(Input.Email);
            if (user is null)
            {
                _statusMessage = "The reset link is invalid or has expired.";
                return;
            }

            var result = await UserManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                _statusMessage = "Password has been reset. You can now log in.";
            }
            else
            {
                _statusMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            }
        }
    }
}
