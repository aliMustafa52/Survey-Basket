
namespace SurveyBasketV3.Api.Persistence.EntitiesConfigrations
{
	public class ApplicationRoleConfigrations : IEntityTypeConfiguration<ApplicationRole>
	{
		public void Configure(EntityTypeBuilder<ApplicationRole> builder)
		{
			//seed default roles
			builder.HasData(
				[
					new ApplicationRole{
						Id = DefaultRoles.AdminRoleId,
						Name = DefaultRoles.Admin,
						NormalizedName = DefaultRoles.Admin.ToUpper(),
						ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
					},
					new ApplicationRole{
						Id = DefaultRoles.MemberRoleId,
						Name = DefaultRoles.Member,
						NormalizedName = DefaultRoles.Member.ToUpper(),
						ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
						IsDefault = true,
					},
				]
			);
		}
	}
}
