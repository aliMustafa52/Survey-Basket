using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using SurveyBasketV3.Api.Abstractions;
using SurveyBasketV3.Api.Contracts.Answers;
using SurveyBasketV3.Api.Contracts.Common;
using SurveyBasketV3.Api.Contracts.Questions;
using SurveyBasketV3.Api.Entities;
using SurveyBasketV3.Api.Persistence;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;

namespace SurveyBasketV3.Api.Services
{
	public class QuestionService(ApplicationDbContext context, ICacheService cacheService) : IQuestionService
	{
		private readonly ApplicationDbContext _context = context;
		private readonly ICacheService _cacheService = cacheService;

		private const string _cachePrefix = "availableQuestions";
		public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters filters, CancellationToken cancellationToken = default)
		{
			var isExistingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
			if (!isExistingPoll)
				return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);

			var questionResponsesQuery = _context.Questions
							.Where(x => x.PollId == pollId && x.IsActive);

			if (!string.IsNullOrEmpty(filters.SearchValue))
			{
				questionResponsesQuery = questionResponsesQuery.Where(x => x.Content.Contains(filters.SearchValue));
			}
			if (!string.IsNullOrEmpty(filters.SortColumn))
			{
				questionResponsesQuery = questionResponsesQuery.OrderBy($"{filters.SortColumn} {filters.SortDirection}");
			}
			var source = questionResponsesQuery
							.Include(x => x.Answers)
							.ProjectToType<QuestionResponse>()
							.AsNoTracking();
			var response = await PaginatedList<QuestionResponse>
							.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

			return Result.Success(response);
		}

		public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
		{
			var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);
			if(hasVote)
				return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DublicatedVote);

			var isPollExists = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && 
								x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EnndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
			if(!isPollExists)
				Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

			var cacheKey = $"{_cachePrefix}-{pollId}";

			var cachedQuestions = await _cacheService.GetAsync<IEnumerable<QuestionResponse>>(cacheKey, cancellationToken);

			IEnumerable<QuestionResponse> questions = [];

			if (cachedQuestions is null)
            {
				questions = await _context.Questions
							.Where(x => x.PollId == pollId && x.IsActive)
							.Include(x => x.Answers)
							.Select(question =>
								new QuestionResponse(
									question.Id, question.Content, question.Answers
									.Where(answer => answer.IsActive)
									.Select(answer => new AnswerResponse(answer.Id, answer.Content))
								)
							)
							.AsNoTracking()
							.ToListAsync(cancellationToken);

				await _cacheService.SetAsync(cacheKey, questions, cancellationToken);
            }
			else
			{
				questions = cachedQuestions;
			}

			return Result.Success<IEnumerable<QuestionResponse>>(questions);
		}

		public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
		{
			var isExistingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
			if (!isExistingPoll)
				return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

			var questionResponse = await _context.Questions
							.Where(x => x.PollId == pollId && x.Id == questionId && x.IsActive)
							.Include(x => x.Answers)
							.AsNoTracking()
							.ProjectToType<QuestionResponse>()
							.SingleOrDefaultAsync(cancellationToken);

			if(questionResponse is null)
				return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

			return Result.Success(questionResponse);
		}

		public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
		{
			var isExistingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId,cancellationToken);
			if (!isExistingPoll) 
				return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

			var isExistingTitle = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == pollId,cancellationToken);
			if (isExistingTitle)
				return Result.Failure<QuestionResponse>(QuestionErrors.DublicatedQuestionContent);

			var question = request.Adapt<Question>();
			question.PollId = pollId;

			//request.Answers.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));
			//foreach(var answer in request.Answers)
			//	question.Answers.Add(new Answer { Content = answer });

			await _context.Questions.AddAsync(question,cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			await _cacheService.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

			return Result.Success(question.Adapt<QuestionResponse>());
		}


		public async Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default)
		{
			var isExistingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
			if (!isExistingPoll)
				return Result.Failure(PollErrors.PollNotFound);

			var isExistingTitle = await _context.Questions.AnyAsync(x => x.PollId == pollId && x.Id != questionId
						&& x.Content == request.Content, cancellationToken);
			if (isExistingTitle)
				return Result.Failure<QuestionResponse>(QuestionErrors.DublicatedQuestionContent);

			var question = await _context.Questions
								.Include(x => x.Answers)
								.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == questionId && x.IsActive, cancellationToken);
			if (question is null)
				return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);


			question.Content = request.Content;

			//add a new answer 
			//delte an answer
			//do nothing

			//current answers
			var currentAnswers =question.Answers.Select(x => x.Content).ToList();

			var newAnswers = request.Answers.Except(currentAnswers).ToList();
			// add new Asnwers
			newAnswers.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));

			//delete or do nothing
			 question.Answers.ToList().ForEach(answer => answer.IsActive = request.Answers.Contains(answer.Content));

			await _context.SaveChangesAsync(cancellationToken);

			await _cacheService.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

			return Result.Success();
        }

		public async Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
		{
			var isExistingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
			if (!isExistingPoll)
				return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

			var question = await _context.Questions
										.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == questionId, cancellationToken);

			if(question is null)
				return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

			question.IsActive = !question.IsActive;
			await _context.SaveChangesAsync(cancellationToken);

			await _cacheService.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

			return Result.Success();
		}
	}
}
