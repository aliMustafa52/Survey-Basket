using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketV3.Api.Authentication;
using SurveyBasketV3.Api.Contracts.Authentication;

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
			//throw new InvalidOperationException();

			var authresponse = await _authService.GetTokenAsync(request.Email,request.Password, cancellationToken);

			return authresponse.IsSuccess
				? Ok(authresponse.Value)
				: authresponse.ToProblem();
        }

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
		{
			var authresponse = await _authService.GetRefreshTokenAsync(refreshTokenRequest.Token
						, refreshTokenRequest.RefreshToken, cancellationToken);

			return authresponse.IsSuccess
				? Ok(authresponse.Value)
				: authresponse.ToProblem();
		}

		[HttpPost("revoke-refresh-token")]
		public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
		{
			var result = await _authService.RevokeRefreshTokenAsync(refreshTokenRequest.Token
						, refreshTokenRequest.RefreshToken, cancellationToken);

			return result.IsSuccess
				? Ok()
				: result.ToProblem();
		}

	}
}
