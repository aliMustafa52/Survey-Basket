
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasketV3.Api.Authentication
{
	public class JwtProvider : IJwtProvider
	{
		public (string token, int expiresIn) GenerateToken(ApplicationUser user)
		{
			Claim[] claims = [
				new(JwtRegisteredClaimNames.Sub,user.Id),
				new(JwtRegisteredClaimNames.Email,user.Email!),
				new(JwtRegisteredClaimNames.GivenName,user.FirstName),
				new(JwtRegisteredClaimNames.FamilyName,user.LastName),
				new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
			];

			var expiresIn = 30;

			var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Ed3B6NzKG7jgvT2jZB6OPjm5m1EnmL2i"));
			var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: "SurveyBasket",
				audience: "SurveyBasket Users",
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(expiresIn),
				signingCredentials: signingCredentials
			);

			return (token: new JwtSecurityTokenHandler().WriteToken(token),expiresIn:expiresIn *60);
		}
	}
}
