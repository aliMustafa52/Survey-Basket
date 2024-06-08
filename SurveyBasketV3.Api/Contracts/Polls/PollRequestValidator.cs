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

			RuleFor(x => x.Summary)
				.NotEmpty()
				.Length(3, 1000);

			RuleFor(x => x.StartsAt)
				.NotEmpty()
				.GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

			RuleFor(x => x.EnndsAt)
				.NotEmpty();
			//.GreaterThanOrEqualTo(x => x.StartsAt);

			RuleFor(x => x)
				.Must(HasValidDates)
				.WithName(nameof(PollRequest.EnndsAt))
				.WithMessage("{PropertyName} must be greater than start date");
		}

		private bool HasValidDates(PollRequest request) => request.EnndsAt >= request.StartsAt;
    }
}
