using Azure.Core;
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
			var pollResponses = await _pollService.GetAllAsync(cancellationToken);

			return Ok(pollResponses);
		}

		[HttpGet("{id}")] // /api/polls/id
		public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
		{
			var pollResponseResult = await _pollService.GetAsync(id, cancellationToken);

			return pollResponseResult.IsSuccess
				? Ok(pollResponseResult.Value)
				: pollResponseResult.ToProblem(StatusCodes.Status404NotFound);
		}
		
		[HttpPost("")]
		public async Task<IActionResult> Add([FromBody] PollRequest request,CancellationToken cancellationToken)
		{
			var newPoll= await _pollService.AddAsync(request, cancellationToken);

			return CreatedAtAction(nameof(Get), new {id =newPoll.Id}, newPoll.Adapt<PollResponse>());
		}
		
		[HttpPut("{id}")]
		public async Task<IActionResult> Update([FromRoute] int id,[FromBody] PollRequest request, CancellationToken cancellationToken)
		{
			var result = await _pollService.UpdateAsync(id, request,cancellationToken);

			return result.IsSuccess 
				? NoContent() 
				: result.ToProblem(StatusCodes.Status404NotFound);
		}
		
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _pollService.DeleteAsync(id,cancellationToken);

			return result.IsSuccess 
				? Ok() 
				: result.ToProblem(StatusCodes.Status404NotFound);
		}

		[HttpPut("{id}/togglepublish")]
		public async Task<IActionResult> TogglePublish([FromRoute]int id,CancellationToken cancellationToken)
		{
			var result = await _pollService.TogglePublishAsync(id, cancellationToken);

			return result.IsSuccess
				? Ok()
				: result.ToProblem(StatusCodes.Status404NotFound);
		}
	}
}
