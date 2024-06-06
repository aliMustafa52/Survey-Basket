using SurveyBasketV3.Api.Contracts.Polls;

namespace SurveyBasketV3.Api.Mapping
{
	public static class ContractMapping
	{
		public static Poll MapToPoll(this PollRequest request)
		{
			return new()
			{
				Title = request.Title,
				Description = request.Description,
			};
		}

		public static PollResponse MapToPollResponse(this Poll poll)
		{
			return new()
			{
				Id = poll.Id,
				Title = poll.Title,
				Description = poll.Description,
			};
		}

		public static IEnumerable<PollResponse> MapToPollResponse(this IEnumerable<Poll> polls) 
		{
			return polls.Select(MapToPollResponse);
			//return polls.Select(x => x.MapToPollResponse());
		}
	}
}
