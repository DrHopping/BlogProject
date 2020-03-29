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
    }
}