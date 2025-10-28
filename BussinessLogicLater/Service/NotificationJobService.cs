using BussinessLogicLater.IService;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BussinessLogicLater.Service
{
    public class NotificationJobService : INotificationJobService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public NotificationJobService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }
        //to do
        //continue
        //Handle toggle poll status 
        //why use record and not classes
        //continue working on Permission based auth
        //fix permission type value bug
        //handle statistics End Points
        //cors policy
        //Get users and their roles using joins 
        public async Task<bool> SendPollsDailyUpdateAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var activePolls = await _context.Polls
                .Where(p => p.IsPublished && p.StartsAt <= today && p.EndsAt >= today)
                .ToListAsync();

            if (!activePolls.Any())
                return false; // No active polls today

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                await SendPollUpdateEmailAsync(user, activePolls, CancellationToken.None);
            }

            return true;
        }


        public async Task<Result<bool>> SendPollUpdateEmailAsync(ApplicationUser user, List<Poll> activePolls, CancellationToken cancellationToken)
        {
            try
            {
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var templatePath = Path.Combine(webRootPath, "EmailTemplates", "DailyPollsUpdate.html");

                var htmlBody = await File.ReadAllTextAsync(templatePath, cancellationToken);

                // Build the polls list as HTML
                var pollsHtml = new StringBuilder();

                foreach (var poll in activePolls)
                {
                    var pollUrl = $"https://yourapp.com/polls/{poll.Id}";

                    pollsHtml.Append($@"
                <div style='margin-bottom: 20px;'>
                    <h3 style='color: #333;'>{poll.Title}</h3>
                    <p>{poll.Summary}</p>
                    <p><strong>Active:</strong> {poll.StartsAt:yyyy-MM-dd} → {poll.EndsAt:yyyy-MM-dd}</p>
                    <a href='{pollUrl}' 
                       style='background-color:#007bff; color:white; padding:8px 15px; text-decoration:none; border-radius:5px;'>
                       View Poll
                    </a>
                </div>
                <hr style='border: 0; border-top: 1px solid #ccc;'/>
            ");
                }

                // Replace placeholders
                htmlBody = htmlBody
                    .Replace("{{UserName}}", user.UserName ?? "User")
                    .Replace("{{PollsList}}", pollsHtml.ToString());

                // Send the email
                var emailResponse = await _emailService.SendEmailAsync(
                    user.Email!,
                    "Today's Active Polls",
                    htmlBody,
                    cancellationToken
                );

                if (!emailResponse.IsSuccess)
                    return Result<bool>.Fail(emailResponse.StatusCode, emailResponse.Errors!);

                return Result<bool>.Success(emailResponse.StatusCode, emailResponse.Data);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(StatusCodes.Status500InternalServerError, new[] { ex.Message });
            }
        }

    }
}
