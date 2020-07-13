using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class CommentUpdateModel
    {
        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
    }
}