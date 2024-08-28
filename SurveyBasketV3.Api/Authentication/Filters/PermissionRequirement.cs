namespace SurveyBasketV3.Api.Authentication.Filters
{
	public class PermissionRequirement(string permisson) : IAuthorizationRequirement
	{
		public string Permisson { get; } = permisson;
	}
}
