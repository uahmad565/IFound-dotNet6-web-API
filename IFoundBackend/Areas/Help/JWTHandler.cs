using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;

using IFoundBackend.Model;
using System.Collections.Generic;

namespace IFoundBackend.Areas.Help
{
    public static class JWTHandler
    {
       
        public static string GetUserID(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                // Create a JwtSecurityTokenHandler to parse the token
                var tokenHandler = new JwtSecurityTokenHandler();

                // Read and parse the token
                var parsedToken = tokenHandler.ReadJwtToken(token);

                // Access the claims from the token
                var claims = parsedToken.Claims;
                
                return claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; 
            }
            return null;
        }

        internal static List<Claim> PrepareClaims(ApplicationUser user,IList<string> userRoles)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier,user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.DateOfBirth, DateTime.UtcNow.ToString()),
            };

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            return claims;
        }
    }
}
