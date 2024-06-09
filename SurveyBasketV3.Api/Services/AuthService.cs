using SurveyBasketV3.Api.Authentication;

namespace SurveyBasketV3.Api.Services
{
	public class AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider) : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly IJwtProvider _jwtProvider = jwtProvider;

		public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
		{
			//check if there's a user with this email
			var user = await _userManager.FindByEmailAsync(email);
			if (user is null)
				return null;

			//check if password is correct
			var isPasswordCorrect=await _userManager.CheckPasswordAsync(user, password);
			if(!isPasswordCorrect)
				return null;

			//generate JWT token
			var (token, expiresIn) = _jwtProvider.GenerateToken(user);

			return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn);
		}
	}
}
