using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BLL.Services
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager<User> _userManager;

        public JwtFactory(IOptions<JwtOptions> jwtOptions, UserManager<User> userManager)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
            CheckOptions(_jwtOptions);
        }

        private static void CheckOptions(JwtOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Time span less than zero", nameof(JwtOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtOptions.JtiGenerator));
            }
        }

        public JwtSecurityToken DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }

        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator())
            };

            claims.AddRange(identity.Claims);

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public async Task<ClaimsIdentity> GenerateClaimsIdentity(User user)
        {
            var claims = new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"), new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            });
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddClaims(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));
            return claims;
        }

        public string GetUserIdClaim(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var decodedToken = DecodeToken(token);
            return decodedToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }

        public string GetUserRoleClaim(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var decodedToken = DecodeToken(token);
            return decodedToken.Claims.First(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
        }
    }
}