using SurveyBasketV3.Api.Abstractions.Consts;

namespace SurveyBasketV3.Api.Contracts.Authentication
{
	public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
	{
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Code)
                .NotEmpty();

			RuleFor(x => x.NewPassword)
				.NotEmpty()
				.Matches(RegexPatterns.Password)
				.WithMessage("password should be at least 8 char and should contains Lowercase, Uppercase, NonAlphanumeric, Length of 8 and one UniqueChars");
		}
    }
}
