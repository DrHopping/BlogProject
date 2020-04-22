using System.Collections.Generic;
using BLL.DTO;

namespace Blog.Helpers
{
    public class ArticleEqualityComparer : IEqualityComparer<ArticleDTO>
    {
        public bool Equals(ArticleDTO x, ArticleDTO y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ArticleDTO obj)
        {
            return obj.Id.GetValueOrDefault();
        }
    }
}