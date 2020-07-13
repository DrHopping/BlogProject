using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO
{
    public class ArticleDTO
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? BlogId { get; set; }
        public string BlogName { get; set; }
        public string AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<CommentDTO> Comments { get; set; }
        public IEnumerable<TagDTO> Tags { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}