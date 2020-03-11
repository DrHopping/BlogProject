using BLL.DTO;
using DAL.Entities;

namespace BLL.Mappers
{
    public class ArticleMapper : BaseMapper<Article, ArticleDto>
    {
        public override Article Map(ArticleDto element)
        {
            throw new System.NotImplementedException();
        }

        public override ArticleDto Map(Article element)
        {
            throw new System.NotImplementedException();
        }
    }
}