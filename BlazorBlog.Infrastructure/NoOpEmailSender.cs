namespace BlazorBlog.Infrastructure
{
    using Microsoft.Extensions.Logging;

    public sealed class NoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly ILogger<NoOpEmailSender> _logger;

        public NoOpEmailSender(ILogger<NoOpEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            _logger.LogWarning("Email sender is not configured. Confirmation link for {Email} was not sent.", email);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            _logger.LogWarning("Email sender is not configured. Password reset link for {Email} was not sent.", email);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            _logger.LogWarning("Email sender is not configured. Password reset code for {Email} was not sent.", email);
            return Task.CompletedTask;
        }
    }
}
