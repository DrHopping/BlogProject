using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class UserUpdateModel
    {
        [MaxLength(30)]
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        [MaxLength(200)]
        public string Info { get; set; }

    }
}