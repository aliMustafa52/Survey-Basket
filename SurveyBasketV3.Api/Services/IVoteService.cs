using SurveyBasketV3.Api.Contracts.Votes;

namespace SurveyBasketV3.Api.Services
{
	public interface IVoteService
	{
		Task<Result> AddAsync(int pollId,string userId,VoteRequest request, CancellationToken cancellationToken = default);
	}
}
