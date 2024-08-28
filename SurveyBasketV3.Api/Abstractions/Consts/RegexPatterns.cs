namespace SurveyBasketV3.Api.Abstractions.Consts
{
	public static class RegexPatterns
	{
		public static readonly string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
	}
}
