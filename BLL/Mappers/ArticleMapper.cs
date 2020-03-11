using BLL.DTO;
using DAL.Entities;

namespace BLL.Mappers
{
    public class ArticleMapper : BaseMapper<Article, ArticleDTO>
    {
        public override Article Map(ArticleDTO element)
        {
            return new Article
            {
                Title = element.Title,
                Content = element.Content,
                BlogId = element.BlogId.GetValueOrDefault()
            };
        }

        public override ArticleDTO Map(Article element)
        {
            return new ArticleDTO
            {
                ArticleId = element.ArticleId,
                Title = element.Title,
                Content = element.Content,
                BlogId = element.BlogId,
            };
        }
    }
}