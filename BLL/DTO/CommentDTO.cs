using System;

namespace BLL.DTO
{
    public class CommentDTO
    {
        public int? CommentId { get; set; }
        public string Content { get; set; }
        public int? ArticleId { get; set; }
        public string CreatorId { get; set; }
        public string CreatorUsername { get; set; }
    }
}