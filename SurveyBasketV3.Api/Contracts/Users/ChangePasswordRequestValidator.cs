using SurveyBasketV3.Api.Abstractions.Consts;

namespace SurveyBasketV3.Api.Contracts.Users
{
	public class ChangePasswordRequestValidator :AbstractValidator<ChangePasswordRequest>
	{
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
				.WithMessage("password should be at least 8 char and should contains Lowercase, Uppercase, NonAlphanumeric, Length of 8 and one UniqueChars");

			RuleFor(x => x.NewPassword)
				.NotEmpty()
				.Matches(RegexPatterns.Password)
				.NotEqual(x => x.CurrentPassword)
				.WithMessage("New Password can't be the same as Current Password");
		}
    }
}
