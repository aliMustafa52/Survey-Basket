using Mapster;
using SurveyBasketV3.Api.Contracts.Polls;
using SurveyBasketV3.Api.Contracts.Questions;

namespace SurveyBasketV3.Api.Mapping
{
	public class MappingConfigrations : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			//config.NewConfig<QuestionRequest,Question>()
			//	.Ignore(nameof(Question.Answers));
			config.NewConfig<QuestionRequest, Question>()
				.Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));


		}
	}
}
