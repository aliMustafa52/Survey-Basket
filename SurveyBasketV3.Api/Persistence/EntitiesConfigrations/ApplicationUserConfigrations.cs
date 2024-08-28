namespace SurveyBasketV3.Api.Persistence.EntitiesConfigrations
{
	public class ApplicationUserConfigrations : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.Property(x => x.FirstName).HasMaxLength(100);
			builder.Property(x => x.LastName).HasMaxLength(100);

			builder
				.OwnsMany(x => x.RefreshTokens)
				.ToTable("RefreshTokens")
				.WithOwner()
				.HasForeignKey("UserId");

			//SEED DEFAULT DATA (must seed id)
			var passwordHasher = new PasswordHasher<ApplicationUser>();
			builder.HasData(new ApplicationUser
			{
				Id = DefaultUsers.AdminId,
				Email = DefaultUsers.AdminEmail,
				NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
				UserName = DefaultUsers.AdminEmail,
				NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
				FirstName = "Survey Basket",
				LastName = "Admin",
				SecurityStamp = DefaultUsers.AdminSecurityStamp,
				ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
				EmailConfirmed = true,
				PasswordHash = passwordHasher.HashPassword(null!,DefaultUsers.AdminPassword)
			});
		}
	}
}
