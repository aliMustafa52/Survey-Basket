using System.ComponentModel.DataAnnotations;

namespace SurveyBasketV3.Api.Settings
{
	public class MailSettings
	{

		[Required]
		public string Mail { get; init; } = string.Empty;

		[Required]
		public string DisplayName { get; init; } = string.Empty;

		[Required]
		public string Password { get; init; } = string.Empty;

		[Required]
		public string Host { get; init; } = string.Empty;

		[Required]
		public int Port { get; init; }

	}
}
