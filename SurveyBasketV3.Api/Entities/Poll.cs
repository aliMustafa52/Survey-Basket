namespace SurveyBasketV3.Api.Entities
{
	public class Poll
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime? DateOfBirth { get; set; }
	}
}
