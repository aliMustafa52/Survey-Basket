using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketV3.Api.Authentication;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController(IAuthService authService) : ControllerBase
	{
		private readonly IAuthService _authService = authService;

		[HttpPost("")]
		public async Task<IActionResult> Login(LoginRequest request,CancellationToken cancellationToken)
		{
			var authresponse = await _authService.GetTokenAsync(request.Email,request.Password, cancellationToken);

			return authresponse is null ? BadRequest("Invalid Email/Password") : Ok(authresponse);
        }

	}
}
