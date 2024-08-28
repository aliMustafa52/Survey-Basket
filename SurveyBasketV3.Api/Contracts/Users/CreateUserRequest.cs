﻿namespace SurveyBasketV3.Api.Contracts.Users
{
	public record CreateUserRequest
	(
		string FirstName,
		string LastName,
		string Email,
		string Password,
		IList<string> Roles
	);
}
