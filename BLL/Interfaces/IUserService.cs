using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Services
{
    public interface IUserService
    {
        Task<UserDTO> RegisterRegularUser(UserDTO userDTO);
        Task<UserDTO> RegisterModerator(UserDTO userDTO);
        Task<IEnumerable<UserDTO>> GetAllRegularUsers();
        Task<IEnumerable<UserDTO>> GetAllModerators();
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task<UserDTO> GetUserById(int id, string token);
        Task<PublicUserInfoDTO> GetPublicUserInfoById(int id);
        Task<bool> DeleteUser(int id, string token);
        Task<bool> UpdateUser(int id, UserDTO user, string token);
        Task<bool> ChangePassword(int id, PasswordDTO password, string token);
        Task PromoteUser(int id, string token);
        Task UnpromoteUser(int id, string token);

    }
}