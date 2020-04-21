using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Services
{
    public interface ICommentService
    {
        Task<CommentDTO> AddComment(CommentDTO comment, string token);
        Task DeleteComment(int id, string token);
        Task UpdateComment(int id, CommentDTO commentDto, string token);
        Task<CommentDTO> GetCommentById(int id);
        Task<IEnumerable<CommentDTO>> GetAllComments();
        Task<IEnumerable<CommentDTO>> GetAllCommentsByUserId(int id);
    }
}