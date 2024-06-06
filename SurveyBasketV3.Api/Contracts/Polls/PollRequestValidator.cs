using FluentValidation;

namespace SurveyBasketV3.Api.Contracts.Polls
{
	public class PollRequestValidator : AbstractValidator<PollRequest>
	{
        public PollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(3,100);

			RuleFor(x => x.Description)
				.NotEmpty()
				.Length(3, 1000);
		}
    }
}
