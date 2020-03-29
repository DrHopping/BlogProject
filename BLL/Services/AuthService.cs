using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public class AuthService
    {
        private UserManager<User> _userManager;
        private IJwtFactory _jwtFactory;
        private JwtOptions _jwtIssuerOptions;

        public AuthService(UserManager<User> userManager, IJwtFactory jwtFactory, JwtOptions jwtIssuerOptions)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtIssuerOptions = jwtIssuerOptions;
        }

        public async Task<ClaimsIdentity> GetClaimsIdentity(UserDTO user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.UserName == null) throw new ArgumentNullException(nameof(user.UserName));
            if (user.Password == null) throw new ArgumentNullException(nameof(user.Password));

            var userToVerify = await _userManager.FindByNameAsync(user.UserName);
            if (userToVerify == null)
            {
                userToVerify = await _userManager.FindByEmailAsync(user.UserName);
                if (userToVerify == null)
                {
                    throw new WrongCredentialsException();
                }
            }

            if (await _userManager.CheckPasswordAsync(userToVerify, user.Password))
            {
                return await _jwtFactory.GenerateClaimsIdentity(userToVerify);
            }

            if (await _userManager.CheckPasswordAsync(userToVerify, user.Password))
            {
                return await _jwtFactory.GenerateClaimsIdentity(userToVerify);
            }

            throw new WrongCredentialsException();
        }
    }
}