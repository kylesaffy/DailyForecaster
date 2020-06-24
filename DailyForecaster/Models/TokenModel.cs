using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class TokenModel
	{
		private const string Secret = "B1C0DEC7768744049E2740D526E8C33A687988713F8B4332A6851FDE82C1616A6ECD3DEFA468440193E84B79D04B01E1";
		public string generateToken(string userName, int expireMin = 20)
		{
			var symKey = Convert.FromBase64String(Secret);
			var tokenHandler = new JwtSecurityTokenHandler();
			var now = DateTime.UtcNow;
			var tokenDescirptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name,userName)
				}),

				Expires = now.AddMinutes(expireMin),

				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(symKey),
					SecurityAlgorithms.HmacSha256Signature)
			};
			var stoken = tokenHandler.CreateToken(tokenDescirptor);
			return tokenHandler.WriteToken(stoken);
		}
		public ClaimsPrincipal GetPrincipal(string token)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

				if (jwtToken == null)
					return null;

				var symmetricKey = Convert.FromBase64String(Secret);

				var validationParameters = new TokenValidationParameters()
				{
					RequireExpirationTime = true,
					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
				};

				SecurityToken securityToken;
				var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

				return principal;
			}
			catch (Exception)
			{
				//should write log
				return null;
			}
		}
	}
}
