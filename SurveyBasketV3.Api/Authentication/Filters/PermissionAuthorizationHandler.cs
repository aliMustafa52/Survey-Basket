namespace SurveyBasketV3.Api.Authentication.Filters
{
	public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
	{
		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
		{
			//var user = context.User.Identity;
			//if (user is null || !user.IsAuthenticated) 
			//	return;

			//var claims = context.User.Claims;

			//if (!claims.Any(x => x.Value == requirement.Permisson && x.Type == Permissions.Type))
			//	return;

			//better way to write the code above
			if (context.User.Identity is not { IsAuthenticated: true } ||
				 !context.User.Claims.Any(x => x.Value == requirement.Permisson && x.Type == Permissions.Type))
				return;

			context.Succeed(requirement);
			return;
		}
	}
}
