using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasketV3.Api.Authentication;
using SurveyBasketV3.Api.Contracts.Authentication;

namespace SurveyBasketV3.Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	[EnableRateLimiting(RateLimitPolicies.IpLimiter)]
	public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
	{
		private readonly IAuthService _authService = authService;
		private readonly ILogger<AuthController> _logger = logger;

		[HttpPost("")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken)
		{
			_logger.LogInformation("Logging with Emalil: {email} and password: {password}" , request.Email, request.Password);

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

		[HttpPost("register")]
		[DisableRateLimiting]
		public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
		{
			var authresponse = await _authService.RegisterAsync(registerRequest, cancellationToken);

			return authresponse.IsSuccess
				? Ok()
				: authresponse.ToProblem();
		}

		[HttpPost("confirm-email")]
		public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest registerRequest)
		{
			var authresponse = await _authService.ConfirmEmailAsync(registerRequest);

			return authresponse.IsSuccess
				? Ok()
				: authresponse.ToProblem();
		}

		[HttpPost("resend-confirmation-email")]
		public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmEmailRequest request)
		{
			var authresponse = await _authService.ResendConfirmEmailAsync(request);

			return authresponse.IsSuccess
				? Ok()
				: authresponse.ToProblem();
		}
		[HttpPost("forget-password")]
		public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
		{
			var authresponse = await _authService.SendCodeToRestPasswordAsync(request.Email);

			return authresponse.IsSuccess
				? Ok()
				: authresponse.ToProblem();
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
		{
			var authresponse = await _authService.ResetPasswordAsync(request);

			return authresponse.IsSuccess
				? Ok()
				: authresponse.ToProblem();
		}


	}
}
