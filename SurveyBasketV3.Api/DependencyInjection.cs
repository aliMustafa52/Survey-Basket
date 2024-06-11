using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyBasketV3.Api.Authentication;
using SurveyBasketV3.Api.Persistence;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace SurveyBasketV3.Api
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
		{
			// Add services to the container.
			services.AddControllers();

			//Add Cors
			services.AddCors(options =>
			options.AddDefaultPolicy(builder => builder
											.AllowAnyMethod()
											.AllowAnyHeader()
											.AllowAnyOrigin()
			));

			services
				.AddSwaggerConfig()
				.AddDbContextConfig(configuration)
				.AddMapsterConfig()
				.AddFluentValidationConfig()
				.AddAuthConfig(configuration);

			//add my services
			services.AddScoped<IPollService, PollService>();
			services.AddScoped<IAuthService, AuthService>();
			

			return services;
		}

		private static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
		{
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			return services;
		}
		private static IServiceCollection AddDbContextConfig(this IServiceCollection services,IConfiguration configuration)
		{
			//Add Dbcontext
			var connectionString = configuration.GetConnectionString("DefaultConnection") ??
			throw new InvalidOperationException("connectionString 'DefaultConnection' not found");

			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
			return services;
		}
		private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
		{
			//Add Mapster
			var mappingConfig = TypeAdapterConfig.GlobalSettings;
			mappingConfig.Scan(Assembly.GetExecutingAssembly());
			services.AddSingleton<IMapper>(new Mapper(mappingConfig));
			return services;
		}
		private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
		{
			// Add Fluent Validation
			services
				.AddFluentValidationAutoValidation()
				.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
			return services;
		}
		private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
		{
			// add identity UserManager and IdentityRole
			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			//single instance throuhout the project
			services.AddSingleton<IJwtProvider, JwtProvider>();

			// link JwtOptions class with configuration Jwt
			//services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

			services.AddOptions<JwtOptions>()
				.BindConfiguration(JwtOptions.SectionName)
				.ValidateDataAnnotations()
				.ValidateOnStart();


			//to use JwtOptions here
			var jwtSettings =configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>(); 

			services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.SaveToken = true;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
						ValidIssuer = jwtSettings?.Issuer,
						ValidAudience = jwtSettings?.Audience,
					};
				});

			return services;
		}
	}
}
