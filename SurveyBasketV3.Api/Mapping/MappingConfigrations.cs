using Mapster;
using SurveyBasketV3.Api.Contracts.Polls;
using SurveyBasketV3.Api.Contracts.Questions;
using SurveyBasketV3.Api.Contracts.Users;

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

			config.NewConfig<RegisterRequest, ApplicationUser>()
				.Map(dest => dest.UserName, src => src.Email);

			config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
				.Map(dest => dest, src => src.user)
				.Map(dest => dest.Roles, src => src.roles);

			config.NewConfig<CreateUserRequest, ApplicationUser>()
				.Map(dest => dest.UserName, src => src.Email)
				.Map(dest => dest.EmailConfirmed, src => true);

			config.NewConfig<UpdateUserRequest, ApplicationUser>()
				.Map(dest => dest.UserName, src => src.Email)
				.Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper());
		}
	}
}
