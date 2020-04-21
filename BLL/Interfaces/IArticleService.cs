using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Services
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDTO>> GetArticlesByTags(string tags);
        Task<ArticleDTO> CreateArticle(ArticleDTO articleDto, string token);
        Task DeleteArticle(int id, string token);
        Task UpdateArticle(int id, ArticleDTO articleDto, string token);
        Task<ArticleDTO> GetArticleById(int id);
        Task<IEnumerable<CommentDTO>> GetCommentsByArticleId(int id);
        Task<IEnumerable<TagDTO>> GetTagsByArticleId(int id);
        Task<IEnumerable<ArticleDTO>> GetArticlesByTextFilter(string filter);
        Task<IEnumerable<ArticleDTO>> GetAllArticles();
    }
}