namespace SurveyBasketV3.Api.Contracts.Results
{
	public record PollVotesResponse(
		string Title,
		IEnumerable<VoteResponse> Votes
	);
}
