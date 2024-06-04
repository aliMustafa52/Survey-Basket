namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/[controller]")]  // /api/polls
	[ApiController]
	public class PollsController : ControllerBase
	{
		// collection expression
		private readonly List<Poll> _polls =
			[
				new(){
					Id = 1,
					Title = "Test",
					Description = "Test",
				}
			]; 

		[HttpGet("")]
		//[Route("")]// /api/polls
		public IActionResult GetAll()
		{
			return Ok(_polls);
		}

		[HttpGet("{id}")] // /api/polls/id
		public IActionResult Get(int id)
		{
			var poll = _polls.SingleOrDefault(x => x.Id == id);

			return poll is null ? NotFound() : Ok(poll);
		}
	}
}
