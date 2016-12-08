using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CloudMineServer.API_server.Services
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal contextUser) 
            => contextUser.Claims.Where(c => c.Type.Contains(JwtRegisteredClaimNames.NameId)).FirstOrDefault().Value;

        public static string GetUserEmail(this ClaimsPrincipal contextUser)
            => contextUser.Claims.Where(c => c.Type.Contains(JwtRegisteredClaimNames.Email)).FirstOrDefault().Value;
    }
}
