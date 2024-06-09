﻿namespace SurveyBasketV3.Api.Authentication
{
	public interface IJwtProvider
	{
		(string token,int expiresIn) GenerateToken(ApplicationUser user);
	}
}
