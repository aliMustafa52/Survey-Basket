namespace SurveyBasketV3.Api.Services
{
	public interface IPollService
	{
		Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default);

		Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);

		Task<PollResponse> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default);

		Task<Result> UpdateAsync(int id, PollRequest pollRequest, CancellationToken cancellationToken = default);

		Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
		Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default);
	}
}
