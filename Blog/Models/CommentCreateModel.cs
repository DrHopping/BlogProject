using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class CommentCreateModel
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int ArticleId { get; set; }

    }
}