using SurveyBasketV3.Api.Contracts.Common;
using SurveyBasketV3.Api.Contracts.Questions;
using SurveyBasketV3.Api.Entities;
using System.Threading;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/polls/{pollId}/[controller]")]
	[ApiController]
	public class QuestionsController(IQuestionService questionService) : ControllerBase
	{
		private readonly IQuestionService _questionService = questionService;

		[HttpGet("")]
		[HasPermisson(Permissions.GetQuestions)]
		public async Task<IActionResult> Get([FromRoute]int pollId,[FromQuery] RequestFilters filters, CancellationToken cancellationToken)
		{
			var result = await _questionService.GetAllAsync(pollId, filters, cancellationToken);

			return result.IsSuccess
				? Ok(result.Value)
				: result.ToProblem();
		}
		[HttpGet("{id}")]
		[HasPermisson(Permissions.GetQuestions)]
		public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _questionService.GetAsync(pollId,id, cancellationToken);

			return result.IsSuccess
				? Ok(result.Value)
				: result.ToProblem();
		}

		[HttpPost("")]
		[HasPermisson(Permissions.AddQuestions)]
		public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] QuestionRequest request,CancellationToken cancellationToken)
		{
			var result =await _questionService.AddAsync(pollId, request,cancellationToken);

			return result.IsSuccess
				? CreatedAtAction(nameof(Get),new { pollId, result.Value.Id},result.Value)
				: result.ToProblem();
		}

		[HttpPut("{id}")]
		[HasPermisson(Permissions.UpdateQuestions)]
		public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
		{
			var result = await _questionService.UpdateAsync(pollId, id,request, cancellationToken);

			return result.IsSuccess
				? NoContent()
				: result.ToProblem();
		}

		[HttpPut("{id}/toggleStatus")]
		[HasPermisson(Permissions.UpdateQuestions)]
		public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);

			return result.IsSuccess
				? NoContent()
				: result.ToProblem();
		}
	}
}
