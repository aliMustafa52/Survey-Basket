using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace SurveyBasketV3.Api.Persistence.EntitiesConfigrations
{
	public class PollConfigrations : IEntityTypeConfiguration<Poll>
	{
		public void Configure(EntityTypeBuilder<Poll> builder)
		{
			builder.Property(x => x.Title).HasMaxLength(100);
			builder.HasIndex(x => x.Title).IsUnique();

			builder.Property(x=> x.Summary).HasMaxLength(1000);

		}
	}
}
