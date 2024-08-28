﻿namespace SurveyBasketV3.Api.Services
{
	public interface IAuthService
	{
		Task<Result<AuthResponse>> GetTokenAsync(string email, string password,CancellationToken cancellationToken = default);

		Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
		Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

		Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
		Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);

		Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request);
		Task<Result> SendCodeToRestPasswordAsync(string email);
		Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
	}
}
