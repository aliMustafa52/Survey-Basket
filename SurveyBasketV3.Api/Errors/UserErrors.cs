namespace SurveyBasketV3.Api.Errors
{
	public static class UserErrors
	{
		public static readonly Error InvalidCredentials = 
			new("User.InvalidCredentials", "Invalid Email/Password", StatusCodes.Status401Unauthorized);

		public static readonly Error InvalidJwtToken =
			new("User.InvalidJwtToken", "Invalid Jwt Token", StatusCodes.Status401Unauthorized);

		public static readonly Error InvalidRefreshToken = 
			new("User.InvalidRefreshToken", "Invalid Refresh Token", StatusCodes.Status401Unauthorized);
	}
}
