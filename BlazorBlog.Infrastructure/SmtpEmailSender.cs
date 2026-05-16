namespace BlazorBlog.Infrastructure
{
    using System.Net;
    using System.Net.Mail;

    using BlazorBlog.Infrastructure.Persistence;
    using BlazorBlog.Infrastructure.Settings;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    public sealed class SmtpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly EmailSettings _settings;

        public SmtpEmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            return SendAsync(email, "Confirm your email", $"Please confirm your email by opening this link: {EncodeLink(confirmationLink)}");
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            return SendAsync(email, "Reset your password", $"Reset your password by opening this link: {EncodeLink(resetLink)}");
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            return SendAsync(email, "Reset your password", $"Use this code to reset your password: <strong>{WebUtility.HtmlEncode(resetCode)}</strong>");
        }

        private async Task SendAsync(string email, string subject, string htmlBody)
        {
            if (!_settings.IsConfigured)
            {
                throw new InvalidOperationException("Email sender is not configured.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl
            };

            if (!string.IsNullOrWhiteSpace(_settings.UserName))
            {
                client.Credentials = new NetworkCredential(_settings.UserName, _settings.Password);
            }

            await client.SendMailAsync(message);
        }

        private static string EncodeLink(string link)
        {
            var encoded = WebUtility.HtmlEncode(link);
            return $"<a href=\"{encoded}\">{encoded}</a>";
        }
    }
}
