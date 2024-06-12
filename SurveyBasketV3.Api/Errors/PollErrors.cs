namespace SurveyBasketV3.Api.Errors
{
	public static class PollErrors
	{
		public static readonly Error PollNotFound = new("Poll.NotFount", "No Poll was found with this Id");
	}
}
