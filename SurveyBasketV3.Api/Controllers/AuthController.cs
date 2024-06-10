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
		public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken)
		{
			var authresponse = await _authService.GetTokenAsync(request.Email,request.Password, cancellationToken);

			return authresponse is null ? BadRequest("Invalid Email/Password") : Ok(authresponse);
        }

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
		{
			var authresponse = await _authService.GetRefreshTokenAsync(refreshTokenRequest.Token
						, refreshTokenRequest.RefreshToken, cancellationToken);

			return authresponse is null ? BadRequest("Invalid Token") : Ok(authresponse);
		}

		[HttpPost("revoke-refresh-token")]
		public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
		{
			var isRevoked = await _authService.RevokeRefreshTokenAsync(refreshTokenRequest.Token
						, refreshTokenRequest.RefreshToken, cancellationToken);

			return isRevoked ? Ok() : BadRequest("Operation Failed");
		}

	}
}
