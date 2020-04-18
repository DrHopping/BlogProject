using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Entities.Base;

namespace DAL.Entities
{
    public class Tag : EntityBase<int>
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public IEnumerable<ArticleTag> ArticleTags { get; set; }

    }
}