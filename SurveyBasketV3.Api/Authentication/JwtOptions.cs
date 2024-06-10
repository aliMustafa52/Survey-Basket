using System.ComponentModel.DataAnnotations;

namespace SurveyBasketV3.Api.Authentication
{
	public class JwtOptions
	{
		public static string SectionName { get; set; } = "Jwt";

		[Required]
		public string Key { get; init; } =string.Empty;

		[Required]
		public string Issuer { get; init; } = string.Empty;

		[Required]
		public string Audience { get; init; } = string.Empty;

		[Range(1,int.MaxValue,ErrorMessage = "Invalid Expiry Minutes")]
		public int ExpiryMinutes { get; init; }
	}
}
