using SurveyBasketV3.Api.Persistence;

namespace SurveyBasketV3.Api.Services
{
	public class PollService(ApplicationDbContext context) : IPollService
	{
		private readonly ApplicationDbContext _context = context;

		public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var polls = await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
			return polls.Adapt<IEnumerable<PollResponse>>();
		}
						

		
		public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			var poll = await _context.Polls.FindAsync(id, cancellationToken);

			return poll is null 
					? Result.Failure<PollResponse>(PollErrors.PollNotFound)
					: Result.Success(poll.Adapt<PollResponse>());
		}
		
		public async Task<PollResponse> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
		{
			var poll = pollRequest.Adapt<Poll>();

			await _context.Polls.AddAsync(poll,cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			return poll.Adapt<PollResponse>();
		}
		
		public async Task<Result> UpdateAsync(int id,PollRequest pollRequest, CancellationToken cancellationToken = default) 
		{
			var currentPoll = await _context.Polls.FindAsync(id,cancellationToken);

			if (currentPoll is null)
				return Result.Failure(PollErrors.PollNotFound);

			var poll = pollRequest.Adapt<Poll>();

			currentPoll.Title = poll.Title;
			currentPoll.Summary = poll.Summary;
			currentPoll.StartsAt = poll.StartsAt;
			currentPoll.EnndsAt = poll.EnndsAt;
			currentPoll.IsPublished = poll.IsPublished;

			await _context.SaveChangesAsync(cancellationToken);

			return Result.Success();
		}
		
		public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			var poll= await _context.Polls.FindAsync(id,cancellationToken);
			if (poll is null)
				return Result.Failure(PollErrors.PollNotFound);

			_context.Polls.Remove(poll);
			await _context.SaveChangesAsync(cancellationToken);

			return Result.Success();
		}
		public async Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default)
		{
			var poll = await _context.Polls.FindAsync(id, cancellationToken);
			if (poll is null)
				return Result.Failure(PollErrors.PollNotFound);

			poll.IsPublished = !poll.IsPublished;
			await _context.SaveChangesAsync(cancellationToken);

			return Result.Success();
		}


	}
}
