using Mapster;
using SurveyBasketV3.Api.Contracts.Questions;
using SurveyBasketV3.Api.Contracts.Votes;
using SurveyBasketV3.Api.Persistence;

namespace SurveyBasketV3.Api.Services
{
	public class VoteService(ApplicationDbContext context) : IVoteService
	{
		private readonly ApplicationDbContext _context = context;

		public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
		{
			var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);
			if (hasVote) 
				return Result.Failure(VoteErrors.DublicatedVote);

			var isPollExists = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished 
				&& x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EnndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

			if (!isPollExists)
				Result.Failure(PollErrors.PollNotFound);

			var availableQuestions = await _context.Questions
						.Where(x => x.PollId == pollId && x.IsActive)
						.Select(x => x.Id)
						.ToListAsync(cancellationToken);

			if(!availableQuestions.SequenceEqual(request.Answers.Select(x => x.QuestionId)))
				Result.Failure(VoteErrors.InvalidQuestions);

			Vote vote = new()
			{
				PollId = pollId,
				UserId= userId,
				VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList(),
			};

			await _context.Votes.AddAsync(vote,cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			return Result.Success();
		}
	}
}
