﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace SurveyBasketV3.Api.Persistence
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IHttpContextAccessor httpContextAccessor)
		: IdentityDbContext<ApplicationUser>(options)
	{
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

		public DbSet<Poll> Polls {  get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Answer> Answers { get; set; }
		public DbSet<Vote> Votes { get; set; }
		public DbSet<VoteAnswer> VoteAnswers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			var cascadeFKs = modelBuilder.Model
								.GetEntityTypes()
								.SelectMany(t => t.GetForeignKeys())
								.Where(FK => FK.DeleteBehavior ==DeleteBehavior.Cascade && !FK.IsOwnership);

			foreach (var fk in cascadeFKs)
				fk.DeleteBehavior = DeleteBehavior.Restrict;

			base.OnModelCreating(modelBuilder);
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			// tracked entities + entities that inherit AuditableEntity
			var entries = ChangeTracker.Entries<AuditableEntity>(); 
			foreach (var entityEntry in entries)
			{
				var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()!;

				if (entityEntry.State == EntityState.Added)
				{
					entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId;
				}
				else if(entityEntry.State == EntityState.Modified)
				{
					entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
					entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
				}
			}
			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
