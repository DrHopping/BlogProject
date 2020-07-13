using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Base;

namespace DAL.Entities
{
    public class Article : EntityBase<int>
    {
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(5000)]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("Blog")]
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<ArticleTag> ArticleTags { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}