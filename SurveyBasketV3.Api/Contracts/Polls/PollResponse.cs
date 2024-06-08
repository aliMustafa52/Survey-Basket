﻿using System.ComponentModel.DataAnnotations;

namespace SurveyBasketV3.Api.Contracts.Polls
{
	public record PollResponse
		(
			int Id,
			string Title,
			string Summary,
			bool IsPublished,
			DateOnly StartsAt,
			DateOnly EnndsAt
		);

}
