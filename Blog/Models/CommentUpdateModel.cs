using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class CommentUpdateModel
    {
        [Required]
        public string Content { get; set; }
    }
}