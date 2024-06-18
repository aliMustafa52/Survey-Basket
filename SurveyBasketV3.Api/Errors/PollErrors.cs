namespace SurveyBasketV3.Api.Errors
{
	public static class PollErrors
	{
		public static readonly Error PollNotFound =
			new("Poll.NotFount", "No Poll was found with this Id",StatusCodes.Status404NotFound);
		public static readonly Error DublicatedPollTitle
				= new("Poll.DublicatedPollTitle", "Another poll with the same title exists",StatusCodes.Status409Conflict);
	}
}
