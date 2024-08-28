using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketV3.Api.Contracts.Users;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("me")]
	[ApiController]
	[Authorize]
	public class AccountController(IUserService userService) : ControllerBase
	{
		private readonly IUserService _userService = userService;

		[HttpGet("")]
		public async Task<IActionResult> GetProfile()
		{
			var result =await _userService.GetProfileAsync(User.GetUserId()!);

			return Ok(result.Value);
		}

		[HttpPut("info")]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
		{
			await _userService.UpdateProfileAsync(User.GetUserId()!, request);

			return NoContent();
		}

		[HttpPut("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

			return result.IsSuccess ? NoContent() : result.ToProblem();
		}
	}
}
