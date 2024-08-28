namespace SurveyBasketV3.Api.Contracts.Users
{
	public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
