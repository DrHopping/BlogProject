using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class TagCreateModel
    {
        [Required]
        public string Name { get; set; }
    }
}