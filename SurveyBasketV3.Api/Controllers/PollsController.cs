using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasketV3.Api.Contracts.Polls;
using SurveyBasketV3.Api.Mapping;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/[controller]")]  // /api/polls
	[ApiController]  // 1- Parameter Binding , 2-Validations
	[Authorize]
	public class PollsController(IPollService pollService) : ControllerBase
	{
		private readonly IPollService _pollService = pollService;

		[HttpGet("")]
		//[Route("")]// /api/polls
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var polls = await _pollService.GetAllAsync(cancellationToken);
			var pollResponses = polls.Adapt<IEnumerable<PollResponse>>();
			return Ok(pollResponses);
		}

		[HttpGet("{id}")] // /api/polls/id
		public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
		{
			var poll = await _pollService.GetAsync(id, cancellationToken);
			if (poll is null) 
				return NotFound();


			//var config = new TypeAdapterConfig();
			//config.NewConfig<Poll, PollResponse>()
			//	.Map(dest => dest.Notes, src => src.Description);

			var pollResponse = poll.Adapt<PollResponse>();



			return Ok(pollResponse);
		}
		
		[HttpPost("")]
		public async Task<IActionResult> Add([FromBody] PollRequest request,CancellationToken cancellationToken)
		{
			var pollRequest = request.Adapt<Poll>();
			var newPoll= await _pollService.AddAsync(pollRequest, cancellationToken);

			return CreatedAtAction(nameof(Get), new {id =newPoll.Id}, newPoll.Adapt<PollResponse>());
		}
		
		[HttpPut("{id}")]
		public async Task<IActionResult> Update([FromRoute] int id,[FromBody] PollRequest request, CancellationToken cancellationToken)
		{
			var isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(),cancellationToken);

			return isUpdated ? NoContent() : NotFound();
		}
		
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
		{
			var isDeleted = await _pollService.DeleteAsync(id,cancellationToken);
			return isDeleted ? Ok() : NotFound();
		}

		[HttpPut("{id}/togglepublish")]
		public async Task<IActionResult> TogglePublish([FromRoute]int id,CancellationToken cancellationToken)
		{
			var isToggled = await _pollService.TogglePublishAsync(id, cancellationToken);
			return isToggled ? NoContent() : NotFound();
		}
	}
}
