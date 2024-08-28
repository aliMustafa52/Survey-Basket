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
using SurveyBasketV3.Api.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Hangfire;
using SurveyBasketV3.Api.Authentication.Filters;
using SurveyBasketV3.Api.Health;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

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
				.AddAuthConfig(configuration)
				.AddBackgroundJobsConfig(configuration);

			//add my services
			services.AddScoped<IPollService, PollService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IQuestionService, QuestionService>();
			services.AddScoped<IVoteService, VoteService>();
			services.AddScoped<IResultService, ResultService>();
			services.AddScoped<ICacheService, CacheService>();
			services.AddScoped<IEmailSender, EmailService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IRoleService, RoleService>();

			services.AddHttpContextAccessor();
			//add GlobalExceptionHandler
			services.AddExceptionHandler<GlobalExceptionHandler>();
			services.AddProblemDetails();

			// map mail settings
			services.AddOptions<MailSettings>()
				.BindConfiguration(nameof(MailSettings))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			//Add Health Check
			services.AddHealthChecks()
				.AddSqlServer(name: "Database", connectionString: configuration.GetConnectionString("DefaultConnection")!)
				.AddHangfire(options => options.MinimumAvailableServers = 1)
				.AddUrlGroup(name: "External Api", uri: new Uri("https://www.google.com"))
				.AddCheck<MailProviderHealthCheck>(name: "Mail Service");

			//Add Rate Limiter
			services.AddDbRateLimitConfig();

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
			services.AddIdentity<ApplicationUser, ApplicationRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			//register sevices
			services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
			services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

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

			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequiredLength = 8;
				options.SignIn.RequireConfirmedEmail = true;
				options.User.RequireUniqueEmail = true;
			});

			return services;
		}
		private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
		{
			// Add Hangfire services.
			services.AddHangfire(HangfireConfiguration => HangfireConfiguration
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"))
			);

			// Add the processing server as IHostedService
			services.AddHangfireServer();

			return services;
		}

		private static IServiceCollection AddDbRateLimitConfig(this IServiceCollection services)
		{
			services.AddRateLimiter(rateLimiterOptions =>
			{
				rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
				rateLimiterOptions.AddPolicy(RateLimitPolicies.IpLimiter, httpContext =>
					RateLimitPartition.GetFixedWindowLimiter(
						partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
						factory: _ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 2,
							Window = TimeSpan.FromSeconds(20)
						}
					)
				);

				rateLimiterOptions.AddPolicy(RateLimitPolicies.UserLimiter, httpContext =>
					RateLimitPartition.GetFixedWindowLimiter(
						partitionKey: httpContext.User.Identity?.Name?.ToString(),
						factory: _ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 4,
							Window = TimeSpan.FromSeconds(20)
						}
					)
				);

				rateLimiterOptions.AddConcurrencyLimiter(RateLimitPolicies.Concurrency, options =>
				{
					options.PermitLimit = 1000;
					options.QueueLimit = 100;
					options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				});
			});

			return services;
		}
	}
}
