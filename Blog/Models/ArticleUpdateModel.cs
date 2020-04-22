using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class ArticleUpdateModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public IEnumerable<TagCreateModel> Tags { get; set; }

    }
}