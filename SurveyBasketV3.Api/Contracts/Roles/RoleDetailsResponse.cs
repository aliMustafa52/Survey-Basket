namespace SurveyBasketV3.Api.Contracts.Roles
{
	public record RoleDetailsResponse(
		string Id,
		string Name,
		bool IsDeleted,
		IEnumerable<string> Permissions
	);
}
