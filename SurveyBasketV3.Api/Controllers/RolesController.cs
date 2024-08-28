using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketV3.Api.Contracts.Roles;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RolesController(IRoleService roleService) : ControllerBase
	{
		private readonly IRoleService _roleService = roleService;

		[HttpGet("")]
		[HasPermisson(Permissions.GetRoles)]
		public async Task<IActionResult> GetAll([FromQuery] bool includeDisable, CancellationToken cancellationToken)
		{ 
			return Ok(await _roleService.GetAllAsync(includeDisable, cancellationToken));
		}

		[HttpGet("{id}")]
		[HasPermisson(Permissions.GetRoles)]
		public async Task<IActionResult> Get([FromRoute] string id)
		{
			var result = await _roleService.GetAsync(id);

			return result.IsSuccess
				? Ok(result.Value)
				: result.ToProblem();
		}

		[HttpPost("")]
		[HasPermisson(Permissions.AddRoles)]
		public async Task<IActionResult> Add([FromBody] RoleRequest request)
		{
			var result = await _roleService.AddAsync(request);

			return result.IsSuccess
				? CreatedAtAction(nameof(Get), new {id = result.Value.Id}, result.Value)
				: result.ToProblem();
		}

		[HttpPut("{id}")]
		[HasPermisson(Permissions.UpdateRoles)]
		public async Task<IActionResult> Update([FromRoute] string id, [FromBody] RoleRequest request)
		{
			var result = await _roleService.UpdateAsync(id, request);

			return result.IsSuccess
				? NoContent()
				: result.ToProblem();
		}

		[HttpPut("{id}/toggle-status")]
		[HasPermisson(Permissions.UpdateRoles)]
		public async Task<IActionResult> Toggle([FromRoute] string id)
		{
			var result = await _roleService.ToggleStatusAsync(id);

			return result.IsSuccess
				? NoContent()
				: result.ToProblem();
		}
	}
}
