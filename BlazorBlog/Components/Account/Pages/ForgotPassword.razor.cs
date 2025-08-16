using System.ComponentModel.DataAnnotations;
using BlazorBlog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;

namespace BlazorBlog.Components.Account.Pages
{
    public partial class ForgotPassword
    {
        private string? _statusMessage;
        private string? _devResetUrl;
        private string? _devToken;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; } = default!;

        [Inject]
        private IEmailSender<ApplicationUser> EmailSender { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IHostEnvironment HostEnvironment { get; set; } = default!;

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        private async Task SendResetAsync()
        {
            var user = await UserManager.FindByEmailAsync(Input.Email);
            if (user is null || !(await UserManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                _statusMessage = "If that email exists in our system, a reset link has been sent.";
                _devResetUrl = null;
                _devToken = null;
                return;
            }

            var token = await UserManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = Navigation.ToAbsoluteUri($"/Account/ResetPassword?email={Uri.EscapeDataString(Input.Email)}&code={Uri.EscapeDataString(token)}").ToString();

            await EmailSender.SendPasswordResetLinkAsync(user, Input.Email, resetUrl);

            // Expose the reset link/token only in Development for local testing
            if (HostEnvironment.IsDevelopment())
            {
                _devResetUrl = resetUrl;
                _devToken = token;
            }
            else
            {
                _devResetUrl = null;
                _devToken = null;
            }

            _statusMessage = "If that email exists in our system, a reset link has been sent.";
        }
    }
}
