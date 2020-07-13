using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class BlogCreateModel
    {
        [MaxLength(50)] [Required] public string Name { get; set; }
        [MaxLength(200)] [Required] public string Description { get; set; }
    }
}