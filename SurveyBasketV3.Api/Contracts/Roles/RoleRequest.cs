namespace SurveyBasketV3.Api.Contracts.Roles
{
	public record RoleRequest(string Name, IEnumerable<string> Permissions);
}
