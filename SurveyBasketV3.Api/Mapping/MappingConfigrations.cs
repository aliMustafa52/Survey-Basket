using Mapster;
using SurveyBasketV3.Api.Contracts.Polls;

namespace SurveyBasketV3.Api.Mapping
{
	public class MappingConfigrations : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			//config.NewConfig<Poll, PollResponse>()
			//	.Map(dest => dest.Notes, src => src.Description);
		}
	}
}
