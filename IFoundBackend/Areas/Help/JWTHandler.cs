using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

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
    }
}
