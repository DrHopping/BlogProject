using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Services
{
    public interface IAccountService
    {
        Task<UserDTO> RegisterRegularUser(UserDTO userDTO);
        Task<UserDTO> RegisterModerator(UserDTO userDTO);
        Task<IEnumerable<UserDTO>> GetAllRegularUsers();
        Task<IEnumerable<UserDTO>> GetAllModerators();
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task<UserDTO> GetUserById(string id, string token);
        Task<bool> DeleteUser(string id, string token);
        Task<bool> UpdateUser(string id, UserDTO user, string token);
        Task<bool> ChangePassword(string id, PasswordDTO password, string token);
        Task<IEnumerable<BlogDTO>> GetAllBlogsByUserId(string id);
        Task<IEnumerable<CommentDTO>> GetAllCommentsByUserId(string id);
    }
}