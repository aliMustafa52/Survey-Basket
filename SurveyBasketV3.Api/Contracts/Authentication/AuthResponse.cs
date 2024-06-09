﻿namespace SurveyBasketV3.Api.Contracts.Authentication
{
	public record AuthResponse
		(
			string Id,
			string? Email,
			string FirstName,
			string LastName,
			string Token,
			int Expiresin
		);
}
