﻿namespace SurveyBasketV3.Api.Contracts.Authentication
{
	public record RefreshTokenRequest(
		string Token,
		string RefreshToken
	);
}
