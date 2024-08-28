namespace SurveyBasketV3.Api.Authentication.Filters
{
	public class HasPermissonAttribute(string permission) : AuthorizeAttribute(permission)
	{
		public string Permission { get; } = permission;
	}
}
