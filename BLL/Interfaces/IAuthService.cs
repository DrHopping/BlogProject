using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Services
{
    public interface IAuthService
    {
        Task<object> Authenticate(UserDTO user);
    }
}