using System.Security.Claims;

namespace SurveyBasketV3.Api.Extensions
{
	public static class UserExtensions
	{
		public static string? GetUserId(this ClaimsPrincipal principal)
		{
			return principal.FindFirstValue(ClaimTypes.NameIdentifier);
		}
	}
}
