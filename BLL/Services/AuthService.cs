using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;

        public AuthService(UserManager<User> userManager, IJwtFactory jwtFactory)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(UserDTO user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.Username == null) throw new ArgumentNullException(nameof(user.Username));
            if (user.Password == null) throw new ArgumentNullException(nameof(user.Password));

            var userToVerify = await _userManager.FindByNameAsync(user.Username);
            if (userToVerify == null) throw new WrongCredentialsException();

            if (await _userManager.CheckPasswordAsync(userToVerify, user.Password))
            {
                return await _jwtFactory.GenerateClaimsIdentity(userToVerify);
            }

            throw new WrongCredentialsException();
        }

        public async Task<object> Authenticate(UserDTO userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));

            var identity = await GetClaimsIdentity(userDto);
            if (identity == null) throw new ArgumentNullException(nameof(identity));

            var token = await _jwtFactory.GenerateEncodedToken(userDto.Username, identity);
            if (token == null) throw new ArgumentNullException(nameof(token));
            return new
            {
                id = identity.FindFirst(ClaimTypes.NameIdentifier).Value,
                username = identity.FindFirst(ClaimTypes.Name).Value,
                role = identity.FindFirst(ClaimTypes.Role).Value,
                auth_token = token,
            };
        }
    }
}