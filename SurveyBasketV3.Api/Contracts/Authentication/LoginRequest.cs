namespace SurveyBasketV3.Api.Contracts.Authentication
{
	public record LoginRequest
	(
		string Email,
		string Password
	);
}
