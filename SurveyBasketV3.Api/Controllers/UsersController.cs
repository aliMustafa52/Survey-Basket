using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketV3.Api.Contracts.Users;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController(IUserService userService) : ControllerBase
	{
		private readonly IUserService _userService = userService;

		[HttpGet("")]
		[HasPermisson(Permissions.GetUsers)]
		public async Task<IActionResult> GetAll()
		{
			var users = await _userService.GetAllAsync();
			return Ok(users);
		}

		[HttpGet("{id}")]
		[HasPermisson(Permissions.GetUsers)]
		public async Task<IActionResult> Get([FromRoute] string id)
		{
			var result = await _userService.GetAsync(id);
			return result.IsSuccess
				? Ok(result.Value)
				: result.ToProblem();
		}

		[HttpPost("")]
		[HasPermisson(Permissions.AddUsers)]
		public async Task<IActionResult> Add([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
		{
			var result = await _userService.AddAsync(request, cancellationToken);
			return result.IsSuccess
				? CreatedAtAction(nameof(Get) , new {id = result.Value.Id}, result.Value)
				: result.ToProblem();
		}

		[HttpPut("{id}")]
		[HasPermisson(Permissions.UpdateUsers)]
		public async Task<IActionResult> Update([FromRoute] string id,[FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
		{
			var result = await _userService.UpdateAsync(id,request, cancellationToken);
			return result.IsSuccess
				? NoContent()
				: result.ToProblem();
		}

		[HttpPut("{id}/toggle-status")]
		[HasPermisson(Permissions.UpdateUsers)]
		public async Task<IActionResult> ToggleStatus([FromRoute] string id)
		{
			var result = await _userService.ToggleStatusAsync(id);
			return result.IsSuccess
				? NoContent()
				: result.ToProblem();
		}

		[HttpPut("{id}/unlock")]
		[HasPermisson(Permissions.UpdateUsers)]
		public async Task<IActionResult> Unlock([FromRoute] string id)
		{
			var result = await _userService.UnlockAsync(id);
			return result.IsSuccess
				? NoContent()
				: result.ToProblem();
		}
	}
}
