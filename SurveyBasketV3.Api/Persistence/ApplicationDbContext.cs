﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SurveyBasketV3.Api.Persistence
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: IdentityDbContext<ApplicationUser>(options)
	{

		public DbSet<Poll> Polls {  get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			base.OnModelCreating(modelBuilder);
		}
	}
}
