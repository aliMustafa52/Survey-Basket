using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasketV3.Api.Contracts.Polls;
using SurveyBasketV3.Api.Mapping;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/[controller]")]  // /api/polls
	[ApiController]  // 1- Parameter Binding , 2-Validations
	public class PollsController(IPollService pollService) : ControllerBase
	{
		private readonly IPollService _pollService = pollService;

		[HttpGet("")]
		//[Route("")]// /api/polls
		public IActionResult GetAll()
		{
			var polls = _pollService.GetAll();
			var pollResponses = polls.Adapt<IEnumerable<PollResponse>>();
			return Ok(pollResponses);
		}

		[HttpGet("{id}")] // /api/polls/id
		public IActionResult Get([FromRoute] int id)
		{
			var poll = _pollService.Get(id);
			if (poll is null) 
				return NotFound();


			//var config = new TypeAdapterConfig();
			//config.NewConfig<Poll, PollResponse>()
			//	.Map(dest => dest.Notes, src => src.Description);

			var pollResponse = poll.Adapt<PollResponse>();



			return Ok(pollResponse);
		}

		[HttpPost("")]
		public IActionResult Add([FromBody] PollRequest request)
		{

			var newPoll= _pollService.Add(request.Adapt<Poll>());

			return CreatedAtAction(nameof(Get), new {id =newPoll.Id}, newPoll.Adapt<PollResponse>());
		}

		[HttpPut("{id}")]
		public IActionResult Update([FromRoute] int id,[FromBody] PollRequest request)
		{
			var isUpdated = _pollService.Update(id, request.Adapt<Poll>());

			return isUpdated ? NoContent() : NotFound();
		}

		[HttpDelete("{id}")]
		public IActionResult Delete([FromRoute] int id)
		{
			var isDeleted = _pollService.Delete(id);
			return isDeleted ? Ok() : NotFound();
		}
	}
}
