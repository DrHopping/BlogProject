using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class ArticleUpdateModel
    {
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(10000)]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<TagCreateModel> Tags { get; set; }

    }
}