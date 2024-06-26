namespace SurveyBasketV3.Api.Contracts.Votes
{
	public record VoteRequest(
		IEnumerable<VoteAnswerRequest> Answers	
	);
}
