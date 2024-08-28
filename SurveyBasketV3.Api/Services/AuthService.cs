using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasketV3.Api.Authentication;
using SurveyBasketV3.Api.Contracts.Authentication;
using SurveyBasketV3.Api.Contracts.Users;
using SurveyBasketV3.Api.Errors;
using SurveyBasketV3.Api.Helpers;
using SurveyBasketV3.Api.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasketV3.Api.Services
{
	public class AuthService(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager,
		IJwtProvider jwtProvider,
		ILogger<AuthService> logger,
		IEmailSender emailSender,
		IHttpContextAccessor httpContextAccessor,
		ApplicationDbContext context
		) : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
		private readonly IJwtProvider _jwtProvider = jwtProvider;
		private readonly ILogger<AuthService> _logger = logger;
		private readonly IEmailSender _emailSender = emailSender;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
		private readonly ApplicationDbContext _context = context;
		private readonly int _refreshTokenExpiryDays = 14;
		public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
		{
			//check if there's a user with this email
			if (await _userManager.FindByEmailAsync(email) is not { } user)
				return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

			if(user.IsDisabled)
				return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

			//check if password is correct and is email confirmed using userManager
			//if (!user.EmailConfirmed)
			//	return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);

			//var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
			//if (!isPasswordCorrect)
			//	return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);


			//check if password is correct and is email confirmed using signInManager
			var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
			if (!result.Succeeded)
			{
				var error = result.IsNotAllowed 
					? UserErrors.EmailNotConfirmed 
					: result.IsLockedOut 
						? UserErrors.LockedUser
						: UserErrors.InvalidCredentials;

				return Result.Failure<AuthResponse>(error);
			}
				

			var authResponse = await GetAuthResponseAsync(user, cancellationToken);

			return Result.Success(authResponse);
		}

		public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			var user = await _userManager.FindByIdAsync(userId);
			if(user is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			if (user.IsDisabled)
				return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

			if (user.LockoutEnd > DateTime.UtcNow)
				return Result.Failure<AuthResponse>(UserErrors.LockedUser);

			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token ==  refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

			userRefreshToken.RevokedOn = DateTime.UtcNow;

			var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user, cancellationToken);
			//generate JWT token
			var (newJwtToken, expiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);

			//generate Refresh token
			var newRefreshtoken = GenerateRefreshToken();
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

			//Add Refresh Token to the database
			user.RefreshTokens.Add(new RefreshToken
			{
				Token = newRefreshtoken,
				ExpiresOn = refreshTokenExpiration
			});

			await _userManager.UpdateAsync(user);

			var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newJwtToken, expiresIn
					, newRefreshtoken, refreshTokenExpiration);

			return Result.Success(authResponse);
		}

		public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return Result.Failure(UserErrors.InvalidJwtToken);

			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure(UserErrors.InvalidJwtToken);

			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return Result.Failure(UserErrors.InvalidRefreshToken);

			userRefreshToken.RevokedOn = DateTime.UtcNow;

			await _userManager.UpdateAsync(user);

			return Result.Success();
		}

		public async Task<Result> RegisterAsync(RegisterRequest request,CancellationToken cancellationToken = default)
		{
			var isUserExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
			if(isUserExists)
				return Result.Failure(UserErrors.ExistedEmail);

			ApplicationUser user = request.Adapt<ApplicationUser>();

			var result = await _userManager.CreateAsync(user, request.Password);
			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
			}

			// generate verfication code 
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			_logger.LogInformation("confirm your email {code}", code);
			//generate email body and send it
			await SendEmailConfirmationAsync(user, code);

			return Result.Success();
		}

		public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
		{
			if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
				return Result.Failure(UserErrors.InvalidConfirmationCode);

			if(user.EmailConfirmed)
				return Result.Failure(UserErrors.EmailAlreadyConfirmed);

			var code = request.Code;
			try
			{
				code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			}
			catch (FormatException)
			{
				return Result.Failure(UserErrors.InvalidConfirmationCode);
			}

			var result = await _userManager.ConfirmEmailAsync(user, code);
			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure(new Error(error.Code,error.Description, StatusCodes.Status400BadRequest));
			} 

			// assign default role (member)
			await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
			return Result.Success();
			
		}

		public async Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request)
		{
			if(await _userManager.FindByEmailAsync(request.Email) is not { } user)
				return Result.Failure(UserErrors.EmailNotFound);

			var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
			if(isEmailConfirmed)
				return Result.Failure(UserErrors.EmailAlreadyConfirmed);

			// generate verfication code 
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			_logger.LogInformation("confirm your email {code}", code);
			//generate email body and send it
			await SendEmailConfirmationAsync(user, code);

			

			return Result.Success();
		}

		public async Task<Result> SendCodeToRestPasswordAsync(string email)
		{

			if (await _userManager.FindByEmailAsync(email) is not { } user)
				return Result.Success();

			if(!user.EmailConfirmed)
				return Result.Failure(UserErrors.EmailNotConfirmed);

			// generate verfication code 
			var code = await _userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			_logger.LogInformation("confirm your email {code}", code);
			//generate email body and send it
			await SendResetPasswordAsync(user, code);
			return Result.Success();

		}

		public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
		{
			if(await _userManager.FindByEmailAsync(request.Email) is not { } user)
				return Result.Failure(UserErrors.EmailNotFound);

			if (!user.EmailConfirmed) 
				return Result.Failure(UserErrors.EmailNotConfirmed); 

			var code = request.Code;

			IdentityResult result;
			try
			{
				code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
				result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
			}
			catch (FormatException)
			{
				result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
			}

			if (!result.Succeeded)
			{
				var error = result.Errors.First();
				return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
			}

			return Result.Success();
		}
		private static string GenerateRefreshToken()
		{
			var number = RandomNumberGenerator.GetBytes(64);
			var token = Convert.ToBase64String(number);

			return token;
		}

		private async Task<AuthResponse> GetAuthResponseAsync(ApplicationUser user, CancellationToken cancellationToken = default)
		{
			var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user, cancellationToken);
			//generate JWT token
			var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);

			//generate Refresh token
			var refreshtoken = GenerateRefreshToken();
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

			//Add Refresh Token to the database
			user.RefreshTokens.Add(new RefreshToken
			{
				Token = refreshtoken,
				ExpiresOn = refreshTokenExpiration
			});

			await _userManager.UpdateAsync(user);

			var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn
					, refreshtoken, refreshTokenExpiration);

			return authResponse;
		}

		private async Task<(IEnumerable<string>, IEnumerable<string>)> GetUserRolesAndPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
		{
			var userRoles = await _userManager.GetRolesAsync(user);
			//var userPermissions = await _context.Roles
			//		.Join(_context.RoleClaims,
			//			role => role.Id,
			//			roleClaim => roleClaim.RoleId,
			//			(role, roleClaim) => new { role, roleClaim }
			//		).Where(x => userRoles.Contains(x.role.Name!))
			//		.Select(x => x.roleClaim.ClaimValue!)
			//		.Distinct()
			//		.ToListAsync(cancellationToken);

			var userPermissions = await (from r in _context.Roles
								   join p in _context.RoleClaims
								   on r.Id equals p.RoleId
								   where userRoles.Contains(r.Name!)
								   select p.ClaimValue!
								  )
								   .Distinct()
								   .ToListAsync(cancellationToken);

			return (userRoles, userPermissions);
		}
		private async Task SendEmailConfirmationAsync(ApplicationUser user, string code)
		{
			// send email
			var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
			var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
					new Dictionary<string, string>
					{
						{"{{name}}", $"{user.FirstName} {user.LastName}"},
						{"{{action_url}}", $"{origin}/auth/EmailConfirmation?UserId={user.Id}&Code={code}"}
					}
			);

			BackgroundJob.Enqueue(()
				=> _emailSender.SendEmailAsync(user.Email!, "✔ Survey Basket: Email Confirmation", emailBody));

			await Task.CompletedTask;
		}
		private async Task SendResetPasswordAsync(ApplicationUser user, string code)
		{
			// send email
			var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
			var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
					new Dictionary<string, string>
					{
						{"{{name}}", $"{user.FirstName} {user.LastName}"},
						{"{{action_url}}", $"{origin}/auth/ForgetPassword?Email={user.Email}&Code={code}"}
					}
			);

			BackgroundJob.Enqueue(()
				=> _emailSender.SendEmailAsync(user.Email!, "✔ Survey Basket: Reset Password", emailBody));

			await Task.CompletedTask;
		}
	}
}
