namespace SurveyBasketV3.Api.Errors
{
	public static class VoteErrors
	{
		public static readonly Error InvalidQuestions =
			new("Vote.InvalidQuestions", "Invalid Questions", StatusCodes.Status404NotFound);

		public static readonly Error DublicatedVote
			= new("Vote.DublicatedQuestionVote",
				"You cannot vote twice for the same poll", StatusCodes.Status409Conflict);
	}
}
