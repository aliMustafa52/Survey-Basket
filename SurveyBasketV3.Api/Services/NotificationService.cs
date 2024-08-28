
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasketV3.Api.Helpers;
using SurveyBasketV3.Api.Persistence;

namespace SurveyBasketV3.Api.Services
{
	public class NotificationService(
		ApplicationDbContext context,
		UserManager<ApplicationUser> userManager,
		IEmailSender emailSender,
		IHttpContextAccessor httpContextAccessor) : INotificationService
	{
		private readonly ApplicationDbContext _context = context;
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly IEmailSender _emailSender = emailSender;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

		public async Task SendNewPollNotification(int? PollId = null)
		{
			IEnumerable<Poll> polls = [];
			if(PollId is not null)
			{
				var poll = await _context.Polls.SingleOrDefaultAsync(x => x.Id == PollId && x.IsPublished);
				polls = [poll!];
			}
			else
			{
				polls = await _context.Polls
						.Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
						.AsNoTracking()
						.ToListAsync();
			}
			var users = await _userManager.Users.ToListAsync();
			var orgin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
			foreach (var poll in polls)
			{
				foreach (var user in users)
				{
					var placeholders = new Dictionary<string, string>
					{
						{"{{name}}" ,$"{user.FirstName} {user.LastName}" },
						{"{{pollTill}}" ,poll.Title },
						{"{{endDate}}" ,poll.EnndsAt.ToString("MM-dd-yyyy") },
						{"{{url}}" ,$"{orgin}/polls/start/{poll.Id}" }
					};
					var emailBody = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholders);
					await _emailSender.SendEmailAsync(user.Email!, $"Survey Basket: New Poll {poll.Title} Drops", emailBody);
				}
			}


		}
	}
}
