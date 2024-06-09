using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController(IAuthService authService,IConfiguration configuration) : ControllerBase
	{
		private readonly IAuthService _authService = authService;
		private readonly IConfiguration _configuration = configuration;

		[HttpPost("")]
		public async Task<IActionResult> Login(LoginRequest request,CancellationToken cancellationToken)
		{
			var authresponse = await _authService.GetTokenAsync(request.Email,request.Password, cancellationToken);

			return authresponse is null ? BadRequest("Invalid Email/Password") : Ok(authresponse);
        }

		[HttpGet("test")]
		public IActionResult Test()
		{
			var config = new
			{
				key=_configuration["Logging:LogLevel:Default"]
			};
			return Ok(config);
		}
	}
}
