namespace SurveyBasketV3.Api.Services
{
	public interface INotificationService
	{
		Task SendNewPollNotification(int? PollId = null);
	}
}
