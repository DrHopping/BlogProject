using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Blog.Models
{
    public class RegisterModel
    {
        [Required]
        [MaxLength(30)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}