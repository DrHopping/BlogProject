using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class BlogCreateModel
    {
        [Required]
        public string Name { get; set; }
    }
}