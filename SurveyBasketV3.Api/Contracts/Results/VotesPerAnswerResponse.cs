namespace SurveyBasketV3.Api.Contracts.Results
{
	public record VotesPerAnswerResponse(
		string Answer,
		int Count
	);
}
