namespace SurveyBasketV3.Api.Errors
{
	public static class QuestionErrors
	{
		public static readonly Error QuestionNotFound =
			new ("Question.NotFound", "No Question was found with this Id", StatusCodes.Status404NotFound);

		public static readonly Error DublicatedQuestionContent
			= new("Question.DublicatedQuestionContent",
				"Another Question with the same Content exists for this poll", StatusCodes.Status409Conflict);
	}
}
