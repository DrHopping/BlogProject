using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class TagCreateModel
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
    }
}