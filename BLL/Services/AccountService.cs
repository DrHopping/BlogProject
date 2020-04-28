using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Helpers;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(UserManager<User> userManager, IJwtFactory jwtFactory, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        private async Task<User> CreateUser(UserDTO userDto)
        {
            if (await _userManager.FindByEmailAsync(userDto.Email) != null) throw new EmailAlreadyTakenException(userDto.Email);
            if (await _userManager.FindByNameAsync(userDto.Username) != null) throw new NameAlreadyTakenException(userDto.Username);
            var user = _mapper.Map<UserDTO, User>(userDto);
            user.AvatarUrl = DefaultAvatarUrlProvider.GetDefaultAvatarUrl();
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Errors.Any()) throw new BadPasswordException(result.Errors);
            return result.Succeeded ? await _userManager.FindByNameAsync(user.UserName) : null;
        }

        private async Task<UserDTO> RegisterToRole(string role, UserDTO userDto)
        {
            var user = await CreateUser(userDto);
            if (user == null) throw new ArgumentNullException("Couldn't create user");
            await _userManager.AddToRoleAsync(user, role);
            return _mapper.Map<User, UserDTO>(user);
        }

        private async Task<IEnumerable<UserDTO>> GetUsersByRole(string role)
            => _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>((await _userManager.GetUsersInRoleAsync(role)).ToList());

        public async Task<UserDTO> RegisterRegularUser(UserDTO userDto) => await RegisterToRole("RegularUser", userDto);
        public async Task<UserDTO> RegisterModerator(UserDTO userDto) => await RegisterToRole("Moderator", userDto);
        public async Task<IEnumerable<UserDTO>> GetAllRegularUsers() => await GetUsersByRole("RegularUser");
        public async Task<IEnumerable<UserDTO>> GetAllModerators() => await GetUsersByRole("Moderator");
        public async Task<IEnumerable<UserDTO>> GetAllUsers()
            => _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(await _userManager.Users.ToListAsync());

        public async Task<UserDTO> GetUserById(int id, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) throw new EntityNotFoundException(nameof(user), id);

            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (requesterId == id) return _mapper.Map<User, UserDTO>(user);

            var requesterRoleClaim = _jwtFactory.GetUserRoleClaim(token);

            switch (requesterRoleClaim)
            {
                case "Admin":
                    return _mapper.Map<User, UserDTO>(user);
                case "Moderator":
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Any(r => r == "Moderator" || r == "Admin")) throw new NotEnoughRightsException();
                        return _mapper.Map<User, UserDTO>(user);
                    }
                default:
                    throw new NotEnoughRightsException();
            }
        }

        public async Task<bool> DeleteUser(int id, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) throw new EntityNotFoundException(nameof(user), id);

            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (requesterId == id) return (await _userManager.DeleteAsync(user)).Succeeded;

            var requesterRoleClaim = _jwtFactory.GetUserRoleClaim(token);
            if (requesterRoleClaim == "Admin")
            {
                return (await _userManager.DeleteAsync(user)).Succeeded;
            }
            throw new NotEnoughRightsException();
        }

        public async Task<bool> UpdateUser(int id, UserDTO user, string token)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.Username == null && user.Email == null) throw new ArgumentNullException(nameof(user));

            var requesterId = _jwtFactory.GetUserIdClaim(token);
            var requesterRole = _jwtFactory.GetUserRoleClaim(token);

            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) throw new EntityNotFoundException(nameof(userEntity), id);

            if (requesterId != id && requesterRole != "Admin") throw new NotEnoughRightsException();

            if (user.Username != null && !userEntity.UserName.Equals(user.Username))
            {
                var isNameTaken = await _userManager.FindByNameAsync(user.Username);
                if (isNameTaken != null) throw new NameAlreadyTakenException(user.Username);
                userEntity.UserName = user.Username;
            }

            if (user.Email != null && !userEntity.Email.Equals(user.Email))
            {
                var isEmailTaken = await _userManager.FindByEmailAsync(user.Email);
                if (isEmailTaken != null) throw new EmailAlreadyTakenException(user.Email);
                userEntity.Email = user.Email;
            }

            if (user.AvatarUrl != null && !userEntity.AvatarUrl.Equals(user.AvatarUrl))
            {
                userEntity.AvatarUrl = user.AvatarUrl;
            }

            return (await _userManager.UpdateAsync(userEntity)).Succeeded;
        }

        public async Task<bool> ChangePassword(int id, PasswordDTO password, string token)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (password.NewPassword == null) throw new ArgumentNullException(nameof(password.NewPassword));
            if (password.OldPassword == null) throw new ArgumentNullException(nameof(password.OldPassword));

            var requesterId = _jwtFactory.GetUserIdClaim(token);
            var requesterRole = _jwtFactory.GetUserRoleClaim(token);
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) throw new EntityNotFoundException(nameof(userEntity), id);

            if (requesterId != id && requesterRole != "Admin") throw new NotEnoughRightsException();

            bool checkPassword = await _userManager.CheckPasswordAsync(userEntity, password.OldPassword);
            if (checkPassword == false) throw new WrongCredentialsException();
            return (await _userManager.ChangePasswordAsync(userEntity, password.OldPassword, password.NewPassword)).Succeeded;
        }
    }
}