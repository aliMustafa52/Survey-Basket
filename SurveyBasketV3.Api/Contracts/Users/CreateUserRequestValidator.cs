﻿namespace SurveyBasketV3.Api.Contracts.Users
{
	public class CreateUserRequestValidator :AbstractValidator<CreateUserRequest>
	{
        public CreateUserRequestValidator()
        {
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress();

			RuleFor(x => x.FirstName)
				.NotEmpty()
				.Length(3, 100);

			RuleFor(x => x.LastName)
				.NotEmpty()
				.Length(3, 100);

			RuleFor(x => x.Password)
				.NotEmpty()
				.Matches(RegexPatterns.Password)
				.WithMessage("password should be at least 8 char and should contains Lowercase, Uppercase, NonAlphanumeric, Length of 8 and one UniqueChars");

			RuleFor(x => x.Roles)
				.NotNull()
				.NotEmpty();

			RuleFor(x => x.Roles)
				.Must(x => x.Distinct().Count() == x.Count)
				.WithMessage("You cannot add dublicated role for the same user")
				.When(x => x.Roles.Any());
		}
    }
}
