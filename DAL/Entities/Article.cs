using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace DAL.Entities
{
    public class Article
    {
        public int ArticleId { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(5000)]
        public string Content { get; set; }

        [ForeignKey("Blog")]
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Tag> Tags { get; set; }

    }
}