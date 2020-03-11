using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO
{
    public class ArticleDTO
    {
        public int? ArticleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? BlogId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
        public ICollection<TagDTO> Tags { get; set; }
    }
}