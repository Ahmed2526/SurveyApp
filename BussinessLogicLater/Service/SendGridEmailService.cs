using BussinessLogicLater.IService;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BussinessLogicLater.Service
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IConfiguration config, ILogger<SendGridEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<Result<bool>> SendEmailAsync(string toEmail, string subject, string message, CancellationToken cancellationToken)
        {
            var apiKey = _config["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(
                _config["SendGrid:FromEmail"],
                _config["SendGrid:FromName"]
            );

            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(msg, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("failed to send email to user {email}", toEmail);

                return Result<bool>.Fail(StatusCodes.Status503ServiceUnavailable, new[] { "Failed to send email. Please try again later." });
            }

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }
    }
}
