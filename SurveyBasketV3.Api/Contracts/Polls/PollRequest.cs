using SurveyBasketV3.Api.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SurveyBasketV3.Api.Contracts.Polls
{
	public record PollRequest
		(
			string Title,
			string Summary,
			bool IsPublished,
			DateOnly StartsAt,
			DateOnly EnndsAt
		);
}
