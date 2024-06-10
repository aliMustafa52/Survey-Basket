using SurveyBasketV3.Api.Authentication;
using System.Security.Cryptography;

namespace SurveyBasketV3.Api.Services
{
	public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly IJwtProvider _jwtProvider = jwtProvider;

		private readonly int _refreshTokenExpiryDays = 14;
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

			return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn
					,refreshtoken,refreshTokenExpiration);
		}

		public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return null;

			var user = await _userManager.FindByIdAsync(userId);
			if(user is null)
				return null;

			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token ==  refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return null;

			userRefreshToken.RevokedOn = DateTime.UtcNow;

			//generate JWT token
			var (newJwtToken, expiresIn) = _jwtProvider.GenerateToken(user);

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

			return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newJwtToken, expiresIn
					, newRefreshtoken, refreshTokenExpiration);
		}

		public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return false;

			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return false;

			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return false;

			userRefreshToken.RevokedOn = DateTime.UtcNow;

			await _userManager.UpdateAsync(user);

			return true;
		}

		private static string GenerateRefreshToken()
		{
			var number = RandomNumberGenerator.GetBytes(64);
			var token = Convert.ToBase64String(number);

			return token;
		}

		
	}
}
