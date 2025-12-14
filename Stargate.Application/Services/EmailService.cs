using Microsoft.Extensions.Logging;
using Stargate.Application.Interfaces;

namespace Stargate.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
        {
            // For development, log the email instead of sending it
            var resetLink = $"http://localhost:4200/reset-password?token={resetToken}";

            var emailContent = $@"
================================================================================
PASSWORD RESET EMAIL
================================================================================
To: {toEmail}
Subject: Password Reset Request

Hello {userName},

You requested a password reset for your Stargate account.

Click the link below to reset your password:
{resetLink}

This link will expire in 1 hour.

If you did not request this password reset, please ignore this email.

================================================================================
";

            _logger.LogInformation(emailContent);

            await Task.CompletedTask;
        }
    }
}
