using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/polls/{pollId}/[controller]")]
	[ApiController]
	[HasPermisson(Permissions.GetResults)]
	public class ResultsController(IResultService resultService) : ControllerBase
	{
		private readonly IResultService _resultService = resultService;

		[HttpGet("row-data")]
		public async Task<IActionResult> PollVotes([FromRoute] int pollId,CancellationToken cancellationToken)
		{
			var pollVotesResult = await _resultService.GetPollVotesAsync(pollId, cancellationToken);

			return pollVotesResult.IsSuccess
				? Ok(pollVotesResult.Value) 
				: pollVotesResult.ToProblem();
		}

		[HttpGet("votes-per-day")]
		public async Task<IActionResult> VotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken)
		{
			var pollVotesResult = await _resultService.GetVotesPerDayAsync(pollId, cancellationToken);

			return pollVotesResult.IsSuccess
				? Ok(pollVotesResult.Value)
				: pollVotesResult.ToProblem();
		}

		[HttpGet("votes-per-question")]		
		public async Task<IActionResult> VotesPerQuestion([FromRoute] int pollId, CancellationToken cancellationToken)
		{
			var pollVotesResult = await _resultService.GetVotesPerQuestionAsync(pollId, cancellationToken);

			return pollVotesResult.IsSuccess
				? Ok(pollVotesResult.Value)
				: pollVotesResult.ToProblem();
		}
	}
}
