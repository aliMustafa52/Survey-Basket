using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SurveyBasketV3.Api.Persistence;

namespace SurveyBasketV3.Api
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
		{
			// Add services to the container.
			services.AddControllers();

			
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();

			//Add Dbcontext
			var connectionString = configuration.GetConnectionString("DefaultConnection") ??
			throw new InvalidOperationException("connectionString 'DefaultConnection' not found");

			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

			//Add Mapster
			var mappingConfig = TypeAdapterConfig.GlobalSettings;
			mappingConfig.Scan(Assembly.GetExecutingAssembly());
			services.AddSingleton<IMapper>(new Mapper(mappingConfig));


			// Add Fluent Validation
			services
				.AddFluentValidationAutoValidation()
				.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


			//add my services
			services.AddScoped<IPollService, PollService>();

			return services;
		}
	}
}
