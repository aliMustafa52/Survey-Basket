namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/[controller]")]  // /api/polls
	[ApiController]
	public class PollsController(IPollService pollService) : ControllerBase
	{
		private readonly IPollService _pollService = pollService;

		[HttpGet("")]
		//[Route("")]// /api/polls
		public IActionResult GetAll()
		{
			return Ok(_pollService.GetAll());
		}

		[HttpGet("{id}")] // /api/polls/id
		public IActionResult Get(int id)
		{
			var poll = _pollService.Get(id);

			return poll is null ? NotFound() : Ok(poll);
		}

		[HttpPost("")]
		public IActionResult Add(Poll poll)
		{
			var newPoll= _pollService.Add(poll);

			return CreatedAtAction(nameof(Get), new {id =newPoll.Id}, newPoll);
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id,Poll poll)
		{
			var isUpdated = _pollService.Update(id, poll);

			return isUpdated ? NoContent() : NotFound();
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var isDeleted = _pollService.Delete(id);
			return isDeleted ? Ok() : NotFound();
		}
	}
}
