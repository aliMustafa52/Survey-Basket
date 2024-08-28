using Hangfire;
using SurveyBasketV3.Api.Persistence;

namespace SurveyBasketV3.Api.Services
{
	public class PollService(ApplicationDbContext context, INotificationService notificationService) : IPollService
	{
		private readonly ApplicationDbContext _context = context;
		private readonly INotificationService _notificationService = notificationService;

		public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var pollResponses = await _context.Polls
				.AsNoTracking()
				.ProjectToType<PollResponse>()
				.ToListAsync(cancellationToken);

			return pollResponses;
		}
		public async Task<IEnumerable<PollResponse>> GetCurrentAsync( CancellationToken cancellationToken = default)
		{
			var pollResponses = await _context.Polls
				.Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EnndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
				.AsNoTracking()
				.ProjectToType<PollResponse>()
				.ToListAsync(cancellationToken);

			return pollResponses;
		}



		public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			var poll = await _context.Polls.FindAsync(id, cancellationToken);

			return poll is null 
					? Result.Failure<PollResponse>(PollErrors.PollNotFound)
					: Result.Success(poll.Adapt<PollResponse>());
		}
		
		public async Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
		{
			var isExistingTitle = _context.Polls.Any(x => x.Title == pollRequest.Title);
			if (isExistingTitle)
				return Result.Failure<PollResponse>(PollErrors.DublicatedPollTitle);

			var poll = pollRequest.Adapt<Poll>();

			await _context.Polls.AddAsync(poll,cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			return Result.Success(poll.Adapt<PollResponse>());
		}
		
		public async Task<Result> UpdateAsync(int id,PollRequest pollRequest, CancellationToken cancellationToken = default) 
		{
			var currentPoll = await _context.Polls.FindAsync(id,cancellationToken);
			if (currentPoll is null)
				return Result.Failure(PollErrors.PollNotFound);

			var isExistingTitle = _context.Polls.Any(x => x.Title == pollRequest.Title && x.Id != id);
			if (isExistingTitle)
				return Result.Failure<PollResponse>(PollErrors.DublicatedPollTitle);

			currentPoll.Title = pollRequest.Title;
			currentPoll.Summary = pollRequest.Summary;
			currentPoll.StartsAt = pollRequest.StartsAt;
			currentPoll.EnndsAt = pollRequest.EnndsAt;

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

			if(poll.IsPublished && poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
				BackgroundJob.Enqueue(() => _notificationService.SendNewPollNotification(poll.Id));

			return Result.Success();
		}


	}
}
