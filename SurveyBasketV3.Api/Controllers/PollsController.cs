using Microsoft.AspNetCore.RateLimiting;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/[controller]")]  // /api/polls
	[ApiController]  // 1- Parameter Binding , 2-Validations
	
	public class PollsController(IPollService pollService) : ControllerBase
	{
		private readonly IPollService _pollService = pollService;

		[HttpGet("")]
		[HasPermisson(Permissions.GetPolls)]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var pollResponses = await _pollService.GetAllAsync(cancellationToken);

			return Ok(pollResponses);
		}

		[HttpGet("current")]
		[Authorize(Roles = DefaultRoles.Member)]
		[EnableRateLimiting(RateLimitPolicies.UserLimiter)]
		public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
		{
			var pollResponses = await _pollService.GetCurrentAsync(cancellationToken);

			return Ok(pollResponses);
		}

		[HttpGet("{id}")] // /api/polls/id
		[HasPermisson(Permissions.GetPolls)]
		public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
		{
			var pollResponseResult = await _pollService.GetAsync(id, cancellationToken);

			return pollResponseResult.IsSuccess
				? Ok(pollResponseResult.Value)
				: pollResponseResult.ToProblem();
		}
		
		[HttpPost("")]
		[HasPermisson(Permissions.AddPolls)]
		public async Task<IActionResult> Add([FromBody] PollRequest request,CancellationToken cancellationToken)
		{
			var pollResponseResult = await _pollService.AddAsync(request, cancellationToken);

			return pollResponseResult.IsSuccess
				? CreatedAtAction(nameof(Get), new { id = pollResponseResult.Value.Id }, pollResponseResult.Value)
				: pollResponseResult.ToProblem();
		}
		
		[HttpPut("{id}")]
		[HasPermisson(Permissions.UpdatePolls)]
		public async Task<IActionResult> Update([FromRoute] int id,[FromBody] PollRequest request, CancellationToken cancellationToken)
		{
			var result = await _pollService.UpdateAsync(id, request,cancellationToken);

			return result.IsSuccess 
				? NoContent() 
				: result.ToProblem();
		}
		
		[HttpDelete("{id}")]
		[HasPermisson(Permissions.DeletePolls)]
		public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _pollService.DeleteAsync(id,cancellationToken);

			return result.IsSuccess 
				? Ok() 
				: result.ToProblem();
		}

		[HttpPut("{id}/togglepublish")]
		[HasPermisson(Permissions.UpdatePolls)]
		public async Task<IActionResult> TogglePublish([FromRoute]int id,CancellationToken cancellationToken)
		{
			var result = await _pollService.TogglePublishAsync(id, cancellationToken);

			return result.IsSuccess
				? Ok()
				: result.ToProblem();
		}
	}
}
