namespace SurveyBasketV3.Api.Errors
{
	public static class UserErrors
	{
		public static readonly Error InvalidCredentials = 
			new("User.InvalidCredentials", "Invalid Email/Password", StatusCodes.Status401Unauthorized);

		public static readonly Error InvalidJwtToken =
			new("User.InvalidJwtToken", "Invalid Jwt Token", StatusCodes.Status401Unauthorized);

		public static readonly Error DisabledUser =
			new("User.DisabledUser", "Disabled user, please contact your admin", StatusCodes.Status401Unauthorized);

		public static readonly Error LockedUser =
			new("User.LockedUser", "your account is locked, please try again later", StatusCodes.Status401Unauthorized);

		public static readonly Error InvalidRefreshToken = 
			new("User.InvalidRefreshToken", "Invalid Refresh Token", StatusCodes.Status401Unauthorized);

		public static readonly Error InvalidConfirmationCode =
			new("User.InvalidConfirmationCode", "Invalid Confirmation Code", StatusCodes.Status401Unauthorized);

		public static readonly Error EmailAlreadyConfirmed =
			new("User.EmailAlreadyConfirmed", "Email Already Confirmed", StatusCodes.Status400BadRequest);

		public static readonly Error EmailNotConfirmed =
			new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

		public static readonly Error ExistedEmail =
			new("User.ExistedEmail", "This email already exists try to log in", StatusCodes.Status409Conflict);

		public static readonly Error EmailNotFound =
			new("User.EmailNotFound", "Email is Not Found", StatusCodes.Status409Conflict);

		public static readonly Error PasswordNotCorrect =
			new("User.PasswordNotCorrect", "current password is wrong", StatusCodes.Status400BadRequest);

		public static readonly Error UserNotFound =
			new("User.NotFound", "No User was found with this Id", StatusCodes.Status404NotFound);

		public static readonly Error InvalidRoles =
			new("User.InvalidRoles", "This Roles are not allowed", StatusCodes.Status409Conflict);
	}
}
