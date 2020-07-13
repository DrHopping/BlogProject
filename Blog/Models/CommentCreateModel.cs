using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class CommentCreateModel
    {
        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
        [Required]
        public int ArticleId { get; set; }

    }
}