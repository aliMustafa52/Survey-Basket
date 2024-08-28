namespace SurveyBasketV3.Api.Errors
{
	public static class RoleErrors
	{
		public static readonly Error RoleNotFound =
			new("Role.NotFound", "No Role was found with this Id", StatusCodes.Status404NotFound);

		public static readonly Error DublicatedRole =
			new("Role.Dublicated", "This Role already exists", StatusCodes.Status409Conflict);

		public static readonly Error InvalidPermission =
			new("Role.InvalidPermission", "This Permission isn't allowed", StatusCodes.Status409Conflict);

		
	}
}
