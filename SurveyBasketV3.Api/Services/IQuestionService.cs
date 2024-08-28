using SurveyBasketV3.Api.Contracts.Common;
using SurveyBasketV3.Api.Contracts.Questions;

namespace SurveyBasketV3.Api.Services
{
	public interface IQuestionService
	{
		Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters filters, CancellationToken cancellationToken = default);
		Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default);
		Task<Result<QuestionResponse>> GetAsync(int pollId,int questionId, CancellationToken cancellationToken = default);
		Task<Result<QuestionResponse>> AddAsync(int pollId,QuestionRequest request,CancellationToken cancellationToken = default);
		Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request,CancellationToken cancellationToken = default);
		
		Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
	}
}
