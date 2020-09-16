using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
    public class JwtFactory : IJwtFactory
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<User> _userManager;

        public JwtFactory(UserManager<User> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }
        public JwtSecurityToken DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }

        public string GenerateEncodedToken(string userName, ClaimsIdentity identity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ClaimsIdentity> GenerateClaimsIdentity(User user)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            });
            var roles = await _userManager.GetRolesAsync(user);
            identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            return identity;
        }

        public int GetUserIdClaim(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var decodedToken = DecodeToken(token);
            var id = decodedToken.Claims.Single(c => c.Type == "nameid").Value;
            return int.Parse(id);
        }

        public string GetUserRoleClaim(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var decodedToken = DecodeToken(token);
            return decodedToken.Claims.First(c => c.Type == "role").Value;
        }
    }
}