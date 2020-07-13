using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BLL.DTO;

namespace Blog.Models
{
    public class ArticleCreateModel
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        [MaxLength(10000)]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public int BlogId { get; set; }
        public IEnumerable<TagCreateModel> Tags { get; set; }

    }
}