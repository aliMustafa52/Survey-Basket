
namespace SurveyBasketV3.Api.Persistence.EntitiesConfigrations
{
	public class UserRoleConfigrations : IEntityTypeConfiguration<IdentityUserRole<string>>
	{
		public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
		{
			//seed userRoles Table
			builder.HasData(new IdentityUserRole<string>
			{
				UserId = DefaultUsers.AdminId,
				RoleId = DefaultRoles.AdminRoleId
			});
		}
	}
}
