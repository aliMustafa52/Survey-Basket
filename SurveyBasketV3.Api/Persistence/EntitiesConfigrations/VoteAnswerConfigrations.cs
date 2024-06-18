﻿using Microsoft.EntityFrameworkCore;

namespace SurveyBasketV3.Api.Persistence.EntitiesConfigrations
{
	public class VoteAnswerConfigrations : IEntityTypeConfiguration<VoteAnswer>
	{
		public void Configure(EntityTypeBuilder<VoteAnswer> builder)
		{
			builder.HasIndex(x => new { x.VoteId, x.QuestionId }).IsUnique();
		}
	}
}
