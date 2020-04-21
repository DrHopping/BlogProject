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
        private readonly JwtOptions _jwtOptions;
        private readonly AppSettings _appSettings;

        public AuthService(UserManager<User> userManager, IJwtFactory jwtFactory, IOptions<JwtOptions> jwtOptions, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _appSettings = appSettings.Value;
            _jwtOptions = jwtOptions.Value;
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(UserDTO user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.UserName == null) throw new ArgumentNullException(nameof(user.UserName));
            if (user.Password == null) throw new ArgumentNullException(nameof(user.Password));

            var userToVerify = await _userManager.FindByNameAsync(user.UserName);
            if (userToVerify == null)
            {
                userToVerify = await _userManager.FindByEmailAsync(user.Email);
                if (userToVerify == null)
                {
                    throw new WrongCredentialsException();
                }
            }

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

            var token = await _jwtFactory.GenerateEncodedToken(userDto.UserName, identity);
            if (token == null) throw new ArgumentNullException(nameof(token));
            return new
            {
                id = identity.FindFirst(ClaimTypes.Name).Value,
                auth_token = token,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };
        }
    }
}