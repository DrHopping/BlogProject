using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BLL.DTO;

namespace Blog.Models
{
    public class ArticleCreateModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int BlogId { get; set; }
        public IEnumerable<TagCreateModel> Tags { get; set; }

    }
}