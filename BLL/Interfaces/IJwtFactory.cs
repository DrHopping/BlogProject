﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IJwtFactory
    {
        JwtSecurityToken DecodeToken(string token);
        string GenerateEncodedToken(string userName, ClaimsIdentity identity);
        Task<ClaimsIdentity> GenerateClaimsIdentity(User user);
        int GetUserIdClaim(string token);
        string GetUserRoleClaim(string token);
    }
}