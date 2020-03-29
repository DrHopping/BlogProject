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

        public async Task<bool> DeleteUser(string id, string token)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (token == null) throw new ArgumentNullException(nameof(token));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new ArgumentNullException(nameof(user), $"Couldn't find user with id {id}");

            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (requesterId == id) return (await _userManager.DeleteAsync(user)).Succeeded;

            var requesterRoleClaim = _jwtFactory.GetUserRoleClaim(token);
            if (requesterRoleClaim == "Admin")
            {
                return (await _userManager.DeleteAsync(user)).Succeeded;
            }
            throw new NotEnoughRightsException();
        }

        public async Task<bool> UpdateUser(string id, UserDTO user, string token)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.UserName == null && user.Email == null) throw new ArgumentNullException(nameof(user));

            var requesterId = _jwtFactory.GetUserIdClaim(token);
            var requesterRole = _jwtFactory.GetUserRoleClaim(token);

            var userEntity = await _userManager.FindByIdAsync(id);
            if (userEntity == null) throw new ArgumentNullException(nameof(userEntity), $"Couldn't find user with id {id}");

            if (requesterId == id || requesterRole == "Admin")
            {
                if (user.UserName != null && userEntity.UserName.CompareTo(user.UserName) != 0)
                {
                    var isNameTaken = await _userManager.FindByNameAsync(user.UserName);
                    if (isNameTaken != null) throw new NameAlreadyTakenException();
                    userEntity.UserName = user.UserName;
                }
                else if (user.Email != null && userEntity.Email.CompareTo(user.Email) != 0)
                {
                    var isEmailTaken = await _userManager.FindByEmailAsync(user.Email);
                    if (isEmailTaken != null) throw new NameAlreadyTakenException();
                    userEntity.Email = user.Email;
                }

                return (await _userManager.UpdateAsync(userEntity)).Succeeded;
            }

            throw new NotEnoughRightsException();
        }

        public async Task<bool> ChangePassword(string id, PasswordDTO password, string token)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (password.NewPassword == null) throw new ArgumentNullException(nameof(password.NewPassword));
            if (password.OldPassword == null) throw new ArgumentNullException(nameof(password.OldPassword));

            var requesterId = _jwtFactory.GetUserIdClaim(token);
            var requesterRole = _jwtFactory.GetUserRoleClaim(token);
            var userEntity = await _userManager.FindByIdAsync(id);
            if (userEntity == null) throw new ArgumentNullException(nameof(userEntity), $"Couldn't find user with id {id}");

            if (requesterId != id && requesterRole != "Admin") throw new NotEnoughRightsException();

            bool checkPassword = await _userManager.CheckPasswordAsync(userEntity, password.OldPassword);
            if (checkPassword == false) throw new ArgumentException(nameof(password));
            return (await _userManager.ChangePasswordAsync(userEntity, password.OldPassword, password.NewPassword)).Succeeded;

        }
    }
}