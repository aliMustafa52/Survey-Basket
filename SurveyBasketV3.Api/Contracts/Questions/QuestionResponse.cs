using SurveyBasketV3.Api.Contracts.Answers;

namespace SurveyBasketV3.Api.Contracts.Questions
{
	public record QuestionResponse(int Id, string Content,IEnumerable<AnswerResponse> Answers);
}
