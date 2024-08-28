namespace SurveyBasketV3.Api.Contracts.Results
{
	public record VotesPerDayResponse(
		DateOnly Date,
		int NumberOfVotes
	);
}
