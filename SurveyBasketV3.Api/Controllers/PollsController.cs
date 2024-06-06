using SurveyBasketV3.Api.Contracts.Polls;
using SurveyBasketV3.Api.Mapping;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/[controller]")]  // /api/polls
	[ApiController]  // 1- Parameter Binding
	public class PollsController(IPollService pollService) : ControllerBase
	{
		private readonly IPollService _pollService = pollService;

		[HttpGet("")]
		//[Route("")]// /api/polls
		public IActionResult GetAll()
		{
			var polls = _pollService.GetAll();
			return Ok(polls.MapToPollResponse());
		}

		[HttpGet("{id}")] // /api/polls/id
		public IActionResult Get([FromRoute] int id)
		{
			var poll = _pollService.Get(id);

			return poll is null ? NotFound() : Ok(poll.MapToPollResponse());
		}

		[HttpPost("")]
		public IActionResult Add([FromBody] PollRequest request)
		{
			var newPoll= _pollService.Add(request.MapToPoll());

			return CreatedAtAction(nameof(Get), new {id =newPoll.Id}, newPoll.MapToPollResponse());
		}

		[HttpPut("{id}")]
		public IActionResult Update([FromRoute] int id,[FromBody] Poll poll)
		{
			var isUpdated = _pollService.Update(id, poll);

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
