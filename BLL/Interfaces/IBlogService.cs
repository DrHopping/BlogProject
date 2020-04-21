using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Services
{
    public interface IBlogService
    {
        Task<BlogDTO> CreateBlog(BlogDTO blogDto, string token);
        Task DeleteBlog(int id, string token);
        Task UpdateBlogName(int id, BlogDTO blogDto, string token);
        Task<BlogDTO> GetBlogById(int id);
        Task<IEnumerable<BlogDTO>> GetAllBlogs();
        Task<IEnumerable<ArticleDTO>> GetAllArticlesByBlogId(int id);
        Task<IEnumerable<BlogDTO>> GetAllBlogsByUserId(int id);
    }
}