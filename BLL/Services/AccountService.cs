using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Mappers;
using Castle.Components.DictionaryAdapter;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class AccountService
    {
        private readonly UserManager<User> _userManager;
        private IJwtFactory _jwtFactory;
        private IUnitOfWork _unitOfWork;
        private UserMapper _userMapper;
        private BlogMapper _blogMapper;
        private CommentMapper _commentMapper;

        private BlogMapper BlogMapper => _blogMapper ??= new BlogMapper();
        private UserMapper UserMapper => _userMapper ??= new UserMapper();
        private CommentMapper CommentMapper => _commentMapper ??= new CommentMapper();

        public AccountService(UserManager<User> userManager, IJwtFactory jwtFactory, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _unitOfWork = unitOfWork;
        }
        private async Task<User> CreateUser(UserDTO userDTO)
        {
            if (await _userManager.FindByEmailAsync(userDTO.Email) != null) throw new EmailAlreadyTakenException();
            if (await _userManager.FindByNameAsync(userDTO.UserName) != null) throw new NameAlreadyTakenException();

            var user = UserMapper.Map(userDTO);

            var result = await _userManager.CreateAsync(user, userDTO.Password);
            return result.Succeeded ? await _userManager.FindByNameAsync(user.UserName) : null;
        }

        private async Task<UserDTO> RegisterToRole(string role, UserDTO userDTO)
        {
            var user = await CreateUser(userDTO);
            if (user == null) throw new ArgumentNullException(nameof(user), "Couldn't create user");
            await _userManager.AddToRoleAsync(user, role);
            return UserMapper.Map(user);
        }

        private async Task<IEnumerable<UserDTO>> GetUsersByRole(string role)
            => UserMapper.Map((await _userManager.GetUsersInRoleAsync(role)).ToList());

        public async Task<UserDTO> RegisterRegularUser(UserDTO userDTO) => await RegisterToRole("RegularUser", userDTO);
        public async Task<UserDTO> RegisterModerator(UserDTO userDTO) => await RegisterToRole("Moderator", userDTO);
        public async Task<IEnumerable<UserDTO>> GetAllRegularUsers() => await GetUsersByRole("RegularUser");
        public async Task<IEnumerable<UserDTO>> GetAllModerators() => await GetUsersByRole("Moderator");
        public async Task<IEnumerable<UserDTO>> GetAllUsers()
            => UserMapper.Map(await _userManager.Users.ToListAsync());

        public async Task<UserDTO> GetUserById(string id, string token)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (token == null) throw new ArgumentNullException(nameof(token));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new ArgumentNullException(nameof(user), $"Couldn't find user with id {id}");

            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (requesterId == id) return UserMapper.Map(user);

            var requesterRoleClaim = _jwtFactory.GetUserRoleClaim(token);

            switch (requesterRoleClaim)
            {
                case "Admin":
                    return UserMapper.Map(user);
                case "Moderator":
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Any(r => r == "Moderator" || r == "Admin")) throw new NotEnoughRightsException();
                        return UserMapper.Map(user);
                    }
                default:
                    throw new NotEnoughRightsException();
            }
        }
    }
}