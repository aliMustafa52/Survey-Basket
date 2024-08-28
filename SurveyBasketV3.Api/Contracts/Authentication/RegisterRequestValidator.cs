using SurveyBasketV3.Api.Abstractions.Consts;

namespace SurveyBasketV3.Api.Contracts.Authentication
{
	public class RegisterRequestValidator:AbstractValidator<RegisterRequest>
	{
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("password should be at least 8 char and should contains Lowercase, Uppercase, NonAlphanumeric, Length of 8 and one UniqueChars");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(3,100);

			RuleFor(x => x.LastName)
				.NotEmpty()
				.Length(3, 100);
		}
    }
}
