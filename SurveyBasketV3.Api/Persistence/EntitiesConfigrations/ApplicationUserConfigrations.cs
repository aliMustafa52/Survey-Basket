namespace SurveyBasketV3.Api.Persistence.EntitiesConfigrations
{
	public class ApplicationUserConfigrations : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.Property(x => x.FirstName).HasMaxLength(100);
			builder.Property(x => x.LastName).HasMaxLength(100);
		}
	}
}
