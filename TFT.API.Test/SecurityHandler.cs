using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TFT.API.Test
{
    public static class SecurityHandler
    {
        public static IEnumerable<Claim> GetClaimsFromJWT(String token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            return securityToken.Claims;
        }

        public static String? GetClaimByName(IEnumerable<Claim> claims, String claimName) => claims.FirstOrDefault(c => c.Type == claimName)?.Value;
    }
}
