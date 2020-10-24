using IdentityModel;
using SHB.Core.Domain.Entities;
using SHB.Core.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace SHB.WebAPI.Utils
{
    public static class ClaimsExtensions
    {
        public static List<Claim> UserToClaims(this User user)
        {
            //These wont be null
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email)
            };

            //these can.

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
            }
            if (!string.IsNullOrWhiteSpace(user.CompanyId.ToString()))
            {
                claims.Add(new Claim(JwtClaimTypes.PreferredUserName, user.CompanyId.ToString()));
            }
            return claims;
        }
    }
}