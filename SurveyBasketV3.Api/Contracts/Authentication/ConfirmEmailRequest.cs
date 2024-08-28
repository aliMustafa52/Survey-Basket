namespace SurveyBasketV3.Api.Contracts.Authentication
{
	public record ConfirmEmailRequest(string UserId,string Code);
}
