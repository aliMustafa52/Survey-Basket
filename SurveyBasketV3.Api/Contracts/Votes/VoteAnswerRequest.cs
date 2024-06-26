namespace SurveyBasketV3.Api.Contracts.Votes
{
	public record VoteAnswerRequest(
		int QuestionId,
		int AnswerId
	);
}
