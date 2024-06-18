using Microsoft.EntityFrameworkCore;

namespace SurveyBasketV3.Api.Persistence.EntitiesConfigrations
{
	public class VoteConfigrations : IEntityTypeConfiguration<Vote>
	{
		public void Configure(EntityTypeBuilder<Vote> builder)
		{
			builder.HasIndex(x => new {x.PollId,x.UserId}).IsUnique();
		}
	}
}
